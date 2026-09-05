using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class UserAdminProfile
    {
        public UserAdminProfile()
        {

        }
        public IEnumerable<UserAddress> userAddresses { get; set; }
        public IEnumerable<UserCodeGift> UserCodeGifts { get; set; }
        public IEnumerable<Order> orders { get; set; }
        public IEnumerable<ProductComment> productComments { get; set; }
        public IEnumerable<ProductQuestion> productQuestions { get; set; }
        public IEnumerable<ProductFavorate> productFavorates { get; set; }
        public IEnumerable<ProductLetmeknow> productLetmeknows { get; set; }

    }

}
