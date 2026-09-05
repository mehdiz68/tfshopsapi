using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketStep2
    {
        public BasketStep2()
        {

        }
        public UserAddress UserDefaultAddress { get; set; }
        public List<UserAddress> UserAddresses { get; set; }
        public List<Seller> sellers { get; set; }

        public BasketStep1 basketStep1 { get; set; }
        public IEnumerable<ProductBasketItem> ProductBasketItems { get; set; }


        public long DeliveryPosPriceCost { get; set; }
        public string UserCodeGift { get; set; }
        public long OffPrice { get; set; }
        public CodeGiftType OffPriceType { get; set; }
    }
    public class BasketStep2VM
    {
        public BasketStep2VM()
        {

        }
        public UserAddressVM UserDefaultAddress { get; set; }
        public List<UserAddressVM> UserAddresses { get; set; }
        public List<SellerVM> sellers { get; set; }

        public BasketStep1V2 basketStep1 { get; set; }
        public IEnumerable<ProductBasketItemV2> ProductBasketItems { get; set; }


        public long DeliveryPosPriceCost { get; set; }
        public string UserCodeGift { get; set; }
        public long OffPrice { get; set; }
        public CodeGiftType OffPriceType { get; set; }
    }
    public class SellerVM
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }
    }
    public class UserAddressVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string LandlinePhone { get; set; }
        public string PostalCode { get; set; }
        public int? CityId { get; set; }
        public string CityEntity { get; set; }
        // Razor (_Step2.cshtml) هم اسم استان (CityEntity.Province.Name) رو جدا از اسم شهر نشون می‌ده
        public string ProvinceName { get; set; }
        public string Address { get; set; }
        public string AddressNumber { get; set; }

        public string AddressUnit { get; set; }
        public string FooterGoogleMapLongitude { get; set; }
        public string FooterGoogleMapLatitude { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }

    }
}
