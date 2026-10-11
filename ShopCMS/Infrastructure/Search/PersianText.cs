using System;
using System.Globalization;
using System.Text;

namespace TfShop.Infrastructure.Search
{
    /// <summary>
    /// نرمال‌سازیِ متنِ فارسی برای جستجو.
    ///
    /// چرا لازمه با اینکه Meilisearch خودش توکنایزر یونیکد داره: Meili حروفِ عربی و فارسی رو
    /// یکی نمی‌کنه. توی دیتای این سایت هر دو شکل قاطیه (هم «کيف» با یای عربی، هم «کیف»)، و
    /// کاربر هم بسته به کیبوردش هر کدوم رو ممکنه تایپ کنه. اگه یکسان‌سازی نشه، نصفِ دیتا برای
    /// نصفِ کاربرها نامرئیه.
    ///
    /// همین تابع باید هم موقعِ ایندکس‌کردن و هم موقعِ کوئری‌زدن اجرا بشه - وگرنه بی‌فایده‌ست.
    /// </summary>
    public static class PersianText
    {
        // نیم‌فاصله و گونه‌های فاصله‌ی صفرعرض. «نیم‌فاصله» و «نیم فاصله» باید یکی بشن.
        private const char Zwnj = '‌';
        private const char Zwj = '‍';
        private const char Lrm = '‎';
        private const char Rlm = '‏';
        private const char Tatweel = 'ـ'; // کشیده: «ســـلام» -> «سلام»

        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var sb = new StringBuilder(input.Length);

            foreach (char ch in input)
            {
                char c = ch;

                switch (c)
                {
                    // ----- یکسان‌سازیِ حروفِ عربی/فارسی -----
                    case 'ي': // ي عربی
                    case 'ى': // ى مقصوره
                    case 'ی': // ی فارسی
                        c = 'ی';
                        break;

                    case 'ك': // ك عربی
                    case 'ک': // ک فارسی
                        c = 'ک';
                        break;

                    case 'ة': // ة
                        c = 'ه'; // ه
                        break;

                    // همزه‌دارها به شکلِ ساده، چون کاربر معمولاً ساده تایپ می‌کنه
                    case 'أ': // أ
                    case 'إ': // إ
                    case 'آ': // آ
                    case 'ٱ': // ٱ
                        c = 'ا'; // ا
                        break;

                    case 'ؤ': // ؤ
                        c = 'و'; // و
                        break;

                    case 'ئ': // ئ
                        c = 'ی';
                        break;

                    // ----- ارقام عربی و فارسی -> لاتین -----
                    // کاربر ممکنه «۱۲۳» تایپ کنه ولی دیتا «123» باشه (یا برعکس).
                    case '۰': case '٠': c = '0'; break;
                    case '۱': case '١': c = '1'; break;
                    case '۲': case '٢': c = '2'; break;
                    case '۳': case '٣': c = '3'; break;
                    case '۴': case '٤': c = '4'; break;
                    case '۵': case '٥': c = '5'; break;
                    case '۶': case '٦': c = '6'; break;
                    case '۷': case '٧': c = '7'; break;
                    case '۸': case '٨': c = '8'; break;
                    case '۹': case '٩': c = '9'; break;

                    // ----- چیزهایی که کلاً حذف می‌شن -----
                    case Tatweel:
                    case Zwj:
                    case Lrm:
                    case Rlm:
                        continue;

                    // نیم‌فاصله به فاصله‌ی معمولی تبدیل می‌شه تا «لپ‌تاپ» و «لپ تاپ» یکی بشن
                    case Zwnj:
                        c = ' ';
                        break;
                }

                // اعرابِ عربی (فتحه/کسره/ضمه/تشدید/سکون و...) بی‌اثرن و حذف می‌شن
                if (c >= 'ً' && c <= 'ٟ') continue;
                if (c == 'ٰ') continue; // الف خنجری

                // نشانه‌گذاری به فاصله تبدیل می‌شه نه حذف، وگرنه «سامسونگ-گلکسی» می‌شد یک توکنِ
                // به‌هم‌چسبیده و با جستجوی «گلکسی» پیدا نمی‌شد.
                if (char.IsPunctuation(c) || char.IsSymbol(c))
                {
                    sb.Append(' ');
                    continue;
                }

                sb.Append(char.ToLowerInvariant(c));
            }

            // فشرده‌کردنِ فاصله‌های پشتِ‌سرهم
            var outSb = new StringBuilder(sb.Length);
            bool lastWasSpace = true;
            foreach (char c in sb.ToString())
            {
                bool isSpace = char.IsWhiteSpace(c);
                if (isSpace)
                {
                    if (!lastWasSpace) outSb.Append(' ');
                    lastWasSpace = true;
                }
                else
                {
                    outSb.Append(c);
                    lastWasSpace = false;
                }
            }

            return outSb.ToString().Trim();
        }

        /// <summary>
        /// نسخه‌ی «بدونِ فاصله» برای تطبیقِ چسبیده/جدا. مثلاً کاربر «سامسونگگلکسی» یا
        /// «لپتاپ» تایپ می‌کنه؛ این شکل هم ایندکس می‌شه تا پیدا بشه.
        /// </summary>
        public static string Compact(string input)
        {
            string n = Normalize(input);
            return string.IsNullOrEmpty(n) ? "" : n.Replace(" ", "");
        }
    }
}
