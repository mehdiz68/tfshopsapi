using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ProductTagViewModel
    {
        public IEnumerable<ProductItem> Products { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public IEnumerable<Domain.TagFAQ> TagFAQs { get; set; }
    }
}
