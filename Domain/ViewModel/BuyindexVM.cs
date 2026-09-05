using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BuyindexVM
    {
        public BuyindexVM()
        {

        }
        public string SendWayTitle { get; set; }
        public string CustomerOrderId { get; set; }
        public long Price { get; set; }
        public string Cost { get; set; }
        public string Fullname { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public long BankOrderId { get; set; }
        public string UserPayAttachment { get; set; }
        public string UserPayAttachmentTitle { get; set; }
        public DateTime ExpireDate { get; set; }
        public short BankId { get; set; }
        public IEnumerable<ProductDetailItem> Products { get; set; }
        public IEnumerable<orderRowMin> OrderRows { get; set; }
    }
    public class orderRowMin
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
    }
}

