using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserActivityGroupSelect : Object
    {
        #region Ctor
        public UserActivityGroupSelect()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserActivityGroupSelect>
        {
            public Configuration()
            {
               // HasRequired(Current => Current.UserActivity).WithMany(Current => Current.UserActivityGroupSelects).HasForeignKey(Current => Current.UserActivityId).WillCascadeOnDelete(true);
                HasRequired(Current => Current.UserActivityGroup).WithMany(Current => Current.UserActivityGroupSelects).HasForeignKey(Current => Current.UserActivityGroupId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "بازدیدکننده")]
        [Index("IX_UserActivityId", IsClustered = false, IsUnique = false)]
        public int UserActivityId { get; set; }
       // public UserActivity UserActivity { get; set; }

        [Display(Name = "گروه")]
        [Index("IX_UserActivityGroupId", IsClustered = false, IsUnique = false)]
        public int UserActivityGroupId { get; set; }
        public UserActivityGroup UserActivityGroup { get; set; }

        [Display(Name = "ضریب")]
        public int Ratio { get; set; }

        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ آخرین بروزرسانی")]
        public DateTime UpdateDate { get; set; }

        #endregion
    }
}
