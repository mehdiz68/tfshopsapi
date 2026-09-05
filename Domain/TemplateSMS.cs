using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TemplateSMS
    {
        public TemplateSMS()
        {

        }

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = "عنوان وارد نشده است")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage ="متن وارد نشده است")]
        [Display(Name = "متن")]
        public string Text { get; set; }


        [Required(ErrorMessage = "تاریخ ثبت وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }




        #endregion
    }

}

