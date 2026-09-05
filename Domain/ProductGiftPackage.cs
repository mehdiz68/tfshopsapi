using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductGiftPackage
    {
        public ProductGiftPackage()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductGiftPackage>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.ProductGiftPackages).HasForeignKey(Current => Current.Cover);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public attachment attachment { get; set; }



        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public ICollection<Domain.ProductGiftPackageOrder> ProductGiftPackageOrders { get; set; }

        #endregion
    }
}
