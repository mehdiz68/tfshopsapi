using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class PopUp : Object
    {
        #region Ctor
        public PopUp()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<City>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Province).WithMany(Current => Current.Cities).HasForeignKey(Current => Current.ProvinceId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = "وارد کردن عنوان، اجباری است")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }


        /*
         0- content
         1- only image
             */
        [Display(Name = "نوع پاپ آپ")]
        [Required(ErrorMessage = "اجباری")]
        public bool PopUpType { get; set; }

        [Display(Name = "محتوای پنجره پاپ آپ صفحه اصلی")]
        public string PopUpMessage { get; set; }

        public int PopUpEditVersion { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "محل نمایش انتخاب نشده است")]
        [Display(Name = "محل نمایش(نوع صفحه)")]
        public PopUpPage TypeId { get; set; }

        [Display(Name = "محل نمایش(صفحه)")]
        public int? LinkId { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }


        [Required(ErrorMessage = "تعداد نمایش در روز را وارد نمایید")]
        [Display(Name = "تعداد نمایش در روز")]
        public int CountViewPerDay { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ شروع(اختیاری)")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ پایان(اختیاری)")]
        public DateTime? ExpireDate { get; set; }

        #endregion
    }

    public enum PopUpPage
    {

        [Display(Name = "--همه صفحات--")]
        AllPage,
        [Display(Name = "انوع محتوا")]
        contentType,
        [Display(Name = "دسته بندی محتوا")]
        category,
        [Display(Name = "محتوا")]
        content,
        [Display(Name = "برچسب")]
        tag,
        [Display(Name = "دسته بندی محصول")]
        productCategory,
        [Display(Name = "فیلتر دسته بندی محصول")]
        productSearch,
        [Display(Name = "برند")]
        Brand,
        [Display(Name = "محصول")]
        Product,

    }
}
