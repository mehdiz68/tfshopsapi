using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace TfShop.Infrastructure.Newsletter
{
    /// <summary>
    /// توکنِ تاییدِ ایمیلِ خبرنامه.
    ///
    /// سایتِ قدیم کد را در Session نگه می‌داشت (Session["VerifyNewsletterCode"]) و موقعِ کلیک
    /// همان را مقایسه می‌کرد - یعنی لینک فقط در همان مرورگر و تا وقتی نشست زنده بود کار می‌کرد.
    /// باز کردنِ ایمیل روی گوشی یا بعد از بسته‌شدنِ مرورگر همیشه شکست می‌خورد.
    ///
    /// این API نشست ندارد و جدولِ NewsLetterEmail هم ستونی برای نگه‌داشتنِ کد ندارد، پس توکن
    /// به‌صورت قطعی از خودِ ایمیل ساخته می‌شود: HMAC-SHA256 روی ایمیل با کلیدِ سرور. نه ستونِ
    /// جدیدی لازم دارد، نه نشستی، و روی هر دستگاهی کار می‌کند.
    /// </summary>
    public static class NewsLetterToken
    {
        private static byte[] BuildKey()
        {
            // همان کلیدی که JWT با آن امضا می‌شود؛ با یک پیشوند جدا می‌شود تا توکنِ خبرنامه
            // هیچ‌وقت به‌جای توکنِ دیگری قابلِ استفاده نباشد.
            string key = ConfigurationManager.AppSettings["config:JwtKey"] ?? "";
            return Encoding.UTF8.GetBytes("newsletter|" + key);
        }

        private static string Normalize(string email)
        {
            return (email ?? "").Trim().ToLowerInvariant();
        }

        public static string Create(string email)
        {
            using (var hmac = new HMACSHA256(BuildKey()))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Normalize(email)));

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                // ۳۲ کاراکتر (۱۲۸ بیت) برای لینکِ تاییدِ خبرنامه کافی است و URL را کوتاه نگه می‌دارد
                return sb.ToString().Substring(0, 32);
            }
        }

        /// <summary>مقایسه‌ی زمان‌ثابت، تا توکن با آزمون‌وخطا قابلِ ساختن نباشد.</summary>
        public static bool IsValid(string email, string token)
        {
            string expected = Create(email);

            if (string.IsNullOrEmpty(token) || token.Length != expected.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < expected.Length; i++)
                diff |= expected[i] ^ token[i];

            return diff == 0;
        }
    }
}
