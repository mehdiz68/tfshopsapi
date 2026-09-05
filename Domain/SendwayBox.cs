using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SendwayBox
    {
        public SendwayBox()
        {

        }


        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = "عنوان باکس ارسال باید وارد شود")]
        [Display(Name = "عنوان باکس ارسال")]
        [MaxLength(200, ErrorMessage = "حداکثر طول کارکتر ، 200")]
        public string Title { get; set; }



        [Required]
        [Display(Name = "ماهیت باکس")]
        public ProductPackageType ProductPackageType { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حداکثر ارتفاع بسته ( سانتی متر)")]
        public int Height { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حداکثر عرض بسته ( سانتی متر)")]
        public int Width { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حداکثر طول بسته ( سانتی متر)")]
        public int Lenght { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حداکثر وزن بسته ( گرم)")]
        public int ProductWeight { get; set; }


        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "هزینه کارتن")]
        public int BoxPrice { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "درصد افزایش قیمت")]
        public int BoxPricePercent { get; set; }


        public ICollection<OrderDelivery> OrderDeliveries { get; set; }
        public ICollection<ProductSendWayBox> ProductSendWayBoxes { get; set; }
        public ICollection<FreeSendOfferState> FreeSendOfferStates { get; set; }

        #endregion
    }

    /// <summary>
    /// اندازه محموله
    /// </summary>
    public enum ProductPackageType
    {
        [Display(Name = "عادی")]
        normal,
        [Display(Name = "بزرگ")]
        medium,
        [Display(Name = "سنگین")]
        high


    }

}
