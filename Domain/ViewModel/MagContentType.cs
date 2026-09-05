using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class MagContentType
    {
        public MagContentType()
        {

        }
        public IEnumerable<Domain.ViewModels.adverestingShow> ads { get; set; }

        public IEnumerable<Content> contents { get; set; }

        public CoreLib.ViewModel.Xml.XContentType contentType { get; set; }

        public int contentCount { get; set; }
    }

}
