using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class RedirectUrl : Object
    {
        #region Ctor
        public RedirectUrl()
        {

        }
        #endregion
      
        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

       
        /*
        * 0- direct
        * 1- content 
        * 2- category
        * 3- tag
        * 4- Product Category
        * 5- Product Filter
        * 6- Brand
        * 7- Product
        */
        [Display(Name = "نوع لینک مبدا")]
        public int SourceTypeId { get; set; }

        [Display(Name = "لینک مبدا")]
        public int SourceLinkId { get; set; }

        [Display(Name = "نوع لینک مقصد")]
        public int DestinationTypeId { get; set; }

        [Display(Name = "لینک مقصد")]
        public int DestinationLinkId { get; set; }

        public string Title { get; set; }
        public string SourceUrl { get; set; }
        public string DestinationUrl { get; set; }

        [Required]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        #endregion
    }
}
