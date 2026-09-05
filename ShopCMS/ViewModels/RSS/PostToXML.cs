using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TfShop.ViewModels.RSS
{
    public class PostToXML
    {
        public int PostId { get; set; }

        public string title { get; set; }

        public string link { get; set; }

        public string description { get; set; }

        public Nullable<DateTime> pubDate { get; set; }
    }
}