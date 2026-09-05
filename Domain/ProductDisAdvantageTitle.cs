using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductDisAdvantageTitle : Object
    {
        #region Ctor
        public ProductDisAdvantageTitle()
        {

        }
        #endregion
        

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }


        [Display(Name = "ترتیب نمایش")]
        public int DisplayOrder { get; set; }


        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        #endregion
    }
}
