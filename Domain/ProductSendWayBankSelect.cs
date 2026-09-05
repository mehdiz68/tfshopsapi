using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendWayBankSelect
    {
        public ProductSendWayBankSelect()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSendWayBankSelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductSendWay).WithMany(Current => Current.ProductSendWayBankSelects).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.BankAccount).WithMany(Current => Current.ProductSendWayBankSelects).HasForeignKey(Current => Current.BankAccountId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "روش ارسال")]
        public int ProductSendWayId { get; set; }
        public ProductSendWay ProductSendWay { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "روش پرداخت")]
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        #endregion
    }
}
