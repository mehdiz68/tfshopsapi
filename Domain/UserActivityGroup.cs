using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserActivityGroup : Object
    {
        #region Ctor
        public UserActivityGroup()
        {

        }
        #endregion

      

        #region Properties

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام گروه")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Descr { get; set; }

        public ICollection<UserActivityGroupSelect> UserActivityGroupSelects { get; set; }
        public ICollection<SmartCampaign> SmartCampaigns { get; set; }

        #endregion
    }
}
