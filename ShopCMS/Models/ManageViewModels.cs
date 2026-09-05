using Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TfShop.Infrastructure.Helper;

namespace TfShop.Models
{
    public class IndexViewModelV2
    {
        public bool HasPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool BrowserRemembered { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string About { get; set; }
        public bool? Gender { get; set; }
        public string LandlinePhone { get; set; }
        public string NationalCode { get; set; }
        public int State { get; set; }
        public string City { get; set; }
        public string CardNumber { get; set; }
        public int BonCount { get; set; }
        public int CodeCount { get; set; }
        public int NoticeCount { get; set; }
        public int FavCount { get; set; }
        public long wallet { get; set; }
        public string Address { get; set; }
        public bool IsInNewsLetter { get; set; }
        public DateTime? BirthDate { get; set; }
        public System.Guid? Avatar { get; set; }


        public string Avatarattachment { get; set; }
        public int MessageCout { get; set; }
    }
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool BrowserRemembered { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string About { get; set; }
        public bool? Gender { get; set; }
        public string LandlinePhone { get; set; }
        public string NationalCode { get; set; }
        public int State { get; set; }
        public string City { get; set; }
        public string CardNumber { get; set; }
        public int BonCount { get; set; }
        public int CodeCount { get; set; }
        public int NoticeCount { get; set; }
        public int FavCount { get; set; }
        public long wallet { get; set; }
        public string Address { get; set; }
        public bool IsInNewsLetter { get; set; }
        public DateTime? BirthDate { get; set; }
        public System.Guid? Avatar { get; set; }


        public Domain.attachment Avatarattachment { get; set; }
        public ApplicationUser AppUser { get; set; }
        public int MessageCout { get; set; }
    }
    public class EditProfileViewModel
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }

        //[RegularExpression(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " نام ")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }


        [Display(Name = "ایمیل ")]
        [EmailAddress(ErrorMessage = "ایمیل صحیح نیست.")]
        public string Email { get; set; }

        //[RegularExpression(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "جنسیت را مشخص نمایید")]
        public bool? Gender { get; set; }


        [Display(Name = "آواتار ( 80 * 80 پیکسل، اختیاری)")]
        public System.Guid? Avatar { get; set; }
        public Domain.attachment Avatarattachment { get; set; }

