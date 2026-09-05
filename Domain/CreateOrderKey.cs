using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class CreateOrderKey
    {
        [Key]
        public long Id { get; set; }

        public Guid? OrderId { get; set; }
        public Order Order { get; set; }


        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<CreateOrderKey>
        {
            public Configuration()
            {
                HasOptional(Current => Current.Order).WithMany(Current => Current.CreateOrderKeys).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(false);
            }
        }

        #endregion
    }

}
