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
    public class customerOrderVW
    {

        #region Properties
        public string Id { get; set; }
        public string avatar { get; set; }
        public string phonenumber { get; set; }
        public string fullname { get; set; }
        public bool? gender { get; set; }
        public DateTime registration_date { get; set; }
        public string city { get; set; }
        public long? sumOrders { get; set; }
        public string blacklistTitle { get; set; }
        public bool blacklist { get; set; }
        public bool active { get; set; }
        #endregion
    }
}
