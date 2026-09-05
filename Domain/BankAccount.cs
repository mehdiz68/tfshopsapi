using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BankAccount
    {
        public BankAccount()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<BankAccount>
        {
            public Configuration()
            {
                //HasOptional(Current => Current.ProductSendWay).WithMany(Current => Current.BankAccounts).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Image).WithMany(Current => Current.BankAccounts).HasForeignKey(Current => Current.AttachementId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.CardImage).WithMany(Current => Current.BankAccount2s).HasForeignKey(Current => Current.CardAttachementId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Imagedgpay).WithMany(Current => Current.BankAccount3s).HasForeignKey(Current => Current.AttachementIddgpay).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Imagetorob).WithMany(Current => Current.BankAccount4s).HasForeignKey(Current => Current.AttachementIdtorob).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Imagecard).WithMany(Current => Current.BankAccount5s).HasForeignKey(Current => Current.AttachementIdcard).WillCascadeOnDelete(false);


            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "مشخصات صاحب حساب")]
        public string AccountName { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Display(Name = "توضیحات")]
        public string Descriptiondgpay { get; set; }
        [Display(Name = "توضیحات")]
        public string Descriptiontorob { get; set; }
        [Display(Name = "توضیحات")]
        public string Descriptioncard { get; set; }

        [Display(Name = "عنوان درگاه")]
        public string Title{ get; set; }

        [Display(Name = "عنوان دیجی پی")]
        public string Titledigipay{ get; set; }

        [Display(Name = "عنوان ترب")]
        public string Titletorob{ get; set; }

        [Display(Name = "عنوان کارت به کارت")]
        public string Titlecard{ get; set; }

        [Required]
        [Display(Name = "شماره کارت")]
        public string CardNumber { get; set; }


        [Required]
        [Display(Name = "شماره حساب")]
        public string AccountNumber { get; set; }

        [Display(Name = "شماره شبا")]
        public string AccountSHBN{ get; set; }

        [Required]
        [Display(Name = "پرداخت نقدی ( فیش بانکی )")]
        public bool HasFish { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت نقدی ( فیش بانکی ) ( ساعت)")]
        public int HasFishHours { get; set; }

        [Required]
        [Display(Name = "پرداخت کارت به کارت ( شتاب )")]
        public bool HasCardToCard { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش کارت به کارت ( ساعت)")]
        public int CardNumberHours { get; set; }


        [Required]
        [Display(Name = "پرداخت به پیک نقدی")]
        public bool HasCourierDeliveryCash { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت به پیک نقدی ( ساعت)")]
        public int HasCourierDeliveryCashHours { get; set; }

        [Required]
        [Display(Name = "پرداخت به پیک کارتخوان")]
        public bool HasCourierDeliveryPos { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت به پیک کارتخوان ( ساعت)")]
        public int HasCourierDeliveryPosHours { get; set; }


        [Display(Name = "هزینه ارسال کارتخوان")]
        public long DeliveryPosPrice { get; set; }

        [Display(Name = "کمترین مبلغ فاکتور برای کارتخوان")]
        public long DeliveryPosPriceMin { get; set; }
        [Display(Name = "بیشترین مبلغ فاکتور برای کارتخوان")]
        public long DeliveryPosPriceMax { get; set; }

        public  BankAccountOnlineInfo BankAccountOnlineInfo { get; set; }

        [Display(Name = "درگاه پرداخت آنلاین غیر مستقیم ( زرین پال )")]
        public string MerchantId { get; set; }

        [Display(Name = "صفحه Callback")]
        public string CallbackUrl { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت آنلاین ( ساعت)")]
        public int OnliePaymentHours { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplayOrder { get; set; }

        [Required]
        [Display(Name = "بانک")]
        public Int16 BankId { get; set; }

        [Display(Name = "بانک")]
        public Int16? OfflineBankId { get; set; }


        [Required]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        [Display(Name = "پرداخت ترب")]
        public bool torob { get; set; }
        [Display(Name = "torob_client_id")]
        public string torob_client_id { get; set; }
        [Display(Name = "torob_client_secret")]
        public string torob_client_secret { get; set; }
        [Display(Name = "torob_username")]
        public string torob_username { get; set; }
        [Display(Name = "torob_password")]
        public string torob_password { get; set; }
        [Display(Name = "torob_callback")]
        public string torob_callback { get; set; }
        [Display(Name = "زمان لغو خودکار به دقیقه")]
        public int torob_cancel_minutes { get; set; }

        [Display(Name = "پرداخت دیجی پِی")]
        public bool dgpay { get; set; }
        [Display(Name = "dgpay_client_id")]
        public string dgpay_client_id { get; set; }
        [Display(Name = "dgpay_client_secret")]
        public string dgpay_client_secret { get; set; }
        [Display(Name = "dgpay_username")]
        public string dgpay_username { get; set; }
        [Display(Name = "dgpay_password")]
        public string dgpay_password { get; set; }
        [Display(Name = "dgpay_callback")]
        public string dgpay_callback { get; set; }
        [Display(Name = "زمان لغو خودکار به دقیقه")]
        public int dgpay_cancel_minutes { get; set; }


        [Display(Name = "آیکن")]
        public Guid? AttachementId { get; set; }
        public attachment Image { get; set; }

        [Display(Name = "آیکن")]
        public Guid? AttachementIddgpay { get; set; }
        public attachment Imagedgpay { get; set; }

        [Display(Name = "آیکن")]
        public Guid? AttachementIdtorob { get; set; }
        public attachment Imagetorob { get; set; }

        [Display(Name = "آیکن")]
        public Guid? AttachementIdcard { get; set; }
        public attachment Imagecard { get; set; }


        [Display(Name = "تصویر کارت به کارت")]
        public Guid? CardAttachementId { get; set; }
        public attachment CardImage { get; set; }


        [Display(Name = "حداقل خرید")]
        public int MinBasketPrice { get; set; }

        [Display(Name = "حداکثر خرید")]
        public int MaxBasketPrice { get; set; }



        [Display(Name = "حداقل خرید")]
        public int MinBasketPricedgpay { get; set; }

        [Display(Name = "حداکثر خرید")]
        public int MaxBasketPricedgpay { get; set; }


        [Display(Name = "حداقل خرید")]
        public int MinBasketPricetorob { get; set; }

        [Display(Name = "حداکثر خرید")]
        public int MaxBasketPricetorob { get; set; }


        [Display(Name = "حداقل خرید")]
        public int MinBasketPricecard { get; set; }

        [Display(Name = "حداکثر خرید")]
        public int MaxBasketPricecard { get; set; }

        [Display(Name = "کمیسیون اقساطی")]
        public int Commision { get; set; }

        [Display(Name = "کمیسیون اقساطی")]
        public int Commisiondgpay { get; set; }

        [Display(Name = "کمیسیون اقساطی")]
        public int Commisiontorob { get; set; }

        [Display(Name = "کمیسیون اقساطی")]
        public int Commisioncard { get; set; }


        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        public  ICollection<Wallet> Wallets { get; set; }

        public ICollection<ProductSendWayBankSelect> ProductSendWayBankSelects { get; set; }

        // [Display(Name = "روش ارسال")]
        // public int? ProductSendWayId { get; set; }
        //public ProductSendWay ProductSendWay { get; set; }
        #endregion
    }
}
