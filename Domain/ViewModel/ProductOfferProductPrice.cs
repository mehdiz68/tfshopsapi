using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;
using Domain.ViewModel;

namespace Domain.ViewModels
{
    public class ProductOfferProductPrice
    {
        #region Ctor
        public ProductOfferProductPrice()
        {

        }
        #endregion

        #region Properties
        public int ProductPriceId { get; set; }
        public int ProductOfferId { get; set; }
        #endregion
    }
}
