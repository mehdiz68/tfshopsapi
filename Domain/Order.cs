using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Order : BaseEntityDate
    {
        public Order()
        {
            Id = Guid.NewGuid();
            InsertDate = DateTime.Now;
        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Order>
        {
            public Configuration()
            {
                //HasMany(u => u.Wallets).WithMany(m => m.Orders).Map(m =>
                //{
                //    m.ToTable("OrderWallets");
                //    m.MapLeftKey("WalletId");  // because it is the "left" column, isn't it?
                //    m.MapRightKey("OrderId"); // because it is the "right" column, isn't it?
                //});

                HasOptional(Current => Current.Offer).WithMany(Current => Current.Orders).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.User).WithMany(Current => Current.Orders).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.AdminUser).WithMany(Current => Current.AdminOrders).HasForeignKey(Current => Current.AminUserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ParentOrder).WithMany(Current => Current.ChildOrders).HasForeignKey(Current => Current.OrderId);
                HasOptional(Current => Current.UserActivityReferer).WithMany(Current => Current.Orders).HasForeignKey(Current => Current.UserActivityRefererId);
                HasOptional(Current => Current.UserPayAttachment).WithMany(Current => Current.UserPayAttachments).HasForeignKey(Current => Current.UserPayAttach);
                HasOptional(Current => Current.AdminPayAttachment).WithMany(Current => Current.AdminPayAttachments).HasForeignKey(Current => Current.AdminPayAttach);

            }
        }

        #endregion

        #region Properties


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        
        public bool Isdelete { get; set; }
        [Required]
        [Display(Name = "تخصیص کد تخفیف نظر سفارش")]
        public bool SetGiftCode { get; set; }


        [Required]
        [Display(Name = "تکمیل نظرسنجی")]
        public bool compeleteRate{ get; set; }

        [Required]
        [Display(Name = "ارسال پیام ادمین")]
        public bool AdminNotification { get; set; }


        public bool CallbackSeen { get; set; }
        public bool CallbackFailedBank { get; set; }

        [Required]
        [Display(Name = "منقضی")]
        public bool IsExpire { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "مدیر")]
        public string AminUserId { get; set; }
        public ApplicationUser AdminUser { get; set; }



        [Required]
        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        [Required]
        [Display(Name = "مشاهده شده؟")]
        public bool Visited { get; set; }


        [Required]
        [Display(Name = "جدید؟")]
        public bool New { get; set; }


        [Display(Name = "تخفیف")]
        public int? OfferId { get; set; }
        public Offer Offer { get; set; }

        [Display(Name = "واحد پولی")]
        public short CurrencyId { get; set; }

        [Index("IX_BankOrderId", IsClustered = false, IsUnique = true)]
        public long BankOrderId { get; set; }
        public string CustomerOrderId { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public DateTime ExpireDate { get; set; }

        public bool IsOld { get; set; }

        [Display(Name = "ارسال sms لغو ؟")]
        public bool CancelingSms { get; set; }

        [Display(Name = "یادآوری نظرسنجی")]
        public OrderRateReminder? orderRateReminder { get; set; }

        [Display(Name = "تاریخ ثبت یادآوری")]
        public DateTime? orderRateReminderDate { get; set; }
        public PayUnSuccessfulReason? payUnSuccessfulReason  { get; set; }


        public string TorobStatus { get; set; }
        public string TorobPaymentToken { get; set; }
        public string DgpayStatus { get; set; }
        public string DgpayPaymentToken { get; set; }
        public string DgpaytrackingCode { get; set; }
        public string DgpayRefundtrackingCode { get; set; }
        public long Dgpayamount { get; set; }
        public int Dgpaytype { get; set; }
        public string Dgpayresult { get; set; }
        public string DgpaypaymentGateway{ get; set; }
        public string DgpayJson{ get; set; }

        public int? Commision { get; set; }

        public Guid? OrderId { get; set; }
        public virtual Order ParentOrder { get; set; }
        public virtual ICollection<Order> ChildOrders { get; set; }

        public int? UserActivityRefererId { get; set; }

        public UserActivityReferer UserActivityReferer { get; set; }


        public Guid? UserPayAttach { get; set; }
        public attachment UserPayAttachment { get; set; }


        public Guid? AdminPayAttach { get; set; }
        public attachment AdminPayAttachment { get; set; }

        public ICollection<OrderWallet> OrderWallets { get; set; }
        public ICollection<OrderAttributeOrder> OrderAttributeSelects { get; set; }
        /// <summary>
        /// وضعیت سفارش چیست
        /// </summary>
        public ICollection<OrderState> OrderStates { get; set; }
        public ICollection<OrderRow> OrderRows { get; set; }
        public ICollection<UserBon> UserBons { get; set; }
        public ICollection<UserCodeGiftLog> UserCodeGiftLogs { get; set; }
        public ICollection<GeneralCodeGiftLog> GeneralCodeGiftLogs { get; set; }
        public ICollection<UserBonLog> UserBonLogs { get; set; }
        public ICollection<OrderDelivery> OrderDeliveries { get; set; }
        public ICollection<OrderRate> OrderRates { get; set; }
        public ICollection<CreateOrderKey> CreateOrderKeys { get; set; }
        public ICollection<UserCodeGift> UserCodeGifts { get; set; }
        public ICollection<UserOfferMessageMember> UserOfferMessageMembers { get; set; }
        public ICollection<Domain.ProductGiftPackageOrder> ProductGiftPackageOrders { get; set; }

        #endregion
    }

    public enum PayUnSuccessfulReason
    {

        [Display(Name = "مشکل کارت اعتباری و رمز دوم")]
        CodeCardError,
        [Display(Name = "خطا در درگاه اینترنتی بانک")]
        TerminalError,
        [Display(Name = "از خرید منصرف شدم")]
        CancelOrderError
    }

    public enum OrderRateReminder
    {

        [Display(Name = "بار_اول")]
        بار_اول,
        [Display(Name = "بار_دوم")]
        بار_دوم,
        [Display(Name = "بار_سوم")]
        بار_سوم
    }
}
