using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UnSentMessage
    {
        public UnSentMessage()
        {

        }


        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }
                
        [Required(ErrorMessage ="متن وارد نشده است")]
        [Display(Name = "متن")]
        public string Text { get; set; }

        [Required(ErrorMessage = "شماره موبایل وارد نشده است")]
        [Display(Name = "شماره موبایل")]
        public string Mobile { get; set; }


        [Required(ErrorMessage = "تاریخ ثبت وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public bool state { get; set; }
        public bool InUpdate { get; set; }

        [Required]
        [Display(Name = "روشن")]
        public bool IsActive { get; set; }


        #endregion
    }


}
