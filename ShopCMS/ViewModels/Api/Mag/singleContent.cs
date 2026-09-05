using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace TfShop.ViewModels.Api.Mag
{
    public class singleContent
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Tele { get; set; }
        public string Message { get; set; }
        public int ContentId { get; set; }
        public string addContactBtn { get; set; }
    }
    public class singleComment
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

        public int? ParrentId { get; set; }
        public int ContentId { get; set; }
        public int? CommentId { get; set; }
    }
}