        [Display(Name = "درباره")]
        public string About { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [StringLength(10, ErrorMessage = "طول رمز عبور حداقل باید 10 کارکتر باشد.", MinimumLength = 10)]
        [Required(ErrorMessage = " کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "استان")]
        public int State { get; set; }


        [Required(ErrorMessage = " شهر باید وارد شود.")]
        public int? CityId { get; set; }
        public City CityEntity { get; set; }

        [Display(Name = "شهر")]
        public string City { get; set; }


        //[RegEx(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", System.Text.RegularExpressions.RegexOptions.Multiline, ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " آدرس باید وارد شود.")]
        [Display(Name = "آدرس (فارسی درج شود)")]
        public string Address { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        //[Required(ErrorMessage = " کد ملی باید وارد شود.")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره کارت بانکی")]
        public string CardNumber { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره حساب بانکی")]
        public string AccountNumber { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره شبای بانکی")]
        public string SHBNNumber { get; set; }


        [Display(Name = "پلاک")]
        public string AddressNumber { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }


        [Display(Name = "عرض جغرافیایی")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public string FooterGoogleMapLatitude { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }

    public class EditProfileViewModelStep1
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }

        //[RegularExpression(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " نام ")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }


        [Display(Name = "ایمیل ")]
        [EmailAddress(ErrorMessage = "ایمیل صحیح نیست.")]
        public string Email { get; set; }

        //[RegularExpression(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "جنسیت را مشخص نمایید")]
        public bool? Gender { get; set; }


        [Display(Name = "آواتار ( 80 * 80 پیکسل، اختیاری)")]
        public System.Guid? Avatar { get; set; }
        public Domain.attachment Avatarattachment { get; set; }

        [Display(Name = "درباره")]
        public string About { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }




        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        //[Required(ErrorMessage = " کد ملی باید وارد شود.")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }


        public bool PhoneNumberConfirmed { get; set; }
    }

    public class EditProfileViewModelStep1V2
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }

        //[RegularExpression(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " نام ")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }


        [Display(Name = "ایمیل ")]
        [EmailAddress(ErrorMessage = "ایمیل صحیح نیست.")]
        public string Email { get; set; }

        //[RegularExpression(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "جنسیت را مشخص نمایید")]
        public bool? Gender { get; set; }


        [Display(Name = "آواتار ( 80 * 80 پیکسل، اختیاری)")]
        public System.Guid? Avatar { get; set; }
        public string Avatarattachment { get; set; }

        [Display(Name = "درباره")]
        public string About { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }




        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        //[Required(ErrorMessage = " کد ملی باید وارد شود.")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }


        public bool PhoneNumberConfirmed { get; set; }
    }

    public class EditAddressUser
    {
        public string firstName { get; set; }
        public string LastName { get; set; }
    }
    public class EditAddressViewModel
    {
        public string Title { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string AddressNumber { get; set; }
        public string AddressUnit { get; set; }
        public int Id { get; set; }
    }
    public class EditProfileViewModelStep2
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }



      //  [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
       // [StringLength(10, ErrorMessage = "طول رمز عبور حداقل باید 10 کارکتر باشد.", MinimumLength = 10)]
        [Required(ErrorMessage = " کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "استان")]
        public int State { get; set; }


        [Required(ErrorMessage = " شهر باید وارد شود.")]
        public int? CityId { get; set; }
        public City CityEntity { get; set; }

        [Display(Name = "شهر")]
        public string City { get; set; }


        //[RegEx(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", System.Text.RegularExpressions.RegexOptions.Multiline, ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " خیابان و کوچه باید وارد شود.")]
        [Display(Name = "خیابان و کوچه (فارسی درج شود)")]
        public string Address { get; set; }




        [Display(Name = "پلاک")]
        [Required(ErrorMessage = " پلاک باید وارد شود.")]
        public string AddressNumber { get; set; }



        [Display(Name = "عنوان آدرس")]
        [Required(ErrorMessage = "عنوان آدرس باید وارد گردد")]
        public string AddressTitle { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }


        [Display(Name = "عرض جغرافیایی")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public string FooterGoogleMapLatitude { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }

    public class EditProfileViewModelStep2V2
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }



        //  [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        // [StringLength(10, ErrorMessage = "طول رمز عبور حداقل باید 10 کارکتر باشد.", MinimumLength = 10)]
        [Required(ErrorMessage = " کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "استان")]
        public int State { get; set; }


        [Required(ErrorMessage = " شهر باید وارد شود.")]
        public int? CityId { get; set; }
        public string City { get; set; }

        [Required(ErrorMessage = " شهر باید وارد شود.")]
        public int? ProvinceId { get; set; }
        public string Province { get; set; }

        public string FullName { get; set; }

        //[RegEx(@"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$", System.Text.RegularExpressions.RegexOptions.Multiline, ErrorMessage = "لطفا کیبورد خود را روی فارسی قرار دهید")]
        [Required(ErrorMessage = " خیابان و کوچه باید وارد شود.")]
        [Display(Name = "خیابان و کوچه (فارسی درج شود)")]
        public string Address { get; set; }




        [Display(Name = "پلاک")]
        [Required(ErrorMessage = " پلاک باید وارد شود.")]
        public string AddressNumber { get; set; }



        [Display(Name = "عنوان آدرس")]
        [Required(ErrorMessage = "عنوان آدرس باید وارد گردد")]
        public string AddressTitle { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }


        [Display(Name = "عرض جغرافیایی")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public string FooterGoogleMapLatitude { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }

    public class EditProfileViewModelStep3
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = " شماره کارت بانکی باید وارد شود.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره کارت بانکی")]
        public string CardNumber { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره حساب بانکی")]
        public string AccountNumber { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره شبای بانکی")]
        public string SHBNNumber { get; set; }



        public bool PhoneNumberConfirmed { get; set; }
    }


    public class EditProfileViewModelStep3V2
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = " شماره کارت بانکی باید وارد شود.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره کارت بانکی")]
        public string CardNumber { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره حساب بانکی")]
        public string AccountNumber { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره شبای بانکی")]
        public string SHBNNumber { get; set; }



        public bool PhoneNumberConfirmed { get; set; }
    }

    public class UserAddressV2
    {
        public  int Id { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public int? CityId { get; set; }        
        public string Address { get; set; }        
        public string AddressNumber { get; set; }        
        public string AddressUnit { get; set; }        
        public string PostalCode { get; set; }        
        public string FooterGoogleMapLatitude { get; set; }        
        public string FooterGoogleMapLongitude { get; set; }        
        public int? provinceId { get; set; }
        public string CityName { get; set; }        
        public string ProvinceName { get; set; } 
    }

    public class EditProfile
    {
        public EditProfileViewModelStep1 EditProfileViewModelStep1;
        public EditProfileViewModelStep2 EditProfileViewModelStep2;
        public EditProfileViewModelStep3 EditProfileViewModelStep3;
    }
    public class EditProfileV2
    {
        public EditProfileViewModelStep1V2 EditProfileViewModelStep1;
        public EditProfileViewModelStep2V2 EditProfileViewModelStep2;
        public EditProfileViewModelStep3V2 EditProfileViewModelStep3;
    }
    public class ChangePasswordViewModel
    {
        //    [Required(ErrorMessage = "رمز عبور فعلی خالی است")]
        //    [DataType(DataType.Password)]
        //    [Display(Name = "رمز عبور فعلی")]
        //    public string OldPassword { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ورود دوباره رمز عبورِ جدید")]
        [Compare("NewPassword", ErrorMessage = "رمز های عبور وارد شده یکسان نیستند.")]
        public string ConfirmPassword { get; set; }
    }
}