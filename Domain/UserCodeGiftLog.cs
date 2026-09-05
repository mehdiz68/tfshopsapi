using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserCodeGiftLog : Object
    {
        #region Ctor
        public UserCodeGiftLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserCodeGiftLog>
        {
            public Configuration()
            {
                HasRequired(Current => Current.UserCodeGift).WithMany(Current => Current.UserCodeGiftLogs).HasForeignKey(Current => Current.UserCodeGiftId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Order).WithMany(Current => Current.UserCodeGiftLogs).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.User).WithMany(Current => Current.UserCodeGiftLogs).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Required]
        [Display(Name = "کد تخفیف")]
        public int UserCodeGiftId { get; set; }
        public  UserCodeGift UserCodeGift { get; set; }


        [Display(Name = "سفارش")]
        public Guid? OrderId { get; set; }
        public  Order Order { get; set; }

        [Required]
        [Display(Name = "مقدار مصرفی")]
        public long Value { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }


        [Display(Name = "وضعیت")]
        public bool state { get; set; }


        [Display(Name = "مشتری")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        #endregion
    }
}
