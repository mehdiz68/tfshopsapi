using Domain;
using Domain.ViewModels;
using Stimulsoft.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TfShop.ViewModels.Api.HeaderVM
{

    public class AmazingOffersVM
    {
       
        public int Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public string BackColor { get; set; }
        public SliderTimerPosition timerPosition { get; set; }
        public string Cover { get; set; }
        public string EndDate { get; set; }
        public bool finish { get; set; }
        public int Value { get; set; }
        public IEnumerable<ProductItemVM> ProductItems { get; set; }
    }

    public class NewCategoryProductVN
    {
        public int id { get; set; }
        public int CatId { get; set; }
        public List<int> ProductIds { get; set; }
        public string CatName { get; set; }
        public string CatPageAddress { get; set; }
        public string CatDescription { get; set; }
        public string CatImage { get; set; }
        public IEnumerable<ProductItemVM> ProductItems { get; set; }
    }

    public class ProductItemVM
    {
        public int Id { get; set; }
        public int PrId { get; set; }
        public string title { get; set; }
        public string shortTitle { get; set; }
        public string pageAdress { get; set; }
        public long price { get; set; }
        public long finalPrice { get; set; }
        public string cover { get; set; }
        public int priceCount { get; set; }
        public int rateAvg { get; set; }
        public int rateCount { get; set; }
        public List<string> colors{ get; set; }
        public bool hasoff { get; set; }
        public long offValue { get; set; }
        public short offType { get; set; }
        public string icon { get; set; }
        public int? productStateId { get; set; }
        public string productStateTitle { get; set; }
    }
   
}