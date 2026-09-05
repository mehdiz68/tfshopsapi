using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
   public class OrderRateVM
    {
        public string OrderRateTitle { get; set; }
        public Order order { get; set; }
        public IEnumerable<ProductAdvantageTitle> ProductAdvantageTitless { get; set; }
        public IEnumerable<ProductDisAdvantageTitle> ProductDisAdvantageTitles { get; set; }
        public IEnumerable<OrderRateItem> orderRateItems { get; set; }
        public IEnumerable<ViewModels.ProductItemRanks> products { get; set; }
        public int productCount { get; set; }
        public int productRemainCount { get; set; }
    }
}
