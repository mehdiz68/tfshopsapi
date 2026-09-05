using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class FilterShowViewModel
    {
        public string type { get; set; }
        public string title { get; set; }
        public int? attrId { get; set; }
        public long? min { get; set; }
        public long? max { get; set; }
        public List<FilterShowItems> items { get; set; }
    }
    public class FilterShowItems
    {
        public int id { get; set; }
        public string name { get; set; }
        public string enname { get; set; }
        public string ColorCode { get; set; }
        public bool? checkedd { get; set; }
    }

   public class BrandShowViewModel
    {
        public int Id { get; set; }
        public string PersianName { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
