using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// زمان‌بندی، فعال/غیرفعال و ویدیویِ یک تصویرِ اسلایدر (یک‌به‌صفر‌یا‌یک با SliderImage، با کلیدِ مشترک).
    /// نبودنِ ردیف یعنی فعال، بدونِ تاریخ و بدونِ ویدیو - یعنی همه‌ی اسلایدهایِ قبلی بدونِ هیچ تغییری مثلِ قبل.
    /// این تاریخ‌ها جدا از «تایمر» (SliderImage.ExpireDate) هستند.
    ///
    /// این کلاس در هر دو کپیِ Domain (Tfshop و TfShopApi) عیناً یکی است؛ هر دو به یک دیتابیس با
    /// AutomaticMigrationDataLossAllowed وصل‌اند، پس اختلافِ مدل یعنی حذفِ جدول/ستون توسطِ اپِ دیگر.
    /// </summary>
    public class SliderImageSetting : Object
    {
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SliderImageSetting>
        {
            public Configuration()
            {
                HasKey(Current => Current.SliderImageId);
                HasRequired(Current => Current.SliderImage).WithOptional(Current => Current.Setting).WillCascadeOnDelete(true);
                HasOptional(Current => Current.Video).WithMany().HasForeignKey(Current => Current.VideoId).WillCascadeOnDelete(false);
            }
        }
        #endregion

        #region Properties

        [Display(Name = "تصویرِ اسلایدر")]
        public int SliderImageId { get; set; }
        public SliderImage SliderImage { get; set; }

        [Display(Name = "زمانِ شروعِ نمایش")]
        public DateTime? StartAt { get; set; }

        [Display(Name = "زمانِ پایانِ نمایش")]
        public DateTime? EndAt { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        /// <summary>اگر باشد به‌جایِ تصویر پخش می‌شود و تصویرِ اسلاید پوسترِ آن است</summary>
        [Display(Name = "ویدیو")]
        public Guid? VideoId { get; set; }
        public attachment Video { get; set; }

        #endregion
    }
}
