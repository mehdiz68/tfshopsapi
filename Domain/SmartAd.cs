using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class SmartAd : Object
    {
        #region Ctor
        public SmartAd()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmartAd>
        {
            public Configuration()
            {
                HasRequired(Current => Current.SmartCampaign).WithMany(Current => Current.SmartAds).HasForeignKey(Current => Current.SmartCampaignId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }


        [Display(Name = "کمپین هوشمند")]
        public SmartCampaign SmartCampaign { get; set; }
        public int SmartCampaignId { get; set; }

        [Required(ErrorMessage = "اجباری")]
        //[RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "لینک درست نیست")]
        [Display(Name = "لینک")]
        public string Link { get; set; }


        [Display(Name = "عنوان")]
        [MaxLength(70, ErrorMessage = "حداکثر طول کارکتر ، 70")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        public  ICollection<SmartAdLog> SmartAdLogs { get; set; }
        public ICollection<SmartAdImage> SmartAdImages { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int visits { get; set; }

        #endregion
    }
}
