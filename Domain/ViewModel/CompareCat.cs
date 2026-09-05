using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class CompareCat
    {
        public CompareCat()
        {

        }

        public IEnumerable<ProductItem> productItems { get; set; }
        public IEnumerable<Brand> brands  { get; set; }

    }

}
