using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// «درخواست همکاری» - فرمِ CTAیِ سایت (خرید عمده / اعطای نمایندگی و ...): نام، نام خانوادگی و موبایل،
    /// همراهِ صفحه‌ای که فرم در آن پر شده (SourcePage/SourceTitle) تا در پنل معلوم باشد از کجا آمده.
    /// پنل: Admin/CooperationRequests. ثبت: apiv1/SubmitCooperationRequest.
    ///
    /// در هر دو کپیِ Domain (Tfshop و TfShopApi) عیناً یکی است؛ هر دو به یک دیتابیس با
    /// AutomaticMigrationDataLossAllowed وصل‌اند، پس اختلافِ مدل یعنی حذفِ جدول توسطِ اپِ دیگر.
    /// </summary>
    public class CooperationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Required, MaxLength(11)]
        [Display(Name = "موبایل")]
        public string Mobile { get; set; }

        /// <summary>موضوعِ فرم، مثلاً wholesale-dealership (یک فرم ممکن است بعداً با موضوعِ دیگری در صفحاتِ دیگر هم بیاید)</summary>
        [MaxLength(50)]
        [Display(Name = "موضوع")]
        public string Topic { get; set; }

        /// <summary>مسیرِ صفحه‌ای که فرم در آن پر شد، مثلاً /about-us</summary>
        [MaxLength(400)]
        [Display(Name = "صفحه")]
        public string SourcePage { get; set; }

        /// <summary>عنوانِ آن صفحه، مثلاً «درباره ما»</summary>
        [MaxLength(150)]
        [Display(Name = "عنوانِ صفحه")]
        public string SourceTitle { get; set; }

        [MaxLength(64)]
        public string Ip { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "وضعیت")]
        public CooperationRequestStatus Status { get; set; }

        [MaxLength(1000)]
        [Display(Name = "یادداشتِ کارشناس")]
        public string AdminNote { get; set; }

        /// <summary>آخرین تغییرِ وضعیت/یادداشت در پنل</summary>
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(128)]
        public string UpdatedBy { get; set; }
    }

    public enum CooperationRequestStatus : byte
    {
        جدید = 0,
        تماس_گرفته_شد = 1,
        بسته_شد = 2
    }
}
