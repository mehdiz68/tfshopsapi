using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SmartProductTempLog : Object
    {
        #region Ctor
        public SmartProductTempLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmartProductTempLog>
        {
            public Configuration()
            {
                //HasRequired(Current => Current.UserActivity).WithMany(Current => Current.SmartProductTempLogs).HasForeignKey(Current => Current.UserActivityId);
                HasRequired(Current => Current.Product).WithMany(Current => Current.SmartProductTempLogs).HasForeignKey(Current => Current.ProductId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "مخاطب")]
        public int UserActivityId { get; set; }
        //public UserActivity UserActivity { get; set; }

        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Count { get; set; }


        #endregion
    }
}
