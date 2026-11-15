using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// برگشتِ کاربر از نظرسنجیِ سفارش در Porsline.
    /// Porsline بعد از ثبتِ نظرسنجی کاربر را به /profile/rate/{شماره سفارش}?src=porsline می‌فرستد و API یک ردیف
    /// (به ازایِ هر سفارش یکی) ثبت/به‌روز می‌کند. پنل (مدیریت نظرات > نظرسنجی سفارشات) با آن برچسبِ «از Porsline» می‌زند.
    /// رابطه یک‌به‌صفر‌یا‌یک با Order است (کلیدِ اصلی همان OrderId) و با حذفِ سفارش حذف می‌شود.
    /// </summary>
    public class OrderSurveyReturn : Object
    {
        #region Ctor

        public OrderSurveyReturn()
        {
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderSurveyReturn>
        {
            public Configuration()
            {
                HasKey(Current => Current.OrderId);
                HasRequired(Current => Current.Order).WithOptional().WillCascadeOnDelete(true);
            }
        }

        #endregion

        #region Properties

        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        [Display(Name = "اولین برگشت")]
        public DateTime FirstAt { get; set; }

        [Required]
        [Display(Name = "آخرین برگشت")]
        public DateTime LastAt { get; set; }

        [Required]
        [Display(Name = "تعداد برگشت")]
        public int Hits { get; set; }

        #endregion
    }
}
