using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserGroupMessageMember
    {
        public UserGroupMessageMember()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserGroupMessageMember>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserGroupMessageMembers).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.UserGroupMessage).WithMany(Current => Current.UserGroupMessageMembers).HasForeignKey(Current => Current.UserGroupMessageId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "دریافت کننده وارد نشده است")]
        [Display(Name = "دریافت کننده")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "پیام وارد نشده است")]
        public int UserGroupMessageId { get; set; }
        public UserGroupMessage UserGroupMessage { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public OfferMessageSendMessageType state { get; set; }

        [Required(ErrorMessage = "تاریخ ارسال وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }



        #endregion
    }

   
}
