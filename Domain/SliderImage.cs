using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SliderImage : Object
    {
        #region Ctor
        public SliderImage()
        {

        }
        public SliderImage(string title,string link,Guid cover,int displayorder,int sliderid)
        {
            Title = title;
            Link = link;
            Cover = cover;
            DisplaySort = displayorder;
            SliderId = sliderid;
        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SliderImage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.attachment).WithMany(Current => Current.SliderImages).HasForeignKey(Current => Current.Cover);
                HasRequired(Current => Current.Slider).WithMany(Current => Current.SliderImages).HasForeignKey(Current => Current.SliderId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Display(Name = "لینک")]
        //[MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        public string Link { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "کاور(تصویر)")]
        public Guid Cover { get; set; }
        public  attachment attachment { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Display(Name = "تایمر")]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "موقعیت مکانی")]
        public SliderTimerPosition? SliderTimerPosition { get; set; }

        [Display(Name = "اندازه")]
        public SliderTimerWidth? SliderTimerWidth { get; set; }

        [Display(Name = "اسلایدر")]
        public int SliderId{ get; set; }
        public  Slider Slider { get; set; }

        [Display(Name = "رنگ")]
        public string color { get; set; }


        #endregion
    }
    public enum SliderTimerPosition
    {

        [Display(Name = "بالا راست")]
        TopRight,
        [Display(Name = "بالا وسط")]
        TopCenter,
        [Display(Name = "بالا چپ")]
        TopLeft,
        [Display(Name = "وسط راست")]
        CenterRight,
        [Display(Name = "وسط وسط")]
        CenterCenter,
        [Display(Name = "وسط چپ")]
        CenterLeft,
        [Display(Name = "پایین راست")]
        BottomRight,
        [Display(Name = "پایین وسط")]
        BottomCenter,
        [Display(Name = "پایین چپ")]
        BottomLeft,
    }
    public enum SliderTimerWidth
    {

        [Display(Name = "کامل")]
        Full,
        [Display(Name = "75 درصد")]
        Percent75,
        [Display(Name = "نیم")]
        Half,
        [Display(Name = "25 درصد")]
        Percent25
    }
}
