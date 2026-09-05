using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductVideoGoogleList
    {
        #region Ctor
        public ProductVideoGoogleList()
        {

        }
        #endregion

        #region Properties
        public string @context { get; set; }
        public string @type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IEnumerable<string> thumbnailUrl { get; set; }
        public string uploadDate { get; set; }
        public string duration { get; set; }
        public string contentUrl { get; set; }
        public string embedUrl { get; set; }
        public interactionStatistic interactionStatistic { get; set; }

        #endregion
    }
}
public class interactionStatistic
{

    public string @type { get; set; }
    public interactionType interactionType { get; set; }
    public int userInteractionCount { get; set; }
}
public class interactionType
{

    public string @type { get; set; }
}
