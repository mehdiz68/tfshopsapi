using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class TFMagVM
    {
        public TFMagVM()
        {

        }
        public IEnumerable<Content> BlogMain  { get; set; }
        public IEnumerable<Domain.ViewModels.adverestingShow> ads { get; set; }
        public IEnumerable<Content> ChiefEdiorContents { get; set; }
        public IEnumerable<Content> newBlog { get; set; }
        public IEnumerable<Content> HotBlog { get; set; }
        public IEnumerable<Content> Hotvideo { get; set; }
        public IEnumerable<Content> HotvideoPodcast { get; set; }

    }

}
