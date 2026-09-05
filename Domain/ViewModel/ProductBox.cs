using Domain;
using Domain.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class ProductBox
    {
        public ProductBox()
        {

        }
        public SendwayBox SendwayBox { get; set; }
        public List<SendWayBoxPrice> sendWayBoxPrices { get; set; }

        public List<int> ProductPriceIdList { get; set; }

    }
    public class SendWayBoxPrice
    {
        public int Id { get; set; }
        public ProductSendWay productSendWay { get; set; }
        //public SendwayBox sendwayBox { get; set; }
        public int cost { get; set; }
        public int InsuranceCost { get; set; }
        public int PackageCost { get; set; }
        public bool PasKeraye { get; set; }
        public bool IsDefault { get; set; }

        public List<DeliveryDateTime> deliveryDateTimes { get; set; }
        public string DeliverSelectDescr { get; set; }
        public bool DeliverSelectable { get; set; }
        public int BoxMass { get; set; }
        public bool FreeSend { get; set; }
        public bool show { get; set; }
        public int DisplaySort { get; set; }
        public int Extraprice2Cost { get; set; }

    }

    public class DeliveryDateTime
    {
        public int Id { get; set; }
        public bool offDay { get; set; }
        public string offDaytitle { get; set; }
        public DateTime dateTime { get; set; }
        public TimeSpan starttime { get; set; }
        public TimeSpan endtime { get; set; }
        public bool completionCapacity { get; set; }
    }

    public static class DateTimeHelper
    {
        public static PersianDayOfWeek PersionDayOfWeek(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return PersianDayOfWeek.Shanbe;
                case DayOfWeek.Sunday:
                    return PersianDayOfWeek.Yekshanbe;
                case DayOfWeek.Monday:
                    return PersianDayOfWeek.Doshanbe;
                case DayOfWeek.Tuesday:
                    return PersianDayOfWeek.Seshanbe;
                case DayOfWeek.Wednesday:
                    return PersianDayOfWeek.Charshanbe;
                case DayOfWeek.Thursday:
                    return PersianDayOfWeek.Panjshanbe;
                case DayOfWeek.Friday:
                    return PersianDayOfWeek.Jome;
                default:
                    throw new Exception();
            }
        }
        public enum PersianDayOfWeek
        {
            Shanbe = 0,
            Yekshanbe = 1,
            Doshanbe = 2,
            Seshanbe = 3,
            Charshanbe = 4,
            Panjshanbe = 5,
            Jome = 6
        }
    }

    public class ProductBoxV2
    {
        public ProductBoxV2()
        {

        }
        public SendwayBoxVM SendwayBox { get; set; }
        public List<SendWayBoxPriceVm> sendWayBoxPrices { get; set; }

        public List<int> ProductPriceIdList { get; set; }

    }
    public class SendwayBoxVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ProductPackageType { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Lenght { get; set; }
        public int ProductWeight { get; set; }
        public string Description { get; set; }
    }

    public class SendWayBoxPriceVm
    {
        public int Id { get; set; }
        public ProductSendWayVM productSendWay { get; set; }
        public int cost { get; set; }
        public int InsuranceCost { get; set; }
        public int PackageCost { get; set; }
        public bool PasKeraye { get; set; }
        public bool IsDefault { get; set; }

        public List<DeliveryDateTime> deliveryDateTimes { get; set; }
        public string DeliverSelectDescr { get; set; }
        public bool DeliverSelectable { get; set; }
        public int BoxMass { get; set; }
        public bool FreeSend { get; set; }
        public bool show { get; set; }
        public int DisplaySort { get; set; }
        public int Extraprice2Cost { get; set; }

    }
    public class ProductSendWayVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public int DeliveryStartDay { get; set; }
        public int DeliveryStartHour { get; set; }
        public int DeliveryHourDay { get; set; }
        public string Image { get; set; }
        public bool HasExtraPrice { get; set; }
        public bool HasExtraPrice2 { get; set; }
        public string ExtraPriceTitle { get; set; }
        public int ExtraPriceValue { get; set; }
        public bool IsFree { get; set; }
        public bool PasKeraye { get; set; }
        public bool InvoiceType { get; set; }
        public bool IsActive { get; set; }
        public bool DisableTehran { get; set; }
        public bool IsDefault { get; set; }
        public bool IsIrPost { get; set; }
        public bool DeliverSelectable { get; set; }
        public string DeliverSelectDescr { get; set; }
        public bool FreeOff { get; set; }
        public int DisplaySort { get; set; }
    }
}

