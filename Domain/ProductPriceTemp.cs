using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductPriceTemp
    {
        public ProductPriceTemp()
        {

        }

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public int prpriceid { get; set; }
        public DateTime ExpireDate { get; set; }

        public bool Compelete { get; set; }

        #endregion
    }
}
