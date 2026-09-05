using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace TfShop.ViewModels
{
    public class ShowSideBarProfileViewModel
    {
        public System.Guid? Avatar { get; set; }
        public Domain.attachment Avatarattachment { get; set; }
        public ApplicationUser AppUser { get; set; }
        public int MessageCout { get; set; }
    }
    public class ShowSideBarProfileViewModelV2
    {
        public System.Guid? Avatar { get; set; }
        public string Avatarattachment { get; set; }
        public string FirstName{ get; set; }
        public string LastName { get; set; }
        public int MessageCout { get; set; }
    }
}