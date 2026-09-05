using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Gender { get; set; }
        public bool Disable { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid? Avatar { get; set; }
        public attachment Avatarattachment { get; set; }
        public string About { get; set; }
        public string LandlinePhone { get; set; }
        public string PostalCode { get; set; }
        public int State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string AddressNumber{ get; set; }
        public string AddressUnit { get; set; }
        public string AddressTitle { get; set; }

        public string FooterGoogleMapLongitude { get; set; }

        public string FooterGoogleMapLatitude { get; set; }
        public string NationalCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CardNumber { get; set; }
        public string AccountNumber { get; set; }
        public string SHBNNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public int? CityId { get; set; }
        public City CityEntity { get; set; }
        public string ActiveTempCode { get; set; }
        public DateTime? ActiveTempCodeExpire { get; set; }
        public bool firstTime { get; set; }
        public bool Haghighi { get; set; }
        public string ErpCode { get; set; }
        public string ReturnParam1 { get; set; }
        public string ReturnParam2 { get; set; }
        public string ReturnParam3 { get; set; }
        public UserActivityReferer UserActivityReferer { get; set; }
        public int? UserActivityRefererId { get; set; }
        public bool BlackList { get; set; }
        public bool SendSmsBlackList { get; set; }
        public string MessageBlackList { get; set; }
        public string JWTtoken { get; set; }
        public DateTime? JWTtokenExpire { get; set; }

        // معادل refresh token برای احراز هویت JWT سمت فرانت (Next.js) — چون JWTtoken بالا فقط ۱ روز
        // اعتبار داره (config:JwtExpireDays) و تا قبل از این هیچ راهی برای تمدیدش بدون لاگین دوباره
        // وجود نداشت. این فیلد مقدار HASH‌شده‌ی (SHA-256) رفرش‌توکن رو نگه می‌داره (نه خودِ متن خام
        // رو - مثل هش پسورد)، تا با لو رفتن دیتابیس، توکن‌های قابل استفاده لو نره؛ ببینید
        // TfShop.Infrastructure.AuthJTW.Authentication.HashToken/GenerateRefreshToken و اکشن
        // ApiV1Controller.AuthRefreshToken.
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpire { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ApplicationUser>
        {
            public Configuration()
            {
                HasOptional(Current => Current.CityEntity).WithMany(Current => Current.Users).HasForeignKey(Current => Current.CityId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Avatarattachment).WithMany(Current => Current.Avatars).HasForeignKey(Current => Current.Avatar).WillCascadeOnDelete(false);
                HasOptional(Current => Current.UserActivityReferer).WithMany(Current => Current.ApplicationUsers).HasForeignKey(Current => Current.UserActivityRefererId);

            }
        }

        public ICollection<SearchLog> SearchLogs { get; set; }
        public ICollection<attachment> Attachements { get; set; }
        public ICollection<AdministratorPermission> AdministratorPermissions { get; set; }
        public ICollection<Content> Contents { get; set; }
        public ICollection<EventLog> EventLogs { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<ProductQuestion> ProductQuestions { get; set; }
        public ICollection<ProductComment> ProductComments { get; set; }
        public ICollection<ProductLetmeknow> ProductLetmeknows { get; set; }
        public ICollection<ProductFavorate> ProductFavorates { get; set; }
        public ICollection<ProductRankSelectValue> ProductRankSelectValues { get; set; }
        public ICollection<Wallet> Wallets { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }
        public ICollection<UserMessage> UserMessages { get; set; }
        public ICollection<UserMessage> UserMessage2s { get; set; }
        public ICollection<UserBon> UserBons { get; set; }
        public ICollection<Seller> Sellers { get; set; }
        public ICollection<UserCodeGiftLog> UserCodeGiftLogs { get; set; }
        public ICollection<CourseRating> CourseRatings { get; set; }
        public ICollection<Offer> Offers { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Order> AdminOrders { get; set; }
        public ICollection<UserCodeGift> UserCodeGifts { get; set; }
        public ICollection<ProductPriceGroupModification> ProductPriceGroupModifications { get; set; }
        public ICollection<UserGroupSelect> UserGroupSelects { get; set; }
        public ICollection<UserOfferMessageMember> UserOfferMessageMembers { get; set; }
        public ICollection<UserGroupMessageMember> UserGroupMessageMembers { get; set; }
        public ICollection<PhoneChangeLog> PhoneChangeLogs { get; set; }
        //public ICollection<letmeKnowMessageMember> letmeKnowMessageMembers { get; set; }
    }
}
