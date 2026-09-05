using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TfShop.ViewModels.Api.HeaderVM
{
    public class payWM
    {
    }
    public class dgpay_token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string jti { get; set; }
    }
    public class dgpayticket
    {
        public dgpayticketresponse result { get; set; }
        public string ticket { get; set; }
        public string redirectUrl { get; set; }
        public short paymentGateway { get; set; }
    }
    public class dgpayticketresponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public string level { get; set; }
    }
    public class torob_token {
        public string access_token { get; set; }
    }
    public class torobEligible
    {
        public torobEligibleresponse response { get; set; }
        public bool successful { get; set; }
    }
    public class torobEligibleresponse
    {
        public bool eligible { get; set; }
        public string title_message { get; set; }
        public string description { get; set; }
    }
    public class cartList
    {
        public string cartId { get; set; }
        public int totalAmount { get; set; }
        public int taxAmount { get; set; }
        public int shippingAmount { get; set; }
        public bool isTaxIncluded { get; set; }
        public bool isShipmentIncluded { get; set; }
        public List<cartItem> cartItems { get; set; }
    }
    public class basketDetailsDto
    {
        public string basketId { get; set; }
        public List<paycartList> items { get; set; }
    }
    public class paycartList
    {
        public string sellerId { get; set; }
        public string supplierId { get; set; }
        public string productCode { get; set; }
        public string brand { get; set; }
        public int productType { get; set; }
        public int count { get; set; }
        public string categoryId { get; set; }
    }
    public class cartItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public int amount { get; set; }
        public string category { get; set; }
        public int commissionType { get; set; }
    }

    public class torobpayment
    {
        public torobpaymentresponse response { get; set; }
        public bool successful { get; set; }
    }
    public class torobpaymentresponse
    {
        public string paymentToken { get; set; }
        public string paymentPageUrl { get; set; }
    }
}