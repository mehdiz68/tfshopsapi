using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TfShop.Infrastructure.Search
{
    /// <summary>
    /// ساختِ ایندکسِ جستجو از روی جدولِ SearchEngineFacts و فرستادنش به Meilisearch.
    ///
    /// SearchEngineFacts همون جدولِ denormalizedیه که مکانیزمِ قدیمی هم ازش می‌خوند - پس
    /// منبعِ داده عوض نشده و هر چیزی که تا حالا قابلِ جستجو بود همچنان هست. فقط روشِ
    /// جستجو کردنش عوض شده.
    ///
    /// ElementTypeID ها (طبق استور پروسیجرِ MainSearch3 قدیمی):
    ///     1 = برند، 2 = دسته‌بندی، 3 = محصول
    /// خروجیِ اکشن‌ها همچنان TypeId 1/2/3/4 برمی‌گردونه (۴ = خودِ محصول، ۳ = پیشنهادِ
    /// «فلان‌چیز در دسته‌ی فلان») تا فرانت دست‌نخورده بمونه.
    /// </summary>
    public static class SearchIndexer
    {
        /// <summary>
        /// مترادف‌ها. Meilisearch دوطرفه حسابشون نمی‌کنه مگه خودمون هر دو جهت رو بدیم، پس
        /// AddPair این کار رو می‌کنه.
        ///
        /// این لیست عمداً کوچیک و دستیه: مترادفِ غلط بدتر از نبودِ مترادفه (کاربر دنبالِ
        /// «شارژر» می‌گرده و «کابل» می‌گیره). با دیدنِ لاگِ جستجوها (جدول SearchLogs) می‌شه
        /// به‌مرور کامل‌ترش کرد.
        /// </summary>
        public static Dictionary<string, string[]> BuildSynonyms()
        {
            var map = new Dictionary<string, List<string>>();

            Action<string, string> addPair = (a, b) =>
            {
                string na = PersianText.Normalize(a);
                string nb = PersianText.Normalize(b);
                if (string.IsNullOrEmpty(na) || string.IsNullOrEmpty(nb) || na == nb) return;
                if (!map.ContainsKey(na)) map[na] = new List<string>();
                if (!map.ContainsKey(nb)) map[nb] = new List<string>();
                if (!map[na].Contains(nb)) map[na].Add(nb);
                if (!map[nb].Contains(na)) map[nb].Add(na);
            };

            // نگارشِ لاتین/فارسیِ برندها و واژه‌های پرتکرارِ همین فروشگاه (ابزار و لوازم)
            addPair("لپ تاپ", "laptop");
            addPair("لپ تاپ", "نوت بوک");
            addPair("موبایل", "mobile");
            addPair("موبایل", "گوشی");
            addPair("هدفون", "هدفن");
            addPair("هدفون", "headphone");
            addPair("شارژر", "charger");
            addPair("کابل", "cable");
            addPair("پیچ گوشتی", "پیچگوشتی");
            addPair("آچار", "اچار");
            addPair("انبردست", "انبر دست");
            addPair("دریل", "drill");
            addPair("فرز", "سنگ فرز");
            addPair("اره", "saw");
            addPair("چکش", "چکش کاری");
            addPair("متر", "متر نواری");
            addPair("جعبه ابزار", "کیف ابزار");
            addPair("باتری", "باطری");
            addPair("شلنگ", "شیلنگ");
            addPair("گیربکس", "گیر بکس");

            var result = new Dictionary<string, string[]>();
            foreach (var kv in map) result[kv.Key] = kv.Value.ToArray();
            return result;
        }

        /// <summary>
        /// بازسازیِ کاملِ ایندکس. چون کلِ داده ~۹ هزار ردیف و ۳ مگابایته، بازسازیِ کامل
        /// ساده‌تر و مطمئن‌تر از هم‌گام‌سازیِ تدریجیه و چند ثانیه بیشتر طول نمی‌کشه.
        /// </summary>
        public static async Task<string> RebuildAsync(UnitOfWork.UnitOfWorkClass uow)
        {
            if (!MeiliClient.Configured)
                return "MeiliUrl تنظیم نشده است.";

            await MeiliClient.EnsureIndexAsync();
            await MeiliClient.ApplySettingsAsync(BuildSynonyms());

            // دسته‌بندیِ hidden نه خودش باید توی جستجو بیاد نه محصولاتش.
            //
            // این فیلتر عمداً سرِ ایندکس اعمال می‌شه نه سرِ کوئری: این‌طوری محصولِ مخفی اصلاً
            // وارد موتور نمی‌شه و هیچ مسیری (هدر، صفحه‌ی کامل، پیشنهادِ دسته) نمی‌تونه لوش بده.
            // اگه سرِ کوئری فیلتر می‌شد، کافی بود یه جا فیلتر جا بیفته تا نشت کنه.
            //
            // دو شرط جداست چون نوعِ ۲ خودِ دسته‌بندیه (شناسه‌ش توی ElementID) ولی نوعِ ۳ محصوله
            // (دسته‌ش توی CatId). روی دیتای فعلی هیچ محصولی هم‌زمان توی گروهِ مخفی و
            // قابل‌نمایش نیست، پس این قانون ابهامی نداره.
            var rows = uow.SearchEngineFactRepository.Sql(x => new SearchFactRow(),
                @"select f.ElementID, f.ElementTitle, f.ElementAbstract, f.ElementTypeID, f.ElementMainKeyword,
                         f.PageAddress, f.CatId, f.ProductId, f.model, f.size, f.color, f.garanty,
                         case when f.Cover is null then null
                              else (select replace(filename,'lg_','xs_') from attachments where id = f.Cover) end as CoverFile
                  from SearchEngineFacts f
                  where not exists (select 1 from ProductCategories c
                                    where c.Id = f.CatId and c.hidden = 1)
                    and not (f.ElementTypeID = 2
                             and exists (select 1 from ProductCategories c2
                                         where c2.Id = f.ElementID and c2.hidden = 1))").ToList();

            // نامِ دسته‌بندی و برند هم قابلِ جستجو می‌شه: کاربر «مته دیوالت» می‌زنه در حالی که
            // عنوانِ محصول فقط «مته» ئه و «دیوالت» اسمِ برنده. مکانیزمِ قدیمی این رو نمی‌گرفت.
            var cats = uow.SearchEngineFactRepository.Sql(x => new CatRow(),
                "select Id, Name, replace(isnull(PageAddress2,''),' ','-') as PageAddress2 from ProductCategories where hidden = 0").ToList()
                .GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());

            var docs = new List<object>(rows.Count);
            foreach (var r in rows)
            {
                string catName = "";
                string catPage = "";
                if (r.CatId.HasValue && cats.ContainsKey(r.CatId.Value))
                {
                    catName = cats[r.CatId.Value].Name ?? "";
                    catPage = cats[r.CatId.Value].PageAddress2 ?? "";
                }

                string title = r.ElementTitle ?? "";

                docs.Add(new
                {
                    // شناسه باید یکتا و از نظر Meili معتبر باشه (حروف/عدد/خط‌تیره/آندرلاین)
                    id = r.ElementTypeID + "_" + r.ElementID + "_" + (r.ProductId ?? 0),

                    // ----- فیلدهای قابلِ جستجو: همه نرمال‌شده -----
                    title = PersianText.Normalize(title),
                    titleCompact = PersianText.Compact(title),
                    keywords = PersianText.Normalize(r.ElementMainKeyword ?? ""),
                    // چکیده معمولاً بلنده و اگه وزنِ زیاد بگیره نویز می‌سازه؛ آخرین فیلدِ
                    // searchable ئه پس کمترین وزن رو داره.
                    @abstract = PersianText.Normalize(Truncate(r.ElementAbstract, 1200)),
                    model = PersianText.Normalize(r.model ?? ""),
                    size = PersianText.Normalize(r.size ?? ""),
                    color = PersianText.Normalize(r.color ?? ""),
                    garanty = PersianText.Normalize(r.garanty ?? ""),
                    categoryName = PersianText.Normalize(catName),
                    brandName = "",

                    // ----- فیلدهای نمایشی: متنِ اصلی، دست‌نخورده -----
                    // خروجیِ API باید عیناً همون چیزی باشه که قبلاً بود، پس نرمال‌شده نمایش داده نمی‌شه.
                    displayTitle = title,
                    displayCategory = catName,
                    displayCategoryPage = catPage,
                    // این سه تا برای بازسازیِ عینِ عنوانِ قدیمی لازمن:
                    //   ElementTitle + " مدل X" + " سایز Y" + " Z"
                    displayModel = r.model ?? "",
                    displaySize = r.size ?? "",
                    displayColor = r.color ?? "",

                    elementId = r.ElementID,
                    productId = r.ProductId ?? 0,
                    typeId = r.ElementTypeID,
                    catId = r.CatId ?? 0,
                    pageAddress = r.PageAddress ?? "",
                    cover = r.CoverFile ?? "default-thumbnail.jpg",

                    // فعلاً ثابت. جای طبیعیِ سیگنالِ محبوبیت (فروش/بازدید) همین‌جاست و
                    // rankingRules از قبل ازش استفاده می‌کنه.
                    popularity = 0
                });
            }

            await MeiliClient.ClearDocumentsAsync();

            // دسته‌دسته فرستاده می‌شه تا یه درخواستِ چندمگابایتی ساخته نشه
            int batch = 1000;
            for (int i = 0; i < docs.Count; i += batch)
            {
                var slice = docs.Skip(i).Take(batch).ToList();
                bool ok = await MeiliClient.AddDocumentsAsync(slice);
                if (!ok) return "ارسال دسته‌ی " + (i / batch + 1) + " ناموفق بود.";
            }

            // واژگانِ اصلاحِ املا از همون عنوان‌های نرمال‌شده ساخته می‌شه
            SearchVocabulary.Rebuild(rows.Select(r => PersianText.Normalize(r.ElementTitle ?? "")));

            return "ایندکس با " + docs.Count + " سند بازسازی شد. واژگان: " + SearchVocabulary.Count;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max);
        }

        private static int vocabLoading;

        /// <summary>
        /// اطمینان از پر بودنِ واژگانِ اصلاحِ املا.
        ///
        /// واژگان فقط در حافظه نگه‌داری می‌شود و تنها جایی که پرش می‌کرد RebuildAsync بود.
        /// یعنی بعد از هر ری‌استارتِ app pool (دیپلوی، recycle، ری‌استارتِ سرور) خالی می‌ماند
        /// و SearchVocabulary.Correct بی‌صدا null برمی‌گرداند - نتیجه‌اش این بود که کوئریِ
        /// غلط‌املاییِ کوتاه مثلِ «درل» که Meili خودش (به‌خاطرِ سقفِ طولِ واژه) اصلاحش نمی‌کند،
        /// هیچ نتیجه‌ای نمی‌داد، در حالی که قبلاً همه‌ی دریل‌ها را می‌آورد.
        ///
        /// این متد با یک کوئریِ سبک (فقط ستونِ عنوان) دوباره می‌سازدش، بدونِ نیاز به بازسازیِ
        /// کاملِ ایندکس. فقط وقتی صدا زده می‌شود که Meili هیچ نتیجه‌ای نداده باشد، پس روی مسیرِ
        /// داغِ جستجو نیست. اگر هم‌زمان چند درخواست برسند، تنها یکی بار را برمی‌دارد و بقیه
        /// بدونِ اصلاح رد می‌شوند (همان رفتارِ قبلی) تا صف نبندند.
        /// </summary>
        public static void EnsureVocabularyLoaded()
        {
            if (SearchVocabulary.Count > 0) return;
            if (Interlocked.CompareExchange(ref vocabLoading, 1, 0) != 0) return;

            try
            {
                using (var uow = new UnitOfWork.UnitOfWorkClass())
                {
                    // همان فیلترِ hidden ای که RebuildAsync استفاده می‌کند، تا واژه‌ای از
                    // دسته‌بندیِ مخفی وارد پیشنهادها نشود.
                    var titles = uow.SearchEngineFactRepository.Sql(x => new TitleRow(),
                        @"select f.ElementTitle
                          from SearchEngineFacts f
                          where not exists (select 1 from ProductCategories c
                                            where c.Id = f.CatId and c.hidden = 1)
                            and not (f.ElementTypeID = 2
                                     and exists (select 1 from ProductCategories c2
                                                 where c2.Id = f.ElementID and c2.hidden = 1))").ToList();

                    SearchVocabulary.Rebuild(titles.Select(t => PersianText.Normalize(t.ElementTitle ?? "")));

                    TfShop.Infrastructure.EventLog.Logger.Add(5, "api", "search-vocab-load", true, 200,
                        "واژگان از دیتابیس بارگذاری شد: " + SearchVocabulary.Count,
                        DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                }
            }
            catch (Exception ex)
            {
                TfShop.Infrastructure.EventLog.Logger.Add(5, "api", "search-vocab-load", false, 500, ex.Message,
                    DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }
            finally
            {
                Interlocked.Exchange(ref vocabLoading, 0);
            }
        }

        public class TitleRow
        {
            public string ElementTitle { get; set; }
        }

        public class SearchFactRow
        {
            public int ElementID { get; set; }
            public string ElementTitle { get; set; }
            public string ElementAbstract { get; set; }
            public int ElementTypeID { get; set; }
            public string ElementMainKeyword { get; set; }
            public string PageAddress { get; set; }
            public int? CatId { get; set; }
            public int? ProductId { get; set; }
            public string model { get; set; }
            public string size { get; set; }
            public string color { get; set; }
            public string garanty { get; set; }
            public string CoverFile { get; set; }
        }

        public class CatRow
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PageAddress2 { get; set; }
        }
    }
}
