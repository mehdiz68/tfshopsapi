using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
namespace Domain.ViewModels
{
    public class PrsProduct
    {
        public PrsProduct()
        {

        }
        public int CampaignId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }

        public string PageAddress { get; set; }
        public string Image { get; set; }
        public int? Count { get; set; }
    }

    public class PrsBanner
    {
        public PrsBanner()
        {

        }
        public int CampaignId { get; set; }
        public int Id { get; set; }
        public int AdId { get; set; }
        public string Title { get; set; }

        public string PageAddress { get; set; }
        public string Image { get; set; }
        public int? Count { get; set; }
        public int CatId { get; set; }
        public SmartAdImageSize SmartAdImageSize { get; set; }
    }
}
