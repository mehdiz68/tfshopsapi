using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SmsPattern
    {
        public SmsPattern()
        {

        }

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }
                
        [Required(ErrorMessage ="متن وارد نشده است")]
        [Display(Name = "متن")]
        public string Text { get; set; }


        [Required(ErrorMessage = "تاریخ ثبت وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        
        [Required(ErrorMessage = "نوع الگوی پیام وارد نشده است")]
        [Display(Name = "نوع الگوی پیام")]
        public SmsPatternType smsPatternType { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public bool state { get; set; }



        #endregion
    }

    public enum SmsPatternType
    {
        [Display(Name = "ثبت نام")]
        ثبت_نام,
        [Display(Name = "فراموشی رمزعبور")]
        فراموشی,
        [Display(Name = "تایید سفارش استعلامی")]
        تایید_سفارش_استعلامی,
        [Display(Name = "عدم تایید سفارش استعلامی")]
        عدم_تایید_سفارش_استعلامی,
        [Display(Name = "تغییر وضعیت سفارش")]
        تغییر_وضعیت_سفارش,
        [Display(Name = "ارسال سفارش با پیک")]
        ارسال_سفارش_پیک,
        [Display(Name = "ارسال سفارش به جز پیک پیک")]
        ارسال_سفارش_به_جز_پیک,
        [Display(Name = "تحویل سفارش و درخواست شرکت در نطرسنجی")]
        تحویل_سفارش_درخواست_نظرسنجی_سفارش,
        [Display(Name = "تایید لغو سفارش")]
        تایید_لغو_سفارش,
        [Display(Name = "تایید مرجوعی سفارش")]
        تایید_مرجوعی_سفارش,
        [Display(Name = "ثبت سفارش جدید(مدیر)")]
        ثبت_سفارش_جدید_مدیر,
        [Display(Name = "ثبت سفارش جدید(کاربر)")]
        ثبت_سفارش_جدید_کاربر,
        [Display(Name = "ثبت سفارش جدید استعلامی(کاربر)")]
        ثبت_سفارش_جدید_استعلامی_کاربر,
        [Display(Name = "تایید تلفن همراه(پروفایل)")]
        تایید_تلفن_همراه_پروفایل,
        [Display(Name = "ثبت درخواست لغو(کاربر)")]
        ثبت_درخواست_لغو_کاربر,
        [Display(Name = "اعلام تاخیر ارسال سفارش")]
        اعلام_تاخیر_ارسال_سفارش,
        [Display(Name = "رمز یکبار مصرف ورود")]
        رمز_یکبار_مصرف_ورود,
        [Display(Name = "تایید لغو سفارش مشکل تامین")]
        تایید_لغو_سفارش_مشکل_تامین,
        [Display(Name = "تایید درخواست لغو")]
        تایید_درخواست_لغو,
        [Display(Name = "اعلام مرجوعی")]
        اعلام_مرجوعی,
        [Display(Name = "تغییر وضعیت سفارش حضوری")]
        تغییر_وضعیت_سفارش_حضوری,
        [Display(Name = "پیام بلک لیست")]
        پیام_بلک_لیست,
        [Display(Name = "کارت به کارت - سفارش ادمین")]
        کارت_به_کارت,
        [Display(Name = "آنلاین - سفارش ادمین")]
        آنلاین,
        [Display(Name = "پرداخت در محل - سفارش ادمین")]
        پرداخت_در_محل,
        [Display(Name = "ثبت درخواست لغو(مدیر)")]
        ثبت_درخواست_لغو_مدیر,
        [Display(Name = "عدم تایید لغو سفارش")]
        عدم_تایید_لغو_سفارش,
        [Display(Name = "ثبت به من اطلاع بده ادمین")]
        ثبت_به_من_اطلاع_بده_ادمین,
        [Display(Name = "ارسال هدیه")]
        ارسال_هدیه,
        [Display(Name = "تایید کارت به کارت")]
        تایید_کارت_به_کارت,
        [Display(Name = "عدم تایید کارت به کارت")]
        عدم_تایید_کارت_به_کارت,
        [Display(Name = "آماده ارسال سفارش اشتراکی")]
        آماده_ارسال_سفارش_اشتراکی,
    }
}

