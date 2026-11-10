using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// یک ویدیویِ «تی‌اف شو» - استوری‌هایِ صفحه‌ی اصلیِ سایت (منویی با لینکِ /tfshow آن‌ها را مثلِ استوریِ
    /// اینستاگرام یکی‌یکی پخش می‌کند). هر ویدیو یا لینکِ آپارات است یا فایلِ mp4 از فایل‌منیجر.
    ///
    /// در هر دو کپیِ Domain (Tfshop و TfShopApi) عیناً یکی است؛ هر دو به یک دیتابیس با
    /// AutomaticMigrationDataLossAllowed وصل‌اند، پس اختلافِ مدل یعنی حذفِ جدول توسطِ اپِ دیگر.
    /// </summary>
    public class TfShowVideo : Object
    {
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TfShowVideo>
        {
            public Configuration()
            {
                HasOptional(Current => Current.Video).WithMany().HasForeignKey(Current => Current.VideoId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Cover).WithMany().HasForeignKey(Current => Current.CoverId).WillCascadeOnDelete(false);
            }
        }
        #endregion

        #region Properties

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان را وارد کنید")]
        [MaxLength(150, ErrorMessage = "حداکثر ۱۵۰ کاراکتر")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        /// <summary>شناسه‌ی ویدیو در آپارات (از لینکِ aparat.com/v/...)؛ یا این یا VideoId</summary>
        [MaxLength(50)]
        [Display(Name = "ویدیویِ آپارات")]
        public string AparatHash { get; set; }

        /// <summary>فایلِ mp4 (attachments)؛ یا این یا AparatHash</summary>
        [Display(Name = "فایلِ ویدیو")]
        public Guid? VideoId { get; set; }
        public attachment Video { get; set; }

        /// <summary>تصویرِ کاور؛ اگر خالی باشد برایِ آپارات پوسترِ خودِ آپارات (PosterUrl)</summary>
        [Display(Name = "کاور")]
        public Guid? CoverId { get; set; }
        public attachment Cover { get; set; }

        /// <summary>پوسترِ آپارات - هنگامِ ذخیره از API آپارات گرفته می‌شود</summary>
        [MaxLength(400)]
        public string PosterUrl { get; set; }

        /// <summary>مدتِ ویدیو به ثانیه - برایِ آپارات لازم است (پخش‌کننده‌ی آپارات پایانِ ویدیو را خبر نمی‌دهد)</summary>
        [Display(Name = "مدت (ثانیه)")]
        public int? DurationSeconds { get; set; }

        /// <summary>متنِ «مشخصات» زیرِ ویدیو (اختیاری) - در سایت با زدنِ «مشخصات» باز می‌شود</summary>
        [MaxLength(2000, ErrorMessage = "حداکثر ۲۰۰۰ کاراکتر")]
        [Display(Name = "کپشن")]
        public string Caption { get; set; }

        [MaxLength(50, ErrorMessage = "حداکثر ۵۰ کاراکتر")]
        [Display(Name = "متنِ دکمه")]
        public string ButtonTitle { get; set; }

        [MaxLength(400)]
        [Display(Name = "لینکِ دکمه")]
        public string ButtonUrl { get; set; }

        [Display(Name = "ترتیبِ نمایش")]
        public int DisplaySort { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        #endregion
    }
}
