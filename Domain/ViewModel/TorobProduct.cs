using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;
using Domain.ViewModel;

namespace Domain.ViewModels
{
    public class TorobProduct
    {

        #region Properties


        public int page_unique { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public long current_price { get; set; }
        public long old_price { get; set; }
        public string availability { get; set; }
        public string category_name { get; set; }
        public string image_link { get; set; }
        public string page_url { get; set; }
        public string short_desc { get; set; }
        //public List<spec> specs { get; set; }
        public string guarantee { get; set; }
        public Dictionary<string,string> spec { get; set; }
        #endregion
    }
    //public class spec
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //}

    public class ZarehbinProduct
    {

        #region Properties


        public int id { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public long current_price { get; set; }
        public long old_price { get; set; }
        public string availability { get; set; }
        public List<string> categories { get; set; }
        public string image_link { get; set; }
        public List<string> image_links { get; set; }
        public string page_url { get; set; }
        public string short_desc { get; set; }
        public string guarantee { get; set; }
        public string registry { get; set; }
        public Dictionary<string, string> spec { get; set; }
        #endregion
    }
}
