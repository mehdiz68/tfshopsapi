using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class OrderDetailV2
    {
        public Guid Id { get; set; }
        public int? Commision { get; set; }
        public long BankOrderId { get; set; }
        public string CustomerOrderId { get; set; }
        public string InsertDate { get; set; }
        public List<OrderStateDetailV2> Orderstates { get; set; }
        public List<OrderDeliveryDetailV2> OrderDeliveries { get; set; }
        // معادل حلقه‌ی Razor روی Model.ChildOrders.FirstOrDefault() (سفارش‌های زیرمجموعه‌ی این سفارش،
        // زنجیره‌ای نه درختی) — چون توی Detail.cshtml این ردیف‌ها توی لیست محصولات هر مرسوله تکرار
        // می‌شن، این‌جا یه لیست تخت و مستقل از مرسوله نگه می‌داریم.
        public List<OrderRowsDetailV2> ChildOrderRows { get; set; }
        public List<OrderAttributeSelectDetailV2> OrderAttributeSelects { get; set; }
        public List<OrderV2> ChildOrders { get; set; }
        public int? BankId { get; set; }
        public long Price { get; set; }
        // آیا سفارش هنوز در حالت «استعلام» (در انتظار تایید انبار/حسابداری) هست؛ برای بنر شمارش‌معکوس بالای صفحه
        public bool IsEstelam { get; set; }
        public DateTime? EstelamDeadlineRaw { get; set; }
    }
    public class OrderStateDetailV2
    {
        public int Id { get; set; }
        public OrderStatus state { get; set; }
        public string LogDate { get; set; }
    }
    public class OrderDeliveryDetailV2
    {
        public int Id { get; set; }
        public int? SendWayId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Address { get; set; }
        public string AddressUnit { get; set; }
        public string AddressNumber { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? PredictDate { get; set; }
        public int AdminSelectedSendwayId { get; set; }
        public string AdminSelectedSendwayTitle { get; set; }
        public string AdminSelectedSendwayTrackingUrl { get; set; }
        public string AdminSelectedSendwayTrackingCode { get; set; }
        // وقتی ادمین دستی روش ارسال دیگه‌ای انتخاب نکرده، همین اطلاعات روش ارسال پیش‌فرض کالا نمایش داده می‌شه.
        public string ProductSendWayTitle { get; set; }
        public string ProductSendWayTrackingUrl { get; set; }
        public ProductSendWayWorkTimeDetailV2 ProductSendWayWorkTime { get; set; }
        public List<OrderStateDetailV2> Orderstates { get; set; }
        public List<WalletAttributeWalletDetailV2> WalletAttributeWalles { get; set; }
        public List<OrderRowsDetailV2> OrderRows { get; set; }

    }
    public class ProductSendWayWorkTimeDetailV2
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
    public class WalletAttributeWalletDetailV2
    {
        public int DataType { get; set; }
    }
    public class OrderAttributeSelectDetailV2
    {
        public int DataType { get; set; }
        public string Value { get; set; }
        public int? OrderDeliveryId { get; set; }
    }
    public class OrderRowsDetailV2
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string PageAddress { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
        public bool Disabled { get; set; }
        public int? ProductAttributeSelectModelId { get; set; }
        public string ProductAttributeSelectModel { get; set; }
        public int? ProductAttributeSelectSizeId { get; set; }
        public string ProductAttributeSelectSize { get; set; }
        public string Unit { get; set; }
        public int? ProductAttributeSelectColorId { get; set; }
        public string ProductAttributeSelectColor { get; set; }
        public string ImageFileName { get; set; }
    }


}
