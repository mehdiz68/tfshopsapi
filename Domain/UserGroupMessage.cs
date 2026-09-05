using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserGroupMessage
    {
        public UserGroupMessage()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserGroupMessage>
        {
            public Configuration()
            {
                HasMany(u => u.UserGroups).WithMany(m => m.UserGroupMessages).Map(m =>
                {
                    m.ToTable("UserGroupMessagesUserGroups");
                    m.MapLeftKey("UserGroupMessageId");  // because it is the "left" column, isn't it?
                    m.MapRightKey("UserGroupId"); // because it is the "right" column, isn't it?
                });
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }
                
        [Required(ErrorMessage ="متن وارد نشده است")]
        [Display(Name = "متن")]
        public string Text { get; set; }


        [Required(ErrorMessage = "تاریخ ثبت وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage = "نوع پیام وارد نشده است")]
        [Display(Name = "نوع پیام")]
        public OfferMessageType OfferMessageType { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public bool state { get; set; }

        [Required]
        [Display(Name = "روشن")]
        public bool IsActive { get; set; }

        [Display(Name = "زمان ارسال")]
        public DateTime? StartDate { get; set; }

        public ICollection<UserGroupMessageMember> UserGroupMessageMembers { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }

        #endregion
    }


}
