using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using TfShop.Infrastructure.Filter;

namespace TfShop.Infrastructure.AuthJTW
{
    public class Authentication
    {

        public static string GenerateJWTAuthetication(string userName, string role,string Id)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtHeaderParameterNames.Jku, userName),
                new Claim(JwtHeaderParameterNames.Kid,Id),
                new Claim(ClaimTypes.NameIdentifier, userName)
            };


            claims.Add(new Claim(ClaimTypes.Role, role));


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires =
                DateTime.Now.AddDays(
                    Convert.ToDouble(Convert.ToString(ConfigurationManager.AppSettings["config:JwtExpireDays"])));

            var token = new JwtSecurityToken(
                Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }


        // این متد قبلاً کارِ decode/verify رو به JwtSecurityTokenHandler.ValidateToken می‌سپرد، ولی
        // روی سرور دیپلوی‌شده (Microsoft.IdentityModel.Tokens 7.6.1) حتی برای یه توکنِ کاملاً سالم
        // (سه‌تکه، base64url معتبر، بدون هیچ کاراکتر غریبه - با لاگِ دستی تأیید شد) با
        // "IDX12729: Unable to decode the header ... as Base64Url encoded string" رد می‌شد - یعنی
        // یه ناسازگاریِ خودِ نسخه‌ی کتابخونه روی این سرور بود، نه مشکل توکن. برای دور زدنش، اعتبارسنجی
        // امضا/انقضا رو دستی (طبق RFC 7515، همون الگوریتم HS256 که GenerateJWTAuthetication همیشه
        // استفاده می‌کنه) پیاده کردیم - بدون وابستگی به اون متد خاص از کتابخونه.
        public static string ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return null;

                // 1) امضا: HMACSHA256("header.payload", key) باید با تکه‌ی سوم یکی باشه.
                var keyBytes = Encoding.UTF8.GetBytes(
                    Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"]));
                var signingInput = Encoding.UTF8.GetBytes(parts[0] + "." + parts[1]);
                byte[] expectedSig;
                using (var hmac = new HMACSHA256(keyBytes))
                {
                    expectedSig = hmac.ComputeHash(signingInput);
                }
                var actualSig = Base64UrlDecode(parts[2]);
                if (!FixedTimeEquals(expectedSig, actualSig))
                    return null;

                // 2) هدر: فقط الگوریتمی که واقعاً باهاش امضا کردیم (HS256) رو قبول کن - جلوگیری از
                // حمله‌ی "alg confusion" (مثلاً یکی توکنی با alg=none بسازه).
                var header = JObject.Parse(Encoding.UTF8.GetString(Base64UrlDecode(parts[0])));
                if ((string)header["alg"] != "HS256")
                    return null;

                // 3) پیلود: انقضا + claim هایی که caller بهشون نیاز داره (jku/kid).
                var payload = JObject.Parse(Encoding.UTF8.GetString(Base64UrlDecode(parts[1])));
                var exp = payload["exp"]?.Value<long?>();
                if (!exp.HasValue)
                    return null;
                var expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;
                if (expiresAt <= DateTime.UtcNow)
                    return null;

                var jku = (string)payload["jku"];
                var userName = (string)payload["kid"];
                if (string.IsNullOrEmpty(jku) || string.IsNullOrEmpty(userName))
                    return null;

                return userName;
            }
            catch (Exception ex)
            {
                try
                {
                    Infrastructure.EventLog.Logger.Add(5, "auth", "validatetoken", false, 401,
                        ex.GetType().Name + ": " + ex.Message, DateTime.Now,
                        "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                }
                catch { /* لاگ‌نشدنِ خطا نباید خودش باعث خطای جدید بشه */ }

                return null;
            }
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0: break;
                case 2: s += "=="; break;
                case 3: s += "="; break;
                default: throw new FormatException("Invalid base64url string length: " + s.Length);
            }
            return Convert.FromBase64String(s);
        }

        // مقایسه‌ی امضا با زمان ثابت (نه ==/SequenceEqual که زودتر از موعد return می‌کنن) - جلوگیری
        // از timing attack روی مقایسه‌ی امضا.
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }

        // رفرش‌توکن: یه رشته‌ی تصادفیِ کریپتوگرافیک (نه JWT) که فقط توی دیتابیس (هش‌شده، مثل پسورد)
        // نگه‌داری می‌شه و در ازاش، موقع منقضی‌شدنِ access token، بدون نیاز به لاگین دوباره (رمز/OTP)،
        // یه access token جدید صادر می‌شه. رجوع کنید به ApiV1Controller.AuthRefreshToken.
        public static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        // هش SHA-256 برای ذخیره‌سازی امنِ رفرش‌توکن توی دیتابیس (ApplicationUser.RefreshToken) —
        // دقیقاً مثل ذخیره‌ی هش پسورد به‌جای خودِ پسورد، تا لو رفتنِ دیتابیس به معنیِ لو رفتنِ
        // توکن‌های قابل‌استفاده نباشه.
        public static string HashToken(string token)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
                return Convert.ToBase64String(bytes);
            }
        }

    }

}