using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class BrandFAQ : Object
    {
        #region Ctor
        public BrandFAQ()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<BrandFAQ>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Brand).WithMany(Current => Current.BrandFAQs).HasForeignKey(Current => Current.BrandId).WillCascadeOnDelete(true);
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
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        [Required]
        public int DisplaySort { get; set; }


        #endregion
    }
}
