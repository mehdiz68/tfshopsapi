using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductGiftPackageOrder
    {
        public ProductGiftPackageOrder()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductGiftPackageOrder>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductGiftPackage).WithMany(Current => Current.ProductGiftPackageOrders).HasForeignKey(Current => Current.ProductGiftPackageId);
                HasRequired(Current => Current.Order).WithMany(Current => Current.ProductGiftPackageOrders).HasForeignKey(Current => Current.OrderId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

      
        [Display(Name = "هدیه")]
        public int ProductGiftPackageId { get; set; }
        public ProductGiftPackage ProductGiftPackage { get; set; }


        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public DateTime InsertDate { get; set; }

        public bool SmsSent { get; set; }
        public DateTime? SmsSentDate { get; set; }


        #endregion
    }
}
