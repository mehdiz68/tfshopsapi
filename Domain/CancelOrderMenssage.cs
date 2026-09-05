using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CancelOrderMenssage
    {
        public CancelOrderMenssage()
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

        
        [Required(ErrorMessage = "نوع محتوای پیام وارد نشده است")]
        [Display(Name = "نوع محتوای پیام")]
        public CancelOrderMenssageType CancelOrderMenssageType { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public bool state { get; set; }


        //public ICollection<letmeKnowMessageMember> letmeKnowMessageMembers { get; set; }

        #endregion
    }

    public enum CancelOrderMenssageType
    {
        [Display(Name = "عادی")]
        عادی,
        [Display(Name = "استعلامی")]
        استعلامی,
    }
}

