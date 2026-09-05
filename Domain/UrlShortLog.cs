using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UrlShortLog : Object
    {
        #region Ctor
        public UrlShortLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UrlShortLog>
        {
            public Configuration()
            {
                HasRequired(Current => Current.UrlShort).WithMany(Current => Current.UrlShortLogs).HasForeignKey(Current => Current.UrlShortId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }



        [Display(Name = "ClientIP")]
        public string ClientIP { get; set; }

        [Display(Name = "Browser")]
        public string Browser { get; set; }

        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }
        
        [Required]
        [Display(Name = "لیننک")]
        public int UrlShortId { get; set; }
        public  UrlShort UrlShort { get; set; }


        #endregion
    }
}
