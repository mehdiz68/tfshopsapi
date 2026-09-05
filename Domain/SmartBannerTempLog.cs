using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SmartBannerTempLog : Object
    {
        #region Ctor
        public SmartBannerTempLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmartBannerTempLog>
        {
            public Configuration()
            {
                //HasRequired(Current => Current.UserActivity).WithMany(Current => Current.SmartBannerTempLogs).HasForeignKey(Current => Current.UserActivityId);
                HasRequired(Current => Current.SmartAdImage).WithMany(Current => Current.SmartBannerTempLogs).HasForeignKey(Current => Current.SmartAdImageId);
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

        [Display(Name = "بنر")]
        public int SmartAdImageId { get; set; }
        public SmartAdImage SmartAdImage { get; set; }

        public int Count { get; set; }


        #endregion
    }
}
