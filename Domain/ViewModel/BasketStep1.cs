using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketStep1
    {
        public BasketStep1()
        {

        }
        public IEnumerable<ProductBasketItem> ProductBasketItems { get; set; }
        public List<ProductBox> ProductBoxes { get; set; }
        public List<Domain.Seller> Sellers { get; set; }
        public bool validBuyOneTimePerOffer { get; set; }
        public bool NotvalidBox { get; set; }
    }
    public class BasketStep1V2
    {
        public BasketStep1V2()
        {

        }
        public IEnumerable<ProductBasketItemV2> ProductBasketItems { get; set; }
        public List<ProductBoxV2> ProductBoxes { get; set; }
        public List<SellerVM> Sellers { get; set; }
        public bool validBuyOneTimePerOffer { get; set; }
        public bool NotvalidBox { get; set; }
    }
}
