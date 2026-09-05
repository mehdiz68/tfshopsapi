using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{

    public class ProfileOrderV2
    {
        public PagedList.IPagedList<OrderV2> AllOrders { get; set; }
        public PagedList.IPagedList<OrderV2> CurrentOrders { get; set; }
        public PagedList.IPagedList<OrderV2> ReturnedOrders { get; set; }
        public PagedList.IPagedList<OrderV2> CancelOrders { get; set; }
    }
    public class OrderV2
    {
        public Guid Id { get; set; }
        public long BankOrderId { get; set; }
        public string CustomerOrderId { get; set; }
        public string InsertDate { get; set; }
        public List<OrderStateV2> Orderstates { get; set; }
        public List<OrderDeliveryV2> OrderDeliveries { get; set; }
        public List<OrderV2> ChildOrders { get; set; }
        public int? BankId { get; set; }
        public long Price { get; set; }
        public bool HasRate { get; set; }
        public bool Isdelete { get; set; }
    }
    public class OrderStateV2
    {
        public int Id { get; set; }
        public OrderStatus state { get; set; }
        public string LogDate { get; set; }
        public DateTime LogDateRaw { get; set; }
    }
    public class OrderDeliveryV2
    {
        public int Id { get; set; }
        public List<WalletAttributeWalletV2> WalletAttributeWalles { get; set; }
        public List<OrderRowsV2> OrderRows { get; set; }
        public string SendWayTitle { get; set; }
        // تاریخ احتمالیِ ارسال (فرمت‌شده، مثلِ "دوشنبه ۷ مرداد") - وقتی بک‌اند مقداری براش ثبت نکرده
        // باشه null می‌مونه.
        public string PredictDate { get; set; }

    }
    public class WalletAttributeWalletV2
    {
        public int DataType { get; set; }
    }
    public class OrderAttributeSelectV2
    {
        public int DataType { get; set; }
        public string Value { get; set; }
    }
    public class OrderRowsV2
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string PageAddress { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
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
