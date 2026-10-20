using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TfShop.Infrastructure.AuthJTW;

namespace TfShop.Controllers
{
    /// <summary>
    /// اعلانِ مرورگر (Web Push) - سمتِ سایت: عضویت، لغو، رهگیریِ نمایش و کلیک، آمارِ پنجره‌ی دعوت.
    /// فقط route‌هایِ Next.js (app/api/push/*) این‌ها را صدا می‌زنند؛ IP و شهرِ تقریبی را هم همان‌جا
    /// (که IPِ واقعیِ کاربر را از سرآیندهایِ CDN می‌بیند) می‌سازند و این‌جا فقط ذخیره می‌شود.
    ///
    /// جدول‌ها بیرون از مدلِ EF هستند (Sql\push-notifications.sql) - رجوع کنید به توضیحِ همان فایل.
    /// ارسال در این پروژه نیست؛ پنل و TfScheduler با CoreLib\Infrastructure\Push می‌فرستند.
    /// </summary>
    public class PushController : Controller
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["TfShopConnectionString"].ConnectionString; }
        }

        /// <summary>کلیدِ عمومیِ VAPID برایِ pushManager.subscribe در مرورگر.</summary>
        [HttpGet]
        public JsonResult Key()
        {
            string key = ConfigurationManager.AppSettings["PushVapidPublicKey"] ?? "";
            return Json(new { key, statusCode = string.IsNullOrEmpty(key) ? 503 : 200 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ثبت یا به‌روزرسانیِ عضویت. هر بار که سایت با اجازه‌ی داده‌شده باز می‌شود هم (حداکثر روزی یک بار)
        /// صدا زده می‌شود تا LastSeenAt، IP و اتصال به حسابِ کاربر تازه بماند.
        /// </summary>
        [HttpPost]
        public JsonResult Subscribe(string endpoint, string p256dh, string auth, string ip, string province, string city, string userAgent)
        {
            if (!IsValidEndpoint(endpoint) || string.IsNullOrWhiteSpace(p256dh) || string.IsNullOrWhiteSpace(auth)
                || p256dh.Length > 200 || auth.Length > 100)
                return Json(new { statusCode = 400 });

            try
            {
                string userId = CurrentUserId();
                byte geoSource = string.IsNullOrWhiteSpace(province) && string.IsNullOrWhiteSpace(city) ? (byte)0 : (byte)1;

                // کاربرِ واردشده: شهرِ ثبت‌شده در حسابش دقیق‌تر از IP است (IPِ اینترنتِ همراه معمولاً «تهران» است)
                if (userId != null)
                {
                    using (var uow = new UnitOfWork.UnitOfWorkClass())
                    {
                        // selectorِ Get در حافظه اجرا می‌شود (Func، نه Expression)، پس بدونِ include
                        // CityEntity نال است
                        var u = uow.UserRepository.Get(x => new { City = x.CityEntity.Name, Province = x.CityEntity.Province.Name },
                            x => x.Id == userId && x.CityId != null, null, "CityEntity.Province").FirstOrDefault();
                        if (u != null && !string.IsNullOrWhiteSpace(u.City))
                        {
                            city = u.City;
                            province = u.Province;
                            geoSource = 2;
                        }
                    }
                }

                string ua = Cut(userAgent, 400) ?? "";
                const string sql = @"
MERGE PushSubscribers WITH (HOLDLOCK) AS t
USING (SELECT @hash AS EndpointHash) AS s ON t.EndpointHash = s.EndpointHash
WHEN MATCHED THEN UPDATE SET
    P256dh = @p256dh, Auth = @auth,
    UserId = ISNULL(@userId, t.UserId),
    IsActive = 1, UnsubscribedAt = NULL, FailCount = 0, LastSeenAt = GETDATE(),
    Ip = ISNULL(@ip, t.Ip),
    -- شهرِ آدرسِ کاربر (2) با شهرِ IP (1) جایگزین نمی‌شود
    Province = CASE WHEN @geo > 0 AND @geo >= t.GeoSource THEN @province ELSE t.Province END,
    City     = CASE WHEN @geo > 0 AND @geo >= t.GeoSource THEN @city ELSE t.City END,
    GeoSource = CASE WHEN @geo >= t.GeoSource THEN @geo ELSE t.GeoSource END,
    DeviceType = @device, Browser = @browser, Os = @os, UserAgent = @ua
WHEN NOT MATCHED THEN INSERT
    (Endpoint, EndpointHash, P256dh, Auth, UserId, Ip, Province, City, GeoSource, DeviceType, Browser, Os, UserAgent)
    VALUES (@endpoint, @hash, @p256dh, @auth, @userId, @ip, @province, @city, @geo, @device, @browser, @os, @ua)
OUTPUT inserted.Id, $action;";

                using (var cn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@endpoint", endpoint);
                    cmd.Parameters.AddWithValue("@hash", Sha256(endpoint));
                    cmd.Parameters.AddWithValue("@p256dh", p256dh);
                    cmd.Parameters.AddWithValue("@auth", auth);
                    cmd.Parameters.AddWithValue("@userId", (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ip", (object)Cut(ip, 45) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@province", (object)NullIfEmpty(Cut(province, 50)) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@city", (object)NullIfEmpty(Cut(city, 80)) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@geo", geoSource);
                    cmd.Parameters.AddWithValue("@device", DeviceType(ua));
                    cmd.Parameters.AddWithValue("@browser", (object)Browser(ua) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@os", (object)Os(ua) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ua", ua);
                    cn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        return Json(new { statusCode = 200, id = r.GetInt32(0), created = r.GetString(1) == "INSERT" });
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "push", "Subscribe", false, 500, Cut(ex.Message, 400), DateTime.Now,
                    "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                // پیامِ خطا فقط به route‌ِ Next می‌رسد (سمتِ سرور)، نه به مرورگرِ کاربر
                return Json(new { statusCode = 500, error = Cut(ex.Message, 300) });
            }
        }

        [HttpPost]
        public JsonResult Unsubscribe(string endpoint)
        {
            if (!IsValidEndpoint(endpoint))
                return Json(new { statusCode = 400 });
            Execute("UPDATE PushSubscribers SET IsActive = 0, UnsubscribedAt = GETDATE() WHERE EndpointHash = @hash AND IsActive = 1",
                new SqlParameter("@hash", Sha256(endpoint)));
            return Json(new { statusCode = 200 });
        }

        /// <summary>
        /// گزارشِ service worker: e = shown | click، a = 0 خودِ پیام، 1 و 2 دکمه‌ها.
        /// t شناسه‌ی تصادفیِ هر ارسال است (PushDeliveries.Token)، پس بدونِ آن نمی‌شود آمار را دستکاری کرد.
        /// هر رویداد فقط یک بار شمرده می‌شود.
        /// </summary>
        [HttpPost]
        public JsonResult Track(string t, string e, int? a)
        {
            Guid token;
            if (!Guid.TryParseExact(t ?? "", "N", out token))
                return Json(new { statusCode = 400 });

            if (e == "shown")
            {
                Execute(@"
UPDATE d SET ShownAt = GETDATE() FROM PushDeliveries d WHERE d.Token = @t AND d.ShownAt IS NULL;
IF @@ROWCOUNT = 1 UPDATE PushCampaigns SET ShownCount = ShownCount + 1
    WHERE Id = (SELECT CampaignId FROM PushDeliveries WHERE Token = @t);",
                    new SqlParameter("@t", token));
            }
            else if (e == "click")
            {
                byte action = (byte)(a == 1 || a == 2 ? a.Value : 0);
                // کلیک یعنی حتماً نمایش داده شده - اگر گزارشِ نمایش نرسیده بود (مثلاً شبکه قطع بود) همین‌جا جبران می‌شود
                Execute(@"
DECLARE @wasShown BIT = (SELECT CASE WHEN ShownAt IS NULL THEN 0 ELSE 1 END FROM PushDeliveries WHERE Token = @t);
UPDATE d SET ClickedAt = GETDATE(), ClickAction = @a, ShownAt = ISNULL(ShownAt, GETDATE())
FROM PushDeliveries d WHERE d.Token = @t AND d.ClickedAt IS NULL;
IF @@ROWCOUNT = 1 UPDATE PushCampaigns SET
    ClickCount = ClickCount + 1,
    Click1Count = Click1Count + CASE WHEN @a = 1 THEN 1 ELSE 0 END,
    Click2Count = Click2Count + CASE WHEN @a = 2 THEN 1 ELSE 0 END,
    ShownCount = ShownCount + CASE WHEN @wasShown = 0 THEN 1 ELSE 0 END
    WHERE Id = (SELECT CampaignId FROM PushDeliveries WHERE Token = @t);",
                    new SqlParameter("@t", token), new SqlParameter("@a", action));
            }
            else
                return Json(new { statusCode = 400 });

            return Json(new { statusCode = 200 });
        }

        /// <summary>آمارِ روزانه‌ی پنجره‌ی دعوت: shown، accepted، dismissed، granted، denied.</summary>
        [HttpPost]
        public JsonResult Prompt(string e)
        {
            string column;
            switch (e)
            {
                case "shown": column = "Shown"; break;
                case "accepted": column = "Accepted"; break;
                case "dismissed": column = "Dismissed"; break;
                case "granted": column = "Granted"; break;
                case "denied": column = "Denied"; break;
                default: return Json(new { statusCode = 400 });
            }
            Execute(@"
UPDATE PushPromptDaily SET " + column + " = " + column + @" + 1 WHERE Day = CAST(GETDATE() AS DATE);
IF @@ROWCOUNT = 0
BEGIN TRY
    INSERT INTO PushPromptDaily (Day, " + column + @") VALUES (CAST(GETDATE() AS DATE), 1);
END TRY
BEGIN CATCH
    UPDATE PushPromptDaily SET " + column + " = " + column + @" + 1 WHERE Day = CAST(GETDATE() AS DATE);
END CATCH");
            return Json(new { statusCode = 200 });
        }

        // ---------------------------------------------------------------- ابزار

        private string CurrentUserId()
        {
            string token = Request.Headers["AuthToken"];
            if (string.IsNullOrWhiteSpace(token)) return null;
            string userId = null;
            try { userId = Authentication.ValidateToken(token); }
            catch { }
            if (userId == null)
                LogRejectedToken(token);
            return userId;
        }

        /// <summary>
        /// تشخیصی (موقت): توکنی که رسید ولی ValidateToken ردش کرد - چرا؟ خودِ توکن ثبت نمی‌شود، فقط
        /// ساختار، انقضا و claimها. (عضویت‌هایِ کاربرانِ واردشده بی‌صدا «مهمان» ثبت می‌شد.)
        /// </summary>
        private static void LogRejectedToken(string token)
        {
            string info;
            try
            {
                var parts = token.Split('.');
                info = "len=" + token.Length + " parts=" + parts.Length;
                if (parts.Length == 3)
                {
                    string b64 = parts[1].Replace('-', '+').Replace('_', '/');
                    b64 = b64.PadRight(b64.Length + (4 - b64.Length % 4) % 4, '=');
                    var payload = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
                    long? exp = payload["exp"] == null ? (long?)null : (long)payload["exp"];
                    long now = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                    info += " exp=" + (exp.HasValue ? (exp.Value - now) + "s" : "none")
                        + " jku=" + (payload["jku"] != null) + " kid=" + (payload["kid"] != null)
                        + " jwtKeyConfigured=" + !string.IsNullOrEmpty(ConfigurationManager.AppSettings["config:JwtKey"]);
                }
            }
            catch (Exception ex)
            {
                info = "unparseable: " + ex.Message;
            }
            try
            {
                Infrastructure.EventLog.Logger.Add(5, "push", "Subscribe-token-rejected", false, 401, Cut(info, 400), DateTime.Now,
                    "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }
            catch { }
        }

        /// <summary>فقط آدرس‌هایِ سرویس‌هایِ pushِ شناخته‌شده - وگرنه سرورِ ما می‌شد ابزارِ ارسالِ درخواست به هر جا.</summary>
        private static bool IsValidEndpoint(string endpoint)
        {
            Uri uri;
            if (string.IsNullOrWhiteSpace(endpoint) || endpoint.Length > 1000 ||
                !Uri.TryCreate(endpoint, UriKind.Absolute, out uri) || uri.Scheme != "https")
                return false;
            string host = uri.Host.ToLowerInvariant();
            return host == "fcm.googleapis.com"
                || host.EndsWith(".push.services.mozilla.com") || host == "updates.push.services.mozilla.com"
                || host.EndsWith(".push.apple.com")
                || host.EndsWith(".notify.windows.com");
        }

        private static byte DeviceType(string ua)
        {
            if (string.IsNullOrEmpty(ua)) return 0;
            if (Regex.IsMatch(ua, "iPad|Tablet|SM-T|Tab ", RegexOptions.IgnoreCase)) return 2;
            if (Regex.IsMatch(ua, "Mobi|Android|iPhone|iPod", RegexOptions.IgnoreCase)) return 1;
            return 3;
        }

        private static string Browser(string ua)
        {
            if (string.IsNullOrEmpty(ua)) return null;
            if (ua.Contains("Edg/") || ua.Contains("EdgA/")) return "Edge";
            if (ua.Contains("OPR/") || ua.Contains("Opera")) return "Opera";
            if (ua.Contains("SamsungBrowser")) return "Samsung";
            if (ua.Contains("Firefox/") || ua.Contains("FxiOS")) return "Firefox";
            if (ua.Contains("Chrome/") || ua.Contains("CriOS")) return "Chrome";
            if (ua.Contains("Safari/")) return "Safari";
            return "Other";
        }

        private static string Os(string ua)
        {
            if (string.IsNullOrEmpty(ua)) return null;
            if (ua.Contains("Android")) return "Android";
            if (Regex.IsMatch(ua, "iPhone|iPad|iPod")) return "iOS";
            if (ua.Contains("Windows")) return "Windows";
            if (ua.Contains("Mac OS X")) return "macOS";
            if (ua.Contains("Linux")) return "Linux";
            return "Other";
        }

        private static byte[] Sha256(string s)
        {
            using (var sha = SHA256.Create())
                return sha.ComputeHash(Encoding.UTF8.GetBytes(s));
        }

        private static void Execute(string sql, params SqlParameter[] parameters)
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parameters);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max);
        }

        private static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }
    }
}
