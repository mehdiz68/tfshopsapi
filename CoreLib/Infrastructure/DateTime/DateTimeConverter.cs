using System;

namespace CoreLib.Infrastructure.DateTime
{
    public static class DateTimeConverter
    {
        public static System.DateTime ChangeShamsiToMiladiDateTime(string Shamsi)
        {
            System.DateTime miladi = default(System.DateTime);
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            miladi = pc.ToDateTime(Convert.ToInt32(Shamsi.Substring(0, 4)), Convert.ToInt32(Shamsi.Substring(5, 2)), Convert.ToInt32(Shamsi.Substring(8, 2)), Convert.ToInt32(Shamsi.Substring(11, 2)), Convert.ToInt32(Shamsi.Substring(14, 2)), 0, 0, System.Globalization.Calendar.CurrentEra);
            return miladi;
        }

        public static System.DateTime ChangeShamsiToMiladi(string Shamsi)
        {
            // ۱) ارقام فارسی/عربی → انگلیسی + حذف کاراکترهای مخفی RTL
            Shamsi = NormalizeShamsiDate(Shamsi);

            if (string.IsNullOrWhiteSpace(Shamsi) || Shamsi.Length < 10)
                return default(System.DateTime);

            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();

            int year = Convert.ToInt32(Shamsi.Substring(0, 4));
            int month = Convert.ToInt32(Shamsi.Substring(5, 2));
            int day = Convert.ToInt32(Shamsi.Substring(8, 2));

            return pc.ToDateTime(year, month, day, 10, 10, 10, 10,
                                 System.Globalization.Calendar.CurrentEra).Date;
        }

        /// <summary>
        /// ارقام فارسی و عربی را به انگلیسی تبدیل و جداکننده‌ها را یکسان می‌کند.
        /// </summary>
        private static string NormalizeShamsiDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // حذف کاراکترهای مخفی RTL/LTR که با کپی‌پیست وارد رشته می‌شوند
            input = input.Replace("\u200F", "").Replace("\u200E", "").Trim();

            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string[] arabicDigits = { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };

            for (int i = 0; i <= 9; i++)
            {
                input = input.Replace(persianDigits[i], i.ToString());
                input = input.Replace(arabicDigits[i], i.ToString());
            }

            // یکسان‌سازی جداکننده‌ها تا هر فرمتی (۱۳۷۵/۰۵/۱۲ یا ۱۳۷۵-۵-۱۲) کار کند
            input = input.Replace('-', '/').Replace('.', '/').Replace('_', '/');

            return input;
        }

        public static string ChangeMiladiToShamsi(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;

            Year = PC.GetYear(Miladi).ToString();
            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;

            Shamsi = Year + "/" + Month + "/" + Day;

            return Shamsi;
        }


        public static string ChangeMiladiToShamsiTime(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;

            Year = PC.GetHour(Miladi).ToString();
            Month = PC.GetMinute(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetSecond(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;

            Shamsi = Year + ":" + Month + ":" + Day;

            return Shamsi;
        }
        public static string ChangeMiladiToLongShamsi(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;
            var daysofweek = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };

            Year = PC.GetYear(Miladi).ToString();
            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;
            string m = "";
            switch (Month)
            {
                case "01": m = "فروردین"; break;
                case "02": m = "اردیبشت"; break;
                case "03": m = "خرداد"; break;
                case "04": m = "تیر"; break;
                case "05": m = "مرداد"; break;
                case "06": m = "شهریور"; break;
                case "07": m = "مهر"; break;
                case "08": m = "آبان"; break;
                case "09": m = "آذر"; break;
                case "10": m = "دی"; break;
                case "11": m = "بهمن"; break;
                case "12": m = "اسفند"; break;
            }
            Shamsi = daysofweek[(int)PC.GetDayOfWeek(Miladi)] + " " + Day + " " + m + " " + Year;

            return Shamsi;
        }

        public static string ChangeMiladiToLongShamsiOnlyDayName(System.DateTime Miladi)
        {
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            var daysofweek = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };
            return daysofweek[(int)PC.GetDayOfWeek(Miladi)];
        }

        public static string ChangeMiladiToLongShamsiOnlyDay(System.DateTime Miladi)
        {
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();         
            string Day = null;

            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;
           
            return Day;;
        }
        public static string ChangeMiladiToLongShamsiWithoutYear(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;
            var daysofweek = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };

            Year = PC.GetYear(Miladi).ToString();
            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;
            string m = "";
            switch (Month)
            {
                case "01": m = "فروردین"; break;
                case "02": m = "اردیبشت"; break;
                case "03": m = "خرداد"; break;
                case "04": m = "تیر"; break;
                case "05": m = "مرداد"; break;
                case "06": m = "شهریور"; break;
                case "07": m = "مهر"; break;
                case "08": m = "آبان"; break;
                case "09": m = "آذر"; break;
                case "10": m = "دی"; break;
                case "11": m = "بهمن"; break;
                case "12": m = "اسفند"; break;
            }
            Shamsi = daysofweek[(int)PC.GetDayOfWeek(Miladi)] + " " + Day + " " + m;

            return Shamsi;
        }
        public static string ChangeMiladiToLongShamsiOnlyYear(System.DateTime Miladi)
        {
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();

            return PC.GetYear(Miladi).ToString();
          

        }
        public static string ChangeMiladiToLongShamsiOnlyDayOfWeek(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            var daysofweek = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };


            Shamsi = daysofweek[(int)PC.GetDayOfWeek(Miladi)];

            return Shamsi;
        }
        public static string ChangeMiladiToLongShamsiWithoutYearAndWeekDay(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Month = null;
            string Day = null;

            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;
            string m = "";
            switch (Month)
            {
                case "01": m = "فروردین"; break;
                case "02": m = "اردیبشت"; break;
                case "03": m = "خرداد"; break;
                case "04": m = "تیر"; break;
                case "05": m = "مرداد"; break;
                case "06": m = "شهریور"; break;
                case "07": m = "مهر"; break;
                case "08": m = "آبان"; break;
                case "09": m = "آذر"; break;
                case "10": m = "دی"; break;
                case "11": m = "بهمن"; break;
                case "12": m = "اسفند"; break;
            }
            Shamsi = Day + " " + m;

            return Shamsi;
        }


        public static string GetTimeAgo(System.DateTime Miladi)
        {
            var time = System.DateTime.Now - Miladi;
            if (time.TotalMinutes < 60)
                return Math.Floor(time.TotalMinutes) + " دقیقه قبل ";
            else if (time.TotalHours < 24)
                return Math.Floor(time.TotalHours) + " ساعت قبل ";
            else if (time.TotalHours < 48)
                return " دیروز ";
            else if (time.TotalDays < 7)
                return Math.Floor(time.TotalDays) + " روز قبل ";
            else if (time.TotalDays < 30)
                return Math.Floor((time.TotalDays / 7)) + " هفته قبل ";
            else if (time.TotalDays < 365)
                return Math.Floor((time.TotalDays / 12)) + " ماه قبل ";
            else
                return Math.Floor((time.TotalDays / 365)) + " سال قبل ";

        }

        public static string GetDayOfWeek(int day)
        {
            switch (day)
            {
                case 0: return "شنبه";
                case 1: return "یکشنبه";
                case 2: return "دوشنبه";
                case 3: return "سه شنبه";
                case 4: return "چهارشنبه";
                case 5: return "پنجشنبه";
                case 6: return "جمعه";
                default: return "نامشخص";
            }
        }
    }
}
