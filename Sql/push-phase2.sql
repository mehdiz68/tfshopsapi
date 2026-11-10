/* =====================================================================
   اعلانِ مرورگر - فاز ۲: علاقه‌مندی، پیام‌هایِ خودکار، سقفِ ارسال، آزمونِ A/B، فروشِ حاصل.
   مثلِ push-notifications.sql بیرون از مدلِ EF. بعد از آن اجرا شود؛ اجرایِ دوباره بی‌ضرر است.
   GOها لازم‌اند: SQL Server هر batch را پیش از اجرا کامپایل می‌کند و ستونی که در همان batch
   اضافه شده هنوز شناخته نمی‌شود (Invalid column name).
   ===================================================================== */

SET XACT_ABORT ON;
BEGIN TRAN;

/* ---------- عضو: آخرین خرید (فیلترِ خریدار/غیرخریدار) ---------- */
IF COL_LENGTH('dbo.PushSubscribers', 'LastPurchaseAt') IS NULL
    ALTER TABLE dbo.PushSubscribers ADD LastPurchaseAt DATETIME NULL;

/* ---------- علاقه به گروه‌هایِ کالا ----------
   امتیاز با هر رویداد اضافه می‌شود (بازدید ۱، سبد ۳، خرید ۵) و هر ۳۰ روز نصف می‌شود؛ کم‌رنگ‌شدن
   هنگامِ خواندن/نوشتن از رویِ UpdatedAt حساب می‌شود، نه با یک jobِ جدا. */
