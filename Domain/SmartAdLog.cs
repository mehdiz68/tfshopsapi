using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SmartAdLog : Object
    {
        #region Ctor
        public SmartAdLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmartAdLog>
        {
            public Configuration()
            {
                //HasRequired(Current => Current.UserActivity).WithMany(Current => Current.SmartAdLogs).HasForeignKey(Current => Current.UserActivityId);
                HasRequired(Current => Current.SmartCampaign).WithMany(Current => Current.SmartAdLogs).HasForeignKey(Current => Current.SmartCampaignId);
                HasOptional(Current => Current.SmartAd).WithMany(Current => Current.SmartAdLogs).HasForeignKey(Current => Current.AdId);
                HasOptional(Current => Current.SmartAdImage).WithMany(Current => Current.SmartAdLogs).HasForeignKey(Current => Current.SmartAdImageId);
                HasOptional(Current => Current.Product).WithMany(Current => Current.SmartAdLogs).HasForeignKey(Current => Current.ProductId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "مخاطب")]
        public int UserActivityId { get; set; }
        public UserActivity UserActivity { get; set; }

        [Display(Name = "ClientIP")]
        public string ClientIP { get; set; }

        [Display(Name = "Browser")]
        public string Browser { get; set; }

        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "کلیک یا نمایش")]
        public bool ViewOrClick { get; set; }


        [Display(Name = "کمپین")]
        public int SmartCampaignId { get; set; }
        public SmartCampaign SmartCampaign { get; set; }


        [Display(Name = "تبلیغ")]
        public int? AdId { get; set; }
        public SmartAd SmartAd { get; set; }


        [Display(Name = "تصویر تبلیغ")]
        public int? SmartAdImageId { get; set; }
        public SmartAdImage SmartAdImage { get; set; }

        [Display(Name = "محصول")]
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        #endregion
    }
}
