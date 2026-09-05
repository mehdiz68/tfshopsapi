using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UrlShort : Object
    {
        #region Ctor
        public UrlShort()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Adveresting>
        {
            public Configuration()
            {
                HasRequired(Current => Current.attachment).WithMany(Current => Current.Adverestings).HasForeignKey(Current => Current.Cover);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]

        [Display(Name = "URL")]
        public string URL { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage ="اجباری")]
        //[RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "لینک درست نیست")]
        [Display(Name = "لینک")]
        public string Link { get; set; }

   
        public  IList<UrlShortLog> UrlShortLogs { get; set; }

      
        #endregion
    }
}
