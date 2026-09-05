using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class contentProduct : Object
    {
        #region Ctor
        public contentProduct()
        {

        }

        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<contentProduct>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithMany(Current => Current.contentProducts).HasForeignKey(Current => Current.ContentId).WillCascadeOnDelete(true);
                HasRequired(Current => Current.Product).WithMany(Current => Current.contentProducts).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(true);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "انتخاب محصول")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Display(Name = "انتخاب محتوا")]
        public int ContentId { get; set; }
        public Content Content { get; set; }

        public contentProductType contentProductType { get; set; }

        #endregion
    }
    public enum contentProductType
    {

        [Display(Name = "معرفی محصول")]
        معرفی_محصول,
        [Display(Name = "آنباکس محصول")]
        آنباکس_محصول,
        [Display(Name = "پادکست محصول")]
        پادکست_محصول,
        [Display(Name = "راهنمای نصب محصول")]
        راهنمای_نصب_محصول
    }
}
