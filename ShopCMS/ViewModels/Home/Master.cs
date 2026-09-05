using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace TfShop.ViewModels.Home
{
    public class Master
    {
        public int BasketCount { get; set; }
        public BasketShort Baskets { get; set; }
        public List<Domain.Social> HeaderSocialNetworks { get; set; }
        public List<string> NewSearchLog { get; set; }
        public List<string> UserSearchLog { get; set; }

        public string HeaderLogo { get; set; }

        public string StaticContentUrl { get; set; }

        public List<DisplayMenu> HeaderMainMenu { get; set; }

        public List<DisplayMenu> FooterMainMenu { get; set; }

        public List<Domain.Content> Blogs { get; set; }

        public string WebSiteName { get; set; }

        public string WebSiteTitle { get; set; }

        public string AboutPageAbstract { get; set; }

        public bool PopUpActive { get; set; }

        public string PopUpMessage { get; set; }

        public bool PopUpType { get; set; }
        public IEnumerable<CoreLib.ViewModel.Xml.XContentType> ContentTypes { get; set; }

        public IEnumerable<Domain.ViewModels.adverestingShow> TopAds { get; set; }
        public IEnumerable<Domain.Adveresting> MenuAds { get; set; }
        public IEnumerable<Domain.ProductCategory> ProductCategories{ get; set; }
        public IEnumerable<Domain.ViewModels.Link> VideoCategories { get; set; }
        public IEnumerable<Domain.ViewModels.Link> Videos1 { get; set; }
        public IEnumerable<Domain.ViewModels.Link> Videos2 { get; set; }
        public IEnumerable<Domain.ViewModels.Link> Videos3 { get; set; }
        public IEnumerable<Domain.ViewModel.SuspendOrder> SuspendOrders { get; set; }
        public Link VideoCat1 { get; set; }
        public Link VideoCat2 { get; set; }
        public Link VideoCat3 { get; set; }

        public bool root { get; set; }
        public ApplicationUser User { get; set; }
    }


    public class DisplayMenu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string menuLink { get; set; }
        public string Link { get; set; }
        public int PlaceShow { get; set; }
        public int DisplayOrder { get; set; }
        public string  icon { get; set; }
        public string Cover { get; set; }
        public bool IsRoot { get; set; }
        public int parentId { get; set; }

        public List<DisplayMenu> ChildMenu;
    }


    public class NewsLetter
    {
        [Required(ErrorMessage = "وارد کردن ایمیل ، اجباری است")]
        [Display(Name = " ایمیل")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کاراکتر ، 255")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "آدرس ایمیل وارد نمایید")]
        public string Email { get; set; }
    }

    public class HomePage
    {

        public IEnumerable<Domain.Slider> Sliders { get; set; }
        public List<HighSaleCategoryProduct> HighSaleCategoryProducts { get; set; }
        public List<NewCategoryProduct> NewCategoryProducts { get; set; }
        public IEnumerable<Domain.Brand> Brands { get; set; }
        public Domain.Brand TopBrand { get; set; }
        public IEnumerable<Domain.ProductCategory> ProductCategories { get; set; }        
        public Domain.ProductCategory TopCategoryCover { get; set; }
        public IEnumerable<ProductItem> ProductRecommends{ get; set; }
        public List<DisplayMenu> HeaderMainMenu { get; set; }
        public IEnumerable<Domain.ViewModels.adverestingShow> TopAdveresting { get; set; }
        public IEnumerable<Domain.ViewModels.adverestingShow> RightAdveresting { get; set; }
        public IEnumerable<Domain.ViewModels.adverestingShow> BottomAdveresting { get; set; }
        public IEnumerable<Domain.ViewModels.adverestingShow> LeftAdveresting { get; set; }
        public IEnumerable<AmazingOffers> AmazingOffers { get; set; }
    }
}