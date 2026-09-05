using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserActivityBasketItem : Object
    {
        #region Ctor
        public UserActivityBasketItem()
        {

        }

        #endregion


        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserActivityBasketItem>
        {
            public Configuration()
            {
                //HasRequired(Current => Current.userActivity).WithMany(Current => Current.UserActivityBasketItems).HasForeignKey(Current => Current.UserActivityId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        public int UserActivityId { get; set; }
        //public UserActivity userActivity{ get; set; }

        public int ProductPriceId { get; set; }


        #endregion
    }
}
