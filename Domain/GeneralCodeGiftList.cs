using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class GeneralCodeGiftList : Object
    {
        #region Ctor
        public GeneralCodeGiftList()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<GeneralCodeGiftList>
        {
            public Configuration()
            {
                HasRequired(Current => Current.GeneralCodeGift).WithMany(Current => Current.GeneralCodeGiftLists).HasForeignKey(Current => Current.GeneralCodeGiftId).WillCascadeOnDelete(false);
            }
        }

        #endregion


        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "کد تخفیف عمومی")]
        public int GeneralCodeGiftId { get; set; }

        public GeneralCodeGift GeneralCodeGift { get; set; }

      
        [Required]
        [Display(Name = "کد تخفیف")]
        public string Code { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public ICollection<GeneralCodeGiftLog> GeneralCodeGiftLogs { get; set; }

        #endregion
    }

 
}
