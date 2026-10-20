/* =====================================================================
   جدول‌هایِ اعلانِ مرورگر (Web Push) - فاز ۱

   عمداً بیرون از مدلِ EF (TfShopDbContext). آن مدل در چند برنامه (API، پنل،
   TfScheduler) نسخه‌ی جدا دارد و با AutomaticMigrationDataLossAllowed = true
   هر بار خودکار migrate می‌شود؛ اگر این جدول‌ها داخلِ مدل بودند، اجرایِ هر
   نسخه‌ی قدیمی‌تر آن‌ها را - با همه‌ی عضوها - پاک می‌کرد. EF جدول‌هایی را که
   در مدلش نیستند لمس نمی‌کند.

   اجرا: یک بار رویِ دیتابیسِ اصلی. چند بار اجرا کردنش بی‌ضرر است.
   ===================================================================== */

SET XACT_ABORT ON;
BEGIN TRAN;

/* ---------- عضوها (هر مرورگر/دستگاه یک ردیف) ---------- */
IF OBJECT_ID('dbo.PushSubscribers') IS NULL
CREATE TABLE dbo.PushSubscribers (
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PushSubscribers PRIMARY KEY,
    Endpoint        NVARCHAR(1000)    NOT NULL,
    EndpointHash    BINARY(32)        NOT NULL,             -- SHA-256ِ Endpoint برایِ ایندکسِ یکتا
    P256dh          VARCHAR(200)      NOT NULL,
    Auth            VARCHAR(100)      NOT NULL,
    UserId          NVARCHAR(128)     NULL,                 -- اگر کاربر وارد شده باشد
    IsActive        BIT               NOT NULL CONSTRAINT DF_PushSubscribers_IsActive DEFAULT 1,
    CreatedAt       DATETIME          NOT NULL CONSTRAINT DF_PushSubscribers_CreatedAt DEFAULT GETDATE(),
    LastSeenAt      DATETIME          NOT NULL CONSTRAINT DF_PushSubscribers_LastSeenAt DEFAULT GETDATE(),
    UnsubscribedAt  DATETIME          NULL,
    Ip              VARCHAR(45)       NULL,
    Province        NVARCHAR(50)      NULL,
    City            NVARCHAR(80)      NULL,
    GeoSource       TINYINT           NOT NULL CONSTRAINT DF_PushSubscribers_GeoSource DEFAULT 0, -- 0 نامعلوم، 1 از IP، 2 از آدرسِ کاربر
    DeviceType      TINYINT           NOT NULL CONSTRAINT DF_PushSubscribers_DeviceType DEFAULT 0, -- 0 نامعلوم، 1 موبایل، 2 تبلت، 3 دسکتاپ
    Browser         NVARCHAR(30)      NULL,
    Os              NVARCHAR(30)      NULL,
    UserAgent       NVARCHAR(400)     NULL,
    FailCount       INT               NOT NULL CONSTRAINT DF_PushSubscribers_FailCount DEFAULT 0
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PushSubscribers_EndpointHash')
    CREATE UNIQUE INDEX UX_PushSubscribers_EndpointHash ON dbo.PushSubscribers(EndpointHash);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushSubscribers_UserId')
    CREATE INDEX IX_PushSubscribers_UserId ON dbo.PushSubscribers(UserId) WHERE UserId IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushSubscribers_Active')
    CREATE INDEX IX_PushSubscribers_Active ON dbo.PushSubscribers(IsActive, LastSeenAt) INCLUDE (Province, City, DeviceType, UserId);

/* ---------- پیام‌ها (کمپین‌ها) ---------- */
IF OBJECT_ID('dbo.PushCampaigns') IS NULL
CREATE TABLE dbo.PushCampaigns (
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PushCampaigns PRIMARY KEY,
    Name            NVARCHAR(150)     NOT NULL,             -- نامِ داخلی برایِ پنل
    Title           NVARCHAR(120)     NOT NULL,
    Body            NVARCHAR(400)     NOT NULL,
    IconUrl         NVARCHAR(500)     NULL,
    ImageUrl        NVARCHAR(500)     NULL,
    Url             NVARCHAR(1000)    NOT NULL,
    Button1Title    NVARCHAR(40)      NULL,
    Button1Url      NVARCHAR(1000)    NULL,
    Button2Title    NVARCHAR(40)      NULL,
    Button2Url      NVARCHAR(1000)    NULL,
    TtlHours        INT               NOT NULL CONSTRAINT DF_PushCampaigns_TtlHours DEFAULT 24,
    -- مخاطبان (خالی = بدونِ محدودیت)
    FilterProvince  NVARCHAR(400)     NULL,                 -- چند استان با «،»
    FilterCity      NVARCHAR(400)     NULL,
    FilterDevice    TINYINT           NULL,                 -- 1 موبایل، 2 تبلت، 3 دسکتاپ
    FilterLoggedIn  TINYINT           NULL,                 -- 1 فقط واردشده، 2 فقط مهمان
    FilterActiveDays INT              NULL,                 -- فقط کسانی که در N روزِ اخیر سایت را دیده‌اند
    -- وضعیت
    Status          TINYINT           NOT NULL CONSTRAINT DF_PushCampaigns_Status DEFAULT 0, -- 0 پیش‌نویس، 1 در صف، 2 در حالِ ارسال، 3 ارسال شد، 4 لغو شد، 5 خطا
    ScheduledAt     DATETIME          NULL,
    StartedAt       DATETIME          NULL,
    FinishedAt      DATETIME          NULL,
    CreatedAt       DATETIME          NOT NULL CONSTRAINT DF_PushCampaigns_CreatedAt DEFAULT GETDATE(),
    CreatedBy       NVARCHAR(128)     NULL,
    -- شمارنده‌ها (برایِ فهرست و گزارشِ سریع؛ منبعِ اصلی PushDeliveries است)
    TargetCount     INT               NOT NULL CONSTRAINT DF_PushCampaigns_TargetCount DEFAULT 0,
    SentCount       INT               NOT NULL CONSTRAINT DF_PushCampaigns_SentCount DEFAULT 0,
    FailedCount     INT               NOT NULL CONSTRAINT DF_PushCampaigns_FailedCount DEFAULT 0,
    GoneCount       INT               NOT NULL CONSTRAINT DF_PushCampaigns_GoneCount DEFAULT 0,
    ShownCount      INT               NOT NULL CONSTRAINT DF_PushCampaigns_ShownCount DEFAULT 0,
    ClickCount      INT               NOT NULL CONSTRAINT DF_PushCampaigns_ClickCount DEFAULT 0,
    Click1Count     INT               NOT NULL CONSTRAINT DF_PushCampaigns_Click1Count DEFAULT 0,
    Click2Count     INT               NOT NULL CONSTRAINT DF_PushCampaigns_Click2Count DEFAULT 0,
    LastError       NVARCHAR(400)     NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushCampaigns_Status')
    CREATE INDEX IX_PushCampaigns_Status ON dbo.PushCampaigns(Status, ScheduledAt);

/* ---------- ارسال به هر عضو ---------- */
IF OBJECT_ID('dbo.PushDeliveries') IS NULL
CREATE TABLE dbo.PushDeliveries (
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PushDeliveries PRIMARY KEY,
    Token           UNIQUEIDENTIFIER  NOT NULL,             -- شناسه‌ی غیرقابل‌حدس برایِ رهگیریِ نمایش و کلیک
    CampaignId      INT               NOT NULL,
    SubscriberId    INT               NOT NULL,
    Status          TINYINT           NOT NULL,             -- 1 پذیرفته شد، 2 خطا، 3 اشتراک باطل (404/410)
    HttpStatus      SMALLINT          NULL,
    Error           NVARCHAR(300)     NULL,
    SentAt          DATETIME          NOT NULL CONSTRAINT DF_PushDeliveries_SentAt DEFAULT GETDATE(),
    ShownAt         DATETIME          NULL,
    ClickedAt       DATETIME          NULL,
    ClickAction     TINYINT           NULL                  -- 0 خودِ پیام، 1 دکمه‌ی اول، 2 دکمه‌ی دوم
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PushDeliveries_Token')
    CREATE UNIQUE INDEX UX_PushDeliveries_Token ON dbo.PushDeliveries(Token);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PushDeliveries_Campaign_Subscriber')
    CREATE UNIQUE INDEX UX_PushDeliveries_Campaign_Subscriber ON dbo.PushDeliveries(CampaignId, SubscriberId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PushDeliveries_Subscriber')
    CREATE INDEX IX_PushDeliveries_Subscriber ON dbo.PushDeliveries(SubscriberId);

/* ---------- آمارِ روزانه‌ی پنجره‌ی دعوت (برایِ نرخِ پذیرش در داشبورد) ---------- */
IF OBJECT_ID('dbo.PushPromptDaily') IS NULL
CREATE TABLE dbo.PushPromptDaily (
    Day             DATE              NOT NULL CONSTRAINT PK_PushPromptDaily PRIMARY KEY,
    Shown           INT               NOT NULL CONSTRAINT DF_PushPromptDaily_Shown DEFAULT 0,  -- پنجره‌ی خودمان نمایش داده شد
    Accepted        INT               NOT NULL CONSTRAINT DF_PushPromptDaily_Accepted DEFAULT 0, -- «بله» در پنجره‌ی خودمان
    Dismissed       INT               NOT NULL CONSTRAINT DF_PushPromptDaily_Dismissed DEFAULT 0, -- «بعداً»
    Granted         INT               NOT NULL CONSTRAINT DF_PushPromptDaily_Granted DEFAULT 0,  -- اجازه در پنجره‌ی مرورگر
    Denied          INT               NOT NULL CONSTRAINT DF_PushPromptDaily_Denied DEFAULT 0    -- ردِ پنجره‌ی مرورگر
);

COMMIT;
