using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TfShop.ViewModels.Home;

namespace TfShop.Infrastructure.Search
{
    /// <summary>
    /// موتورِ جستجوی جدید. خروجی عیناً هم‌شکلِ استور پروسیجرِ MainSearch3 قدیمیه تا فرانت
    /// دست‌نخورده بمونه:
    ///     TypeId=1 -> برند   (۲ تا)
    ///     TypeId=2 -> دسته   (۲ تا)
    ///     TypeId=3 -> پیشنهادِ «فلان در دسته‌ی فلان» (۵ تا)
    ///     TypeId=4 -> محصول  (۶ تا)
    /// </summary>
    public static class SearchService
    {
        public const int BrandLimit = 2;
        public const int CategoryLimit = 2;
        public const int CategorySuggestLimit = 5;
        public const int ProductLimit = 6;

        /// <summary>
        /// جستجوی هدر. اگه Meilisearch در دسترس نباشه null برمی‌گرده تا صدازننده به مسیرِ
        /// قدیمیِ SQL برگرده - یعنی جستجوی سایت هیچ‌وقت کاملاً از کار نمی‌افته.
        /// </summary>
        public static async Task<List<MainSearch>> MainSearchAsync(string rawKeyword)
        {
            if (!MeiliClient.Configured) return null;

            string q = PersianText.Normalize(rawKeyword);
            if (string.IsNullOrEmpty(q)) return new List<MainSearch>();

            var products = await MeiliClient.SearchAsync(q, "typeId = 3", 40);
            if (products == null) return null; // سرویس در دسترس نیست -> fallback

            var brands = await MeiliClient.SearchAsync(q, "typeId = 1", BrandLimit);
            var cats = await MeiliClient.SearchAsync(q, "typeId = 2", CategoryLimit);

            // هیچ‌جا نتیجه نداد -> شاید غلطِ املایی بوده. یک‌بار با نسخه‌ی اصلاح‌شده تلاش می‌شه.
            if (products.Count == 0 && (brands == null || brands.Count == 0) && (cats == null || cats.Count == 0))
            {
                SearchIndexer.EnsureVocabularyLoaded();
                string corrected = SearchVocabulary.Correct(q);
                if (!string.IsNullOrEmpty(corrected))
                {
                    products = await MeiliClient.SearchAsync(corrected, "typeId = 3", 40) ?? products;
                    brands = await MeiliClient.SearchAsync(corrected, "typeId = 1", BrandLimit) ?? brands;
                    cats = await MeiliClient.SearchAsync(corrected, "typeId = 2", CategoryLimit) ?? cats;
                }
            }

            var result = new List<MainSearch>();

            // ---------- برندها ----------
            if (brands != null)
                foreach (var d in brands.Take(BrandLimit))
                    result.Add(new MainSearch
                    {
                        Id = Int(d, "elementId"),
                        Name = Str(d, "displayTitle"),
                        Title = Str(d, "displayTitle"),
                        PageAddress = Str(d, "pageAddress"),
                        TypeId = 1,
                        AttachementId = Str(d, "cover")
                    });

            // ---------- دسته‌بندی‌ها ----------
            if (cats != null)
                foreach (var d in cats.Take(CategoryLimit))
                    result.Add(new MainSearch
                    {
                        Id = Int(d, "elementId"),
                        Name = Str(d, "displayTitle"),
                        Title = Str(d, "displayTitle"),
                        PageAddress = Str(d, "pageAddress"),
                        TypeId = 2,
                        AttachementId = Str(d, "cover")
                    });

            // ---------- پیشنهادهای «X در دسته‌ی Y» ----------
            // نسخه‌ی قدیمی این‌ها رو با group by و «order by id desc» می‌ساخت، یعنی ترتیبش
            // ربطی به ارتباطِ نتیجه با کوئری نداشت. این‌جا از روی همون محصولاتِ رتبه‌بندی‌شده
            // ساخته می‌شه، پس دسته‌ای که مرتبط‌ترین محصول رو داره اول می‌آد.
            var seenCats = new HashSet<int>();
            foreach (var d in products)
            {
                int catId = Int(d, "catId");
                string catName = Str(d, "displayCategory");
                if (catId <= 0 || string.IsNullOrEmpty(catName)) continue;
                if (!seenCats.Add(catId)) continue;

                result.Add(new MainSearch
                {
                    Id = catId,
                    Name = catName,
                    Title = Str(d, "displayTitle"),
                    PageAddress = Str(d, "displayCategoryPage"),
                    TypeId = 3,
                    AttachementId = null
                });

                if (seenCats.Count >= CategorySuggestLimit) break;
            }

            // ---------- محصولات ----------
            // نسخه‌ی قدیمی این‌جا «order by NEWID()» داشت، یعنی شش محصولِ شانسی. حالا همون
            // ترتیبی که موتور بر اساسِ ارتباط داده حفظ می‌شه.
            var seenProducts = new HashSet<int>();
            foreach (var d in products)
            {
                int pid = Int(d, "productId");
                if (pid <= 0 || !seenProducts.Add(pid)) continue;

                result.Add(new MainSearch
                {
                    Id = pid,
                    Name = Str(d, "displayTitle"),
                    Title = BuildProductTitle(d),
                    PageAddress = Str(d, "pageAddress"),
                    TypeId = 4,
                    AttachementId = Str(d, "cover")
                });

                if (seenProducts.Count >= ProductLimit) break;
            }

            return result;
        }

