using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserOfferMessageMemberTemp
    {
        public UserOfferMessageMemberTemp()
        {

        }

        

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "دریافت کننده وارد نشده است")]
        [Display(Name = "دریافت کننده")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "نام و نام خانوادگی وارد نشده است")]
        [Display(Name = "نام و نام خانوادگی دریافت کننده")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "پیام وارد نشده است")]
        public string Text { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public OfferMessageSendMessageType state { get; set; }

        [Required(ErrorMessage = "تاریخ ارسال وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }



        #endregion
    }

}
