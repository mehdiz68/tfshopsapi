using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CoreLib
{
    public static class HtmlExtensions
    {

        public static string PersianToEnglish(this string persianStr)
        {
            if (!String.IsNullOrEmpty(persianStr))
            {
                Dictionary<string, string> LettersDictionary = new Dictionary<string, string>
                {
                    ["۰"] = "0",
                    ["۱"] = "1",
                    ["۲"] = "2",
                    ["۳"] = "3",
                    ["۴"] = "4",
                    ["۵"] = "5",
                    ["۶"] = "6",
                    ["۷"] = "7",
                    ["۸"] = "8",
                    ["۹"] = "9"
                };
                return LettersDictionary.Aggregate(persianStr, (current, item) =>
                             current.Replace(item.Key, item.Value));
            }
            else
                return "";
        }
        public static string EnumDisplayNameFor(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            if (displayName != null)
            {
                return   displayName.Name;
            }

            return  item.ToString();
        }
        public static string GetFileName(this string filename)
        {
            if (filename.Contains("/"))
                return filename.Substring(filename.IndexOf("/") + 1);
            else
                return filename;
        }
        public static string GetFolderName(this string filename)
        {
            if (filename.Contains("/"))
                return filename.Substring(0, filename.IndexOf("/"));
            else
                return "-";
        }
    }
}
