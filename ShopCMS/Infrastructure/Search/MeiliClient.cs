using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TfShop.Infrastructure.Search
{
    /// <summary>
    /// کلاینتِ سبکِ Meilisearch روی HttpClient خام.
    ///
    /// عمداً از پکیجِ رسمیِ NuGet استفاده نشده: این پروژه .NET Framework 4.7.2 ئه و SDK رسمی
    /// وابستگی‌های جدیدتری می‌خواد که با بقیه‌ی پکیج‌های همین سولوشن تداخل دارن. کلِ چیزی که
    /// لازم داریم چهار تا فراخوانیِ HTTP ساده‌ست.
    ///
    /// نکته‌ی مهمِ طراحی: هیچ متدی استثنا بیرون نمی‌ده. اگه Meilisearch بالا نباشه، null یا
    /// false برمی‌گرده و صدازننده به مسیرِ قدیمیِ SQL برمی‌گرده - یعنی جستجوی سایت هیچ‌وقت
    /// به‌خاطرِ خوابیدنِ این سرویس کامل از کار نمی‌افته.
    /// </summary>
    public static class MeiliClient
    {
        public const string IndexUid = "tfshop";

        private static readonly HttpClient Http = CreateClient();

        private static HttpClient CreateClient()
        {
            var c = new HttpClient();
            // جستجو باید سریع باشه یا اصلاً نباشه؛ معطل‌نگه‌داشتنِ کاربر بدتر از fallback ئه.
            c.Timeout = TimeSpan.FromSeconds(5);
            return c;
        }

        public static string BaseUrl
        {
            get
            {
                string v = ConfigurationManager.AppSettings["MeiliUrl"];
                return string.IsNullOrWhiteSpace(v) ? "" : v.TrimEnd('/');
            }
        }

        private static string ApiKey
        {
            get { return ConfigurationManager.AppSettings["MeiliKey"] ?? ""; }
        }

        /// <summary>وقتی آدرس تنظیم نشده، کلِ مسیرِ Meili نادیده گرفته می‌شه.</summary>
        public static bool Configured
        {
            get { return !string.IsNullOrEmpty(BaseUrl); }
        }

        private static HttpRequestMessage Build(HttpMethod method, string path, string jsonBody)
        {
            var req = new HttpRequestMessage(method, BaseUrl + path);
            if (!string.IsNullOrEmpty(ApiKey))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            if (jsonBody != null)
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            return req;
        }

        private static async Task<string> SendAsync(HttpMethod method, string path, string jsonBody)
        {
            if (!Configured) return null;
            try
            {
                using (var req = Build(method, path, jsonBody))
                using (var res = await Http.SendAsync(req))
                {
                    string body = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        Log("http " + (int)res.StatusCode + " " + path + " | " + Trim(body));
                        return null;
                    }
                    return body;
                }
            }
            catch (Exception ex)
            {
                Log(path + " | " + ex.Message);
                return null;
            }
        }

        private static string Trim(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length > 500 ? s.Substring(0, 500) : s;
        }

        private static void Log(string message)
        {
            try
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "meili", false, 500, message, DateTime.Now,
                    "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }
            catch { }
        }

        // ---------------------------------------------------------------
        // جستجو
        // ---------------------------------------------------------------

        /// <summary>
        /// یک کوئری روی ایندکس. فیلتر اختیاریه (مثلاً "typeId = 3").
        /// خروجی: لیستِ سندها به‌صورت JObject، یا null اگه سرویس در دسترس نباشه.
        /// </summary>
        public static async Task<List<JObject>> SearchAsync(string query, string filter, int limit,
            string matchingStrategy = "last")
        {
            var payload = new Dictionary<string, object>
            {
                { "q", query },
                { "limit", limit },
                // matchingStrategy=last یعنی اگه همه‌ی کلمات با هم نتیجه ندادن، Meili از آخرین
                // کلمه شروع به حذف می‌کنه. برای جستجوی فروشگاهی که کاربر عبارتِ بلند تایپ
                // می‌کنه («کفش ورزشی مردانه نایک سایز ۴۲») این دقیقاً رفتارِ درستیه.
                // صفحه‌ی کاملِ نتایج اول "all" را امتحان می‌کند - رجوع کنید به SearchService.ProductIdsAsync.
                { "matchingStrategy", matchingStrategy }
            };
            if (!string.IsNullOrEmpty(filter)) payload["filter"] = filter;

            string body = await SendAsync(HttpMethod.Post, "/indexes/" + IndexUid + "/search",
                JsonConvert.SerializeObject(payload));
            if (body == null) return null;

            try
            {
                var hits = JObject.Parse(body)["hits"] as JArray;
                var list = new List<JObject>();
                if (hits != null)
                    foreach (var h in hits)
                        if (h is JObject) list.Add((JObject)h);
                return list;
            }
            catch (Exception ex)
            {
                Log("parse | " + ex.Message);
                return null;
            }
        }

        // ---------------------------------------------------------------
        // ایندکس‌گذاری
        // ---------------------------------------------------------------

        public static async Task<bool> EnsureIndexAsync()
        {
            // ساختِ ایندکس اگه نبود. اگه از قبل باشه Meili خطای index_already_exists می‌ده که
            // بی‌ضرره و همون مسیرِ لاگ می‌گیره.
            await SendAsync(HttpMethod.Post, "/indexes",
                JsonConvert.SerializeObject(new { uid = IndexUid, primaryKey = "id" }));
            return Configured;
        }

        /// <summary>
        /// تنظیماتِ ایندکس. ترتیبِ searchableAttributes مهمه: Meili هرچه فیلد بالاتر باشه
        /// وزنِ بیشتری بهش می‌ده، پس تطبیق در عنوان از تطبیق در توضیحات مهم‌تر حساب می‌شه.
        /// </summary>
        public static async Task<bool> ApplySettingsAsync(Dictionary<string, string[]> synonyms)
        {
            var settings = new Dictionary<string, object>
            {
                { "searchableAttributes", new[] { "title", "titleCompact", "keywords", "model", "color", "size", "garanty", "categoryName", "brandName", "abstract" } },
                { "displayedAttributes", new[] { "*" } },
                { "filterableAttributes", new[] { "typeId", "catId" } },
                { "sortableAttributes", new[] { "popularity" } },
                {
                    "rankingRules", new[]
                    {
                        "words",       // چند کلمه از کوئری تطبیق خورده
                        "typo",        // کم‌غلط‌ترها جلوتر
                        "proximity",   // کلمات نزدیک‌تر به هم
                        "attribute",   // تطبیق در فیلدِ مهم‌تر
                        "exactness",   // تطبیقِ دقیق بهتر از پیشوندی
                        "popularity:desc"
                    }
                },
                {
                    // پیش‌فرضِ Meili برای کلماتِ کوتاه غلط‌یابی نمی‌کنه. کلماتِ فارسی معمولاً
                    // کوتاه‌ترن، پس آستانه پایین‌تر آورده شده تا «سامسونگ»/«سامسنگ» هم بگیره.
                    "typoTolerance", new Dictionary<string, object>
                    {
                        { "enabled", true },
                        { "minWordSizeForTypos", new Dictionary<string, int> { { "oneTypo", 4 }, { "twoTypos", 8 } } }
                    }
                }
            };

            if (synonyms != null && synonyms.Count > 0)
                settings["synonyms"] = synonyms;

            return await SendAsync(new HttpMethod("PATCH"), "/indexes/" + IndexUid + "/settings",
                JsonConvert.SerializeObject(settings)) != null;
        }

        /// <summary>افزودن/به‌روزرسانیِ دسته‌ای سندها.</summary>
        public static async Task<bool> AddDocumentsAsync(IEnumerable<object> documents)
        {
            return await SendAsync(HttpMethod.Post, "/indexes/" + IndexUid + "/documents",
                JsonConvert.SerializeObject(documents)) != null;
        }

        public static async Task<bool> ClearDocumentsAsync()
        {
            return await SendAsync(HttpMethod.Delete, "/indexes/" + IndexUid + "/documents", null) != null;
        }

        /// <summary>برای صفحه‌ی سلامت/تست - آیا سرویس بالاست؟</summary>
        public static async Task<bool> HealthyAsync()
        {
            return await SendAsync(HttpMethod.Get, "/health", null) != null;
        }
    }
}
