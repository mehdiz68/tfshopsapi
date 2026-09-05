using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductletmeKnowViewModel
    {
        #region Ctor
        public ProductletmeKnowViewModel()
        {

        }
        #endregion

        #region Properties


        public int ProductId { get; set; }
        public string ProductPageAddress { get; set; }
        public int ProductDescr { get; set; }
        public string PrName { get; set; }
        public ProductAttributeSelect ProductAttributeSelectModel { get; set; }
        public int? ProductAttributeSelectModelId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectSize { get; set; }
        public int? ProductAttributeSelectSizeId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectColor { get; set; }
        public int? ProductAttributeSelectColorId { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }

        public int Id { get; set; }
        public bool AmazingOffer { get; set; }
        public bool Available { get; set; }
        /*
          1- ایمیل
          2- اس ام اس
          3- سیستم پیام فروشگاه
          4- ایمیل و اس ام اس
          5- ایمیل و سیستم پیام فروشگاه
          6- اس ام اس و سیستم پیام فروشگاه
          7- ایمیل ، اس ام اس ، سیستم پیام فروشگاه
              */
        public short NotificationType { get; set; }
        public DateTime InsertDate { get; set; }

        #endregion
    }
}
