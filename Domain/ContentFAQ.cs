using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ContentFAQ : Object
    {
        #region Ctor
        public ContentFAQ()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ContentFAQ>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithMany(Current => Current.ContentFAQs).HasForeignKey(Current => Current.ContentId).WillCascadeOnDelete(true);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "پرسش")]
        public string Question { get; set; }

        [Required]
        [Display(Name = "پاسخ")]
        public string Answer { get; set; }
    
        [Display(Name = "انتخاب محتوا")]
        public int ContentId { get; set; }
        public  Content Content { get; set; }

        [Required]
        public int DisplaySort { get; set; }


        #endregion
    }
}
