using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class userAdr
    {
        public userAdr()
        {

        }
        public string provience { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string fullname { get; set; }
        public string mobile { get; set; }
        public string postalcode { get; set; }
        public string LandlinePhone { get; set; }
        public string nationalCode { get; set; }
        public string FooterGoogleMapLongitude { get; set; }
        public string FooterGoogleMapLatitude { get; set; }

    }

}
