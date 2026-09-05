using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ProductCategoryFAQ : Object
    {
        #region Ctor
        public ProductCategoryFAQ()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCategoryFAQ>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductCategory).WithMany(Current => Current.productCategoryFAQs).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(true);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "پرسش")]
        public string Question { get; set; }

        [Required]
        [Display(Name = "پاسخ")]
        public string Answer { get; set; }
    
        [Display(Name = "انتخاب محتوا")]
        public int CatId { get; set; }
        public ProductCategory ProductCategory { get; set; }

        [Required]
        public int DisplaySort { get; set; }


        #endregion
    }
}
