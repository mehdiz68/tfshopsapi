using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class letmeKnowMessage
    {
        public letmeKnowMessage()
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
        public LemeKnowTypeMessage LemeKnowTypeMessage { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public bool state { get; set; }


        //public ICollection<letmeKnowMessageMember> letmeKnowMessageMembers { get; set; }

        #endregion
    }

    public enum LemeKnowTypeMessage
    {
        [Display(Name = "موجودی")]
        موجودی,
        [Display(Name = "تخفیف")]
        تخفیف,
    }
}

