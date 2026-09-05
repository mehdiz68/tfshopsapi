using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserActivityFavorate : Object
    {
        #region Ctor
        public UserActivityFavorate()
        {

        }

        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserActivityFavorate>
        {
            public Configuration()
            {
                //HasRequired(Current => Current.UserActivity).WithMany(Current => Current.UserActivityFavorates).HasForeignKey(Current => Current.UserActivityId);
                HasRequired(Current => Current.ProductCategory).WithMany(Current => Current.UserActivityFavorates).HasForeignKey(Current => Current.CatId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }
        //public  UserActivity UserActivity { get; set; }
        public int UserActivityId { get; set; }

        public int CatId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public decimal Rate { get; set; }


        #endregion
    }
}
