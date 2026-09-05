using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserActivityReferer : Object
    {
        #region Ctor
        public UserActivityReferer()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserActivityReferer>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.UserActivityReferers).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ParrentUserActivityReferer).WithMany(Current => Current.ChildUserActivityReferers).HasForeignKey(Current => Current.ParrentId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }
        public string Link { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public attachment attachment { get; set; }

        [Display(Name = "UTMcampaign")]
        public string UTMcampaign { get; set; }
        [Display(Name = "UTMsource")]
        public string UTMsource { get; set; }
        [Display(Name = "UTMmedium")]
        public string UTMmedium { get; set; }

        [Display(Name = "قیمت هر بازدید")]
        public int ppc { get; set; }
        public bool ShowInOrderAdmin { get; set; }

        [Display(Name = "ارجاع اصلی")]
        public int? ParrentId { get; set; }
        public virtual UserActivityReferer ParrentUserActivityReferer { get; set; }
        public virtual ICollection<UserActivityReferer> ChildUserActivityReferers{ get; set; }

        public ICollection<UserActivity> UserActivities { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public ICollection<Order> Orders { get; set; }

        #endregion
    }
}
