using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserActivity : Object
    {
        #region Ctor
        public UserActivity()
        {

        }

        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserActivity>
        {
            public Configuration()
            {
                //HasOptional(Current => Current.ParrentUserActivity).WithMany(Current => Current.ChildUserActivities).HasForeignKey(Current => Current.CookieId);
                //HasOptional(Current => Current.UserActivityReferer).WithMany(Current => Current.UserActivities).HasForeignKey(Current => Current.UserActivityRefererId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public string SessionId { get; set; }

        public int? CookieId { get; set; }
        public int? RouteValueId { get; set; }
       // public  UserActivity ParrentUserActivity { get; set; }
        //public virtual ICollection<UserActivity> ChildUserActivities{ get; set; }
        //public ICollection<SmartAdLog> SmartAdLogs { get; set; }

        [Display(Name = "زمان ثبت")]
        public DateTime LogDateTime { get; set; }

        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Platform")]
        public string Platform { get; set; }
        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }
        [Display(Name = "Browser")]
        public string Browser { get; set; }

        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "Referer")]
        public string Referer { get; set; }

        [Display(Name = "UTMcampaign")]
        public string UTMcampaign { get; set; }
        [Display(Name = "UTMsource")]
        public string UTMsource { get; set; }
        [Display(Name = "UTMmedium")]
        public string UTMmedium { get; set; }

        [Display(Name = "PATH_INFO")]
        public string PATH_INFO { get; set; }

        [Display(Name = "QUERY_STRING")]
        public string QUERY_STRING { get; set; }

        [Display(Name = "HTTP_METHOD")]
        public string HTTP_METHOD { get; set; }

        [Display(Name = "CONTENT_TYPE")]
        public string CONTENT_TYPE { get; set; }
        [Display(Name = "LOGON_USER")]
        public string LOGON_USER { get; set; }

        public int? UserActivityRefererId { get; set; }

        //public UserActivityReferer UserActivityReferer { get; set; }

        public DateTime? LastFavorateUpdate { get; set; }

       // public ICollection<UserActivityBasketItem> UserActivityBasketItems { get; set; }
        //public ICollection<UserActivityGroupSelect> UserActivityGroupSelects { get; set; }
       // public ICollection<UserActivityFavorate> UserActivityFavorates { get; set; }
       // public ICollection<SmartProductTempLog> SmartProductTempLogs { get; set; }
       // public ICollection<SmartBannerTempLog> SmartBannerTempLogs { get; set; }
        //public ICollection<PushMember> PushMembers { get; set; }


        #endregion
    }
}