        /// <summary>
        /// شناسه‌ی محصولات برای صفحه‌ی کاملِ /search، به ترتیبِ ارتباط.
        /// null یعنی سرویس در دسترس نیست و باید مسیرِ قدیمی اجرا بشه.
        /// </summary>
        public static async Task<List<int>> ProductIdsAsync(string rawKeyword, int limit)
        {
            if (!MeiliClient.Configured) return null;

            string q = PersianText.Normalize(rawKeyword);
            if (string.IsNullOrEmpty(q)) return new List<int>();

            // اول فقط محصولاتی که همه‌ی کلمات را دارند. با "last" یک عنوانِ کامل (که کشوی جستجو
            // با کلیک روی محصول می‌فرستد) هزاران نتیجه می‌آورد، چون Meili کلمه‌به‌کلمه حذف می‌کند تا
            // به کلمه‌ی عامی مثلِ «مدل» برسد، و صفحه بعد بر اساسِ جدیدترین مرتب می‌شود؛ پس خودِ
            // محصول زیرِ بقیه گم می‌شد. "last" فقط وقتی که تطبیقِ کامل هیچ نتیجه‌ای نداشت.
            var hits = await MeiliClient.SearchAsync(q, "typeId = 3", limit, "all");
            if (hits == null) return null;
            if (hits.Count == 0)
                hits = await MeiliClient.SearchAsync(q, "typeId = 3", limit) ?? hits;

            if (hits.Count == 0)
            {
                SearchIndexer.EnsureVocabularyLoaded();
                string corrected = SearchVocabulary.Correct(q);
                if (!string.IsNullOrEmpty(corrected))
                    hits = await MeiliClient.SearchAsync(corrected, "typeId = 3", limit) ?? hits;
            }

            var ids = new List<int>();
            var seen = new HashSet<int>();
            foreach (var d in hits)
            {
                int pid = Int(d, "productId");
                if (pid > 0 && seen.Add(pid)) ids.Add(pid);
            }
            return ids;
        }

        /// <summary>
        /// عنوانِ محصول عیناً مثلِ cte4 نسخه‌ی قدیمی:
        ///     ElementTitle [+ " مدل X"] [+ " سایز Y"] [+ " Z"]
        /// </summary>
        private static string BuildProductTitle(JObject d)
        {
            string t = Str(d, "displayTitle");
            string model = Str(d, "displayModel");
            string size = Str(d, "displaySize");
            string color = Str(d, "displayColor");

            if (!string.IsNullOrEmpty(model)) t += " مدل " + model;
            if (!string.IsNullOrEmpty(size)) t += " سایز " + size;
            if (!string.IsNullOrEmpty(color)) t += " " + color;
            return t;
        }

        private static string Str(JObject d, string key)
        {
            var v = d[key];
            return v == null || v.Type == JTokenType.Null ? "" : v.ToString();
        }

        private static int Int(JObject d, string key)
        {
            var v = d[key];
            if (v == null || v.Type == JTokenType.Null) return 0;
            int n;
            return int.TryParse(v.ToString(), out n) ? n : 0;
        }
    }
}
