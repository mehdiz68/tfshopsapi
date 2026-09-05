using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OrderDelivery : Object
    {
        #region Ctor
        public OrderDelivery()
        {
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderDelivery>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ProductSendWay).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.SendWayId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.UserAddress).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.UserAddressId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Order).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductSendWayWorkTime).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.ProductSendWayWorkTimeId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.AdminSelectedSendway).WithMany(Current => Current.OrderDeliverie2s).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Seller).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.SellerId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.SendwayBox).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.SendwayBoxId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "سفارش")]
        public Guid? OrderId { get; set; }
        public  Order Order { get; set; }

        [Display(Name = "تاریخ ارسال مرسوله به مشتری")]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "زمان ارسال مرسوله به مشتری")]
        public int? ProductSendWayWorkTimeId { get; set; }


        [Display(Name = "تاریخ احتمالی ارسال مرسوله به مشتری")]
        public DateTime? PredictDate { get; set; }

        [Display(Name ="وضعیت تحویل")]
        public DeliveryState? DeliveryState { get; set; }


        [Display(Name = "آدرس")]
        public int? UserAddressId { get; set; }
        public  UserAddress UserAddress { get; set; }

        public string Address { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public string FooterGoogleMapLatitude { get; set; }
        public string Provience { get; set; }
        public string LandlinePhone { get; set; }
        public string City { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string PostalCode { get; set; }
        public string nationalCode { get; set; }

        [Display(Name = "روش ارسال")]
        public int? SendWayId { get; set; }
        public  ProductSendWay ProductSendWay { get; set; }

        [Display(Name = "فروشنده(انبار) تحویل")]
        public int? SellerId { get; set; }
        public Seller Seller { get; set; }

        [Display(Name = "زمان تحویل")]
        public string SellerTime { get; set; }


        [Display(Name = "ماهیت باکس")]
        public ProductPackageType ProductPackageType { get; set; }

        [Display(Name = "عنوان باکس")]
        public int? SendwayBoxId { get; set; }
        public SendwayBox SendwayBox { get; set; }

        public bool Insurance { get; set; }
        public bool DeliveryPos { get; set; }
        public int? ProductSendWayId { get; set; }
        public ProductSendWay AdminSelectedSendway { get; set; }
        public string AdminSelectedTrackCode { get; set; }

        public  ProductSendWayWorkTime ProductSendWayWorkTime { get; set; }
        public  ICollection<OrderRow> OrderRows { get; set; }
        public  ICollection<OrderAttributeOrder> OrderAttributeOrders { get; set; }
        public  ICollection<WalletAttributeWallet> WalletAttributeWallets { get; set; }
        public  ICollection<OrderState> OrderStates { get; set; }
        public  ICollection<labelIcon> labelIcons { get; set; }

        #endregion
    }

    public enum DeliveryState
    {
        تحویل_به_موقع,
        تحویل_با_تاخیر
    }
}
