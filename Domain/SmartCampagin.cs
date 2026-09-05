using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class SmartCampaign : Object
    {
        #region Ctor
        public SmartCampaign()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmartCampaign>
        {
            public Configuration()
            {
                HasOptional(Current => Current.UserActivityGroup).WithMany(Current => Current.SmartCampaigns).HasForeignKey(Current => Current.UserActivityGroupId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "اجباری")]
        public string Title { get; set; }

        [Display(Name = "نوع کمپین")]
        public SmartCampaignType SmartCampaignType { get; set; }

        [Display(Name = "گروه مخاطبین")]
        public UserActivityGroup UserActivityGroup { get; set; }
        [Display(Name = "گروه مخاطبین")]
        public int? UserActivityGroupId { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ شروع(اختیاری)")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ پایان(اختیاری)")]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "دستگاه مخاطب")]
        public SmartCampaignDevice SmartCampaignDevice { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش تا زمان خرید؟ (فقط برای ریتارگت)")]
        public bool ShowUntilBuy { get; set; }

        [Display(Name = "UTMcampaign")]
        public string UTMcampaign { get; set; }
        [Display(Name = "UTMsource")]
        public string UTMsource { get; set; }
        [Display(Name = "UTMmedium")]
        public string UTMmedium { get; set; }



        [Display(Name = "تعداد بازدید")]
        public int visits { get; set; }

        public ICollection<SmartAd> SmartAds { get; set; }

        public ICollection<SmartAdLog> SmartAdLogs { get; set; }

        #endregion
    }
    public enum SmartCampaignType
    {
        [Display(Name = "همسان")]
        Hamsan,
        [Display(Name = "بنر")]
        Banner,
        [Display(Name = "ریتارگت محصول")]
        Product,
        [Display(Name = "پوش")]
        Push,
        [Display(Name = "ویدئو")]
        Video


    }
    public enum SmartCampaignDevice
    {
        [Display(Name = "دسکتاپ")]
        Desktop,
        [Display(Name = "موبایل")]
        Mobile,
        [Display(Name = "دسکتاپ/موبایل")]
        All

    }
}
