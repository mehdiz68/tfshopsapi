using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SmartAdImage : Object
    {
        #region Ctor
        public SmartAdImage()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmartAdImage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.SmartAd).WithMany(Current => Current.SmartAdImages).HasForeignKey(Current => Current.AdId);
                HasRequired(Current => Current.attachment).WithMany(Current => Current.SmartAdImages).HasForeignKey(Current => Current.Cover);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "تبلیغ")]
        public int AdId { get; set; }
        public SmartAd SmartAd { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "کاور(تصویر)")]
        public Guid Cover { get; set; }
        public attachment attachment { get; set; }




        [Display(Name = "تعداد بازدید")]
        public int visits { get; set; }

        [Display(Name = "اندازه")]
        public SmartAdImageSize SmartAdImageSize { get; set; }


        public ICollection<SmartAdLog> SmartAdLogs { get; set; }

        public ICollection<SmartBannerTempLog> SmartBannerTempLogs { get; set; }
        #endregion
    }

    public enum SmartAdImageSize
    {
        [Display(Name = "مستطیل عمودی - 120X600")]
        S120X600,
        [Display(Name = "مستطیل افقی - 300X100")]
        S300X100,
        [Display(Name = "مستطیل افقی - 300X200")]
        S300X200,
        [Display(Name = "مستطیل عمودی - 400X600")]
        S400X600,
        [Display(Name = "مستطیل افقی - 468X60")]
        S468X60,
        [Display(Name = "مستطیل افقی - 728X90")]
        S728X90,
        [Display(Name = "مستطیل افقی - 970X250")]
        S970X250

    }

}
