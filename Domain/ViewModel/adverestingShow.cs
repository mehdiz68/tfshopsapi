using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class adverestingShow
    {
        public adverestingShow()
        {

        }
        public int AdverestingSizeId { get; set; }
        public int Position { get; set; }
        public bool TypeLink { get; set; }
        public int LinkId { get; set; }
        public int Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string attachmentFileName { get; set; }

    }

}
