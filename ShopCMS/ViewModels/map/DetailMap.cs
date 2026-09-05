using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TfShop.ViewModels.map
{
    public class DetailMap
    {
        public DetailMap()
        {

        }
        #region Properties

        public string id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public bool isselected { get; set; }
        #endregion

    }

}