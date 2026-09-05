using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class PhoneChangeLog : Object
    {
        #region Ctor
        public PhoneChangeLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<PhoneChangeLog>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.PhoneChangeLogs).HasForeignKey(Current => Current.UserId);
            }
        }
        #endregion

        #region Properties

        [Key]
        public int Id { get; set; }

        public bool OldValue { get; set; }
        public bool NewValue { get; set; }
        public DateTime ChangeDate { get; set; }


        public string UserId { get; set; }
        public ApplicationUser User { get; set; }



        #endregion
    }
}
