using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class BlackListNumber : Object
    {
        #region Ctor
        public BlackListNumber()
        {

        }
        #endregion


        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public string PhoneNumber { get; set; }
        public bool SendSmsBlackList { get; set; }
        public string MessageBlackList { get; set; }
        #endregion
    }
}
