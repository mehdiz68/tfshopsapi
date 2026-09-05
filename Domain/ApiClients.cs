using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ApiClients : Object
    {
        #region Ctor
        public ApiClients()
        {

        }
        #endregion


        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }


        [Display(Name = "عنوان app")]
        public string Name { get; set; }
       
        public string ClientId { get; set; }
        public string ClientSecretHash { get; set; }
        public bool IsActive { get; set; }

        public string Scopes { get; set; }
        public string AllowedIPs { get; set; }
        public int RateLimitPerMinute { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        #endregion
    }
}
