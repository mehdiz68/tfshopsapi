using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
namespace Domain.ViewModels
{
    public class SmartAdViewModel
    {
        public SmartAdViewModel()
        {

        }
        public int CampaignId { get; set; }
        public string CampaignTitle { get; set; }
        public int CampaignAdCount { get; set; }
        public SmartCampaignType SmartCampaignType { get; set; }
        public IEnumerable<SmartAdItem> SmartAdItems { get; set; }

    }

    public class SmartAdItem
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Descr { get; set; }
        public bool IsActive { get; set; }
        public int Visits { get; set; }
        public int Clicks { get; set; }
        public int TodayClicks { get; set; }
        public string Image { get; set; }
    }

}
