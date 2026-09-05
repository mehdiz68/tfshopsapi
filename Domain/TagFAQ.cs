using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class TagFAQ : Object
    {
        #region Ctor
        public TagFAQ()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TagFAQ>
        {
            public Configuration()
            {
                HasRequired(Current => Current.tag).WithMany(Current => Current.TagFAQs).HasForeignKey(Current => Current.TagId).WillCascadeOnDelete(true);
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
        public int TagId { get; set; }
        public Tag tag { get; set; }

        [Required]
        public int DisplaySort { get; set; }


        #endregion
    }
}