IF OBJECT_ID('dbo.PushInterests') IS NULL
CREATE TABLE dbo.PushInterests (
    SubscriberId    INT       NOT NULL,
    CategoryId      INT       NOT NULL,
    Score           FLOAT     NOT NULL,
    UpdatedAt       DATETIME  NOT NULL,
    CONSTRAINT PK_PushInterests PRIMARY KEY (SubscriberId, CategoryId)
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushInterests_Category')
    CREATE INDEX IX_PushInterests_Category ON dbo.PushInterests(CategoryId) INCLUDE (Score, UpdatedAt);

/* ---------- سبدِ خرید (برایِ پیامِ سبدِ رهاشده) ---------- */
IF OBJECT_ID('dbo.PushCarts') IS NULL
CREATE TABLE dbo.PushCarts (
    SubscriberId    INT       NOT NULL CONSTRAINT PK_PushCarts PRIMARY KEY,
    ProductId       INT       NOT NULL,                     -- آخرین کالایِ اضافه‌شده
    LastAddAt       DATETIME  NOT NULL,
    RemindedAt      DATETIME  NULL,
    ClosedAt        DATETIME  NULL                          -- خرید شد / دیگر یادآوری نشود
);

/* ---------- پیام‌ها: نوع، فیلترهایِ تازه، A/B، فروش ---------- */
IF COL_LENGTH('dbo.PushCampaigns', 'Kind') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD Kind TINYINT NOT NULL CONSTRAINT DF_PushCampaigns_Kind DEFAULT 0;   -- 0 دستی، 1 خودکار
IF COL_LENGTH('dbo.PushCampaigns', 'AutomationKey') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD AutomationKey VARCHAR(40) NULL;
IF COL_LENGTH('dbo.PushCampaigns', 'FilterCategoryIds') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD FilterCategoryIds VARCHAR(400) NULL;                              -- شناسه‌هایِ گروه با ویرگول
IF COL_LENGTH('dbo.PushCampaigns', 'FilterBuyer') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD FilterBuyer TINYINT NULL;                                          -- 1 خریدار، 2 غیرخریدار
IF COL_LENGTH('dbo.PushCampaigns', 'IgnoreCap') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD IgnoreCap BIT NOT NULL CONSTRAINT DF_PushCampaigns_IgnoreCap DEFAULT 0;
IF COL_LENGTH('dbo.PushCampaigns', 'TitleB') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD TitleB NVARCHAR(120) NULL;                                         -- عنوانِ دوم برایِ A/B
IF COL_LENGTH('dbo.PushCampaigns', 'AbPhase') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD AbPhase TINYINT NOT NULL CONSTRAINT DF_PushCampaigns_AbPhase DEFAULT 0; -- 0 بدونِ A/B، 1 آزمون، 2 منتظرِ نتیجه، 3 ارسالِ برنده
IF COL_LENGTH('dbo.PushCampaigns', 'AbWinner') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD AbWinner TINYINT NULL;                                             -- 0 عنوانِ اول، 1 عنوانِ دوم
IF COL_LENGTH('dbo.PushCampaigns', 'SkippedCount') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD SkippedCount INT NOT NULL CONSTRAINT DF_PushCampaigns_SkippedCount DEFAULT 0; -- ردشده به‌خاطرِ سقفِ روزانه
IF COL_LENGTH('dbo.PushCampaigns', 'ConversionCount') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD ConversionCount INT NOT NULL CONSTRAINT DF_PushCampaigns_ConversionCount DEFAULT 0;
IF COL_LENGTH('dbo.PushCampaigns', 'ConversionAmount') IS NULL
    ALTER TABLE dbo.PushCampaigns ADD ConversionAmount BIGINT NOT NULL CONSTRAINT DF_PushCampaigns_ConversionAmount DEFAULT 0;

/* ---------- ارسال‌ها: A/B، تکرارِ پیامِ خودکار، فروش ---------- */
IF COL_LENGTH('dbo.PushDeliveries', 'Variant') IS NULL
    ALTER TABLE dbo.PushDeliveries ADD Variant TINYINT NULL;
IF COL_LENGTH('dbo.PushDeliveries', 'DedupeKey') IS NULL
    ALTER TABLE dbo.PushDeliveries ADD DedupeKey VARCHAR(100) NOT NULL CONSTRAINT DF_PushDeliveries_DedupeKey DEFAULT '';
IF COL_LENGTH('dbo.PushDeliveries', 'OrderId') IS NULL
    ALTER TABLE dbo.PushDeliveries ADD OrderId UNIQUEIDENTIFIER NULL;
IF COL_LENGTH('dbo.PushDeliveries', 'OrderAmount') IS NULL
    ALTER TABLE dbo.PushDeliveries ADD OrderAmount BIGINT NULL;
IF COL_LENGTH('dbo.PushDeliveries', 'ConvertedAt') IS NULL
    ALTER TABLE dbo.PushDeliveries ADD ConvertedAt DATETIME NULL;
COMMIT;
GO

/* پیامِ خودکار به یک عضو چند بار می‌رود (هر سفارش، هر کالا) - یکتایی با DedupeKey */
BEGIN TRAN;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PushDeliveries_Campaign_Subscriber')
    DROP INDEX UX_PushDeliveries_Campaign_Subscriber ON dbo.PushDeliveries;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PushDeliveries_Campaign_Subscriber_Key')
    CREATE UNIQUE INDEX UX_PushDeliveries_Campaign_Subscriber_Key ON dbo.PushDeliveries(CampaignId, SubscriberId, DedupeKey);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushDeliveries_Clicked')
    CREATE INDEX IX_PushDeliveries_Clicked ON dbo.PushDeliveries(ClickedAt) INCLUDE (SubscriberId, CampaignId, ConvertedAt) WHERE ClickedAt IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushDeliveries_SubscriberSent')
    CREATE INDEX IX_PushDeliveries_SubscriberSent ON dbo.PushDeliveries(SubscriberId, SentAt) INCLUDE (CampaignId, Status);
COMMIT;
GO

BEGIN TRAN;
/* ---------- تنظیمات (از پنل قابلِ تغییر) ---------- */
IF OBJECT_ID('dbo.PushSettings') IS NULL
CREATE TABLE dbo.PushSettings (
    [Key]           VARCHAR(40)   NOT NULL CONSTRAINT PK_PushSettings PRIMARY KEY,
    Value           NVARCHAR(200) NOT NULL
);
MERGE dbo.PushSettings AS t
USING (VALUES
    ('DailyCap', N'1'),           -- حداکثر پیامِ تبلیغاتیِ دستی برایِ هر نفر در روز
    ('QuietStart', N'23'),        -- ساعتِ شروعِ سکوت
    ('QuietEnd', N'8'),           -- ساعتِ پایانِ سکوت
    ('AbPercent', N'10'),         -- درصدِ مخاطبانِ آزمونِ A/B
    ('AbHours', N'2'),            -- صبر تا انتخابِ برنده
    ('CartHours', N'3'),          -- سبدِ رهاشده بعد از چند ساعت
    ('PriceDropPercent', N'5'),   -- حداقلِ کاهشِ قیمت برایِ پیام
    ('ConversionDays', N'7')      -- فروش تا چند روز بعد از کلیک به پیام نسبت داده شود
) AS s([Key], Value) ON t.[Key] = s.[Key]
WHEN NOT MATCHED THEN INSERT ([Key], Value) VALUES (s.[Key], s.Value);

/* ---------- قیمتِ قبلیِ کالاهایِ موردِ علاقه (برایِ پیامِ کاهشِ قیمت) ---------- */
IF OBJECT_ID('dbo.PushPriceSnapshots') IS NULL
CREATE TABLE dbo.PushPriceSnapshots (
    ProductPriceId  INT       NOT NULL CONSTRAINT PK_PushPriceSnapshots PRIMARY KEY,
    Price           BIGINT    NOT NULL,
    CapturedAt      DATETIME  NOT NULL
);

/* ---------- نشانگرِ پیشرفتِ jobها (مثلاً آخرین وضعیتِ سفارشِ بررسی‌شده) ---------- */
IF OBJECT_ID('dbo.PushJobState') IS NULL
CREATE TABLE dbo.PushJobState (
    [Key]           VARCHAR(40)   NOT NULL CONSTRAINT PK_PushJobState PRIMARY KEY,
    Value           NVARCHAR(100) NOT NULL
);

COMMIT;
GO

BEGIN TRAN;
/* ---------- پیام‌هایِ خودکار: هرکدام یک ردیف در PushCampaigns (Kind = 1) ----------
   Status 7 = فعال، 8 = خاموش. متن‌ها از پنل قابلِ ویرایش‌اند؛ {name} نامِ کاربر، {product} نامِ کالا،
   {price} قیمتِ جدید، {oldprice} قیمتِ قبلی، {order} شماره‌ی سفارش، {status} وضعیتِ سفارش. */
MERGE dbo.PushCampaigns AS t
USING (VALUES
    ('stock',      N'پیامِ خودکار: موجود شد',            N'{product} موجود شد!',                 N'کالایی که منتظرش بودید دوباره موجود است. تا تمام نشده سفارش دهید.', N'https://www.tfshops.com/'),
    ('pricedrop',  N'پیامِ خودکار: کاهشِ قیمت',          N'قیمتِ {product} کم شد',               N'از {oldprice} به {price} تومان - کالایی که در علاقه‌مندی‌هایتان دارید.', N'https://www.tfshops.com/'),
    ('cart',       N'پیامِ خودکار: سبدِ رهاشده',         N'سبدِ خریدتان منتظرِ شماست',          N'{product} هنوز در سبدِ خریدتان است. خریدتان را تکمیل کنید.',          N'https://www.tfshops.com/cart'),
    ('unpaid',     N'پیامِ خودکار: سفارشِ پرداخت‌نشده',  N'سفارشِ {order} منتظرِ پرداخت است',   N'پرداختِ سفارشتان هنوز انجام نشده. تا قبل از لغوِ خودکار پرداخت کنید.', N'https://www.tfshops.com/profile/orders'),
    ('orderstatus',N'پیامِ خودکار: وضعیتِ سفارش',        N'سفارشِ {order}: {status}',            N'برایِ دیدنِ جزئیاتِ سفارش کلیک کنید.',                                 N'https://www.tfshops.com/profile/orders')
) AS s(AutomationKey, Name, Title, Body, Url) ON t.Kind = 1 AND t.AutomationKey = s.AutomationKey
WHEN NOT MATCHED THEN INSERT (Kind, AutomationKey, Name, Title, Body, Url, Status, TtlHours, StartedAt)
    VALUES (1, s.AutomationKey, s.Name, s.Title, s.Body, s.Url, 7, 24, GETDATE());

COMMIT;
