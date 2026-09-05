using System;
using System.ComponentModel.DataAnnotations;

namespace TfShop.ViewModels.Slider
{
    public class SliderImageViewModel
    {
        #region Ctor
        public SliderImageViewModel()
        {

        }
      
        #endregion

        #region Properties

        [Key]
        [Required]
        public Guid? Cover { get; set; }

    
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        public string Link { get; set; }


        #endregion
    }
}
