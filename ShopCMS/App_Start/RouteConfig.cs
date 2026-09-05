using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TfShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {


            routes.IgnoreRoute("elmah");
            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
            new { botdetect = @"(.*) BotDetectCaptcha\.ashx" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(name: "file_details", url: "Tf-products/{f}/{w}/{h}/{q}", defaults: new { controller = "Thumbnail", action = "Index",f=UrlParameter.Optional, w = UrlParameter.Optional, h = UrlParameter.Optional, q = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Old-UploadFiles", url: "uploadfiles/{title}", defaults: new { controller = "OldRedirect", action = "Index", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Old-contact", url: "contact-us", defaults: new { controller = "OldRedirect", action = "contact", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Old-contact2", url: "_تماس-با-ما_18.aspx", defaults: new { controller = "OldRedirect", action = "contact", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Old-contact3", url: "UserPages-U9-تماس-با-ما", defaults: new { controller = "OldRedirect", action = "contact", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Old-contact4", url: "_تی-اف-کاپ-چیست_90.aspx", defaults: new { controller = "OldRedirect", action = "contact", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Base-contact", url: "Base", defaults: new { controller = "OldRedirect", action = "home", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "ss-contact", url: "search.aspx", defaults: new { controller = "OldRedirect", action = "searchss", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "SSC-contact", url: "SSC_{id}_{title}.aspx", defaults: new { controller = "OldRedirect", action = "searchssc", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "SSC-contact2", url: "SSC_{title}_{id}.aspx", defaults: new { controller = "OldRedirect", action = "searchssc", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });

            routes.MapRoute(name: "Product_details", url: "TFP/{id}/{name}", defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Product_details222", url: "TFP2/{id}/{name}", defaults: new { controller = "Product2", action = "Index", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Product_details-Old", url: "TFSP-{Oldname}", defaults: new { controller = "Product", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Product_details-Old3", url: "P_{id}_s{title}.aspx", defaults: new { controller = "Product", action = "OldIndex2", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Product_details-Old4", url: "P_{id}_c{title}.aspx", defaults: new { controller = "Product", action = "OldIndex2", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Product_details-Old5", url: "P_{id}_m{title}.aspx", defaults: new { controller = "Product", action = "OldIndex2", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Product_details-Old2", url: "P_{id}.aspx", defaults: new { controller = "Product", action = "OldIndex2", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "ProductComment_details", url: "TFPComment/{id}", defaults: new { controller = "Product", action = "Comment", id = UrlParameter.Optional, name = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old", url: "Group-{Oldname}", defaults: new { controller = "ProductSearch", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old7", url: "Groups-{Oldname}", defaults: new { controller = "ProductSearch", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old2", url: "SubGroup-{Oldname}", defaults: new { controller = "ProductSearch", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old4", url: "SubGroups-{Oldname}", defaults: new { controller = "ProductSearch", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old3", url: "Products-{Oldname}-{num}", defaults: new { controller = "ProductSearch", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old222", url: "Group-{Oldname}", defaults: new { controller = "ProductSearch", action = "OldIndex", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Category-Old8", url: "TFS/Index/{id}/{title}", defaults: new { controller = "ProductSearch", action = "OldIndex2", Oldname = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "contentTag_details", url: "contentTag/{contentTypeId}/{id}/{title}", defaults: new { controller = "contentTag", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Blog_details", url: "blog", defaults: new { controller = "contentType", action = "Index", id = 3 }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "news_details", url: "news", defaults: new { controller = "contentType", action = "Index", id = 1 }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Pages_details", url: "pages", defaults: new { controller = "contentType", action = "Index", id = 0 }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Videos_details", url: "videos", defaults: new { controller = "AllVideo", action = "Index", id = 5 }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "events_details", url: "events", defaults: new { controller = "contentType", action = "Index", id = 6 }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "contentType_details", url: "contentType/{id}/{title}", defaults: new { controller = "contentType", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "content_details", url: "content/{id}/{title}", defaults: new { controller = "content", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "content_details_blog", url: "blog/{id}/{title}", defaults: new { controller = "blog", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "content_details4", url: "TFBlog/Index", defaults: new { controller = "contentType", action = "Index", id = 3 }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "content_details2", url: "TFBlog/Index/{id}", defaults: new { controller = "content", action = "Index", title = "asd" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "content_details3", url: "TFBlog/Post/{id}", defaults: new { controller = "content", action = "Index", title = "asd" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "video_details", url: "Video/{id}/{title}", defaults: new { controller = "video", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Page_details", url: "Page/{id}/{title}", defaults: new { controller = "Page", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "category_details", url: "Category/{id}/{title}", defaults: new { controller = "category", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Videocategory_details", url: "VideoCategory/{id}/{title}", defaults: new { controller = "VideoCategory", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "tag_details", url: "Tag/{id}/{title}", defaults: new { controller = "tag", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "ProductCategory_details", url: "TFC/{id}/{title}", defaults: new { controller = "ProductCategory", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "ProductSearch_details", url: "TFS/{id}/{title}", defaults: new { controller = "ProductSearch", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_Old", url: "GroupBrand-{BName}-BS1-G{CatId}-{num}", defaults: new { controller = "Brand", action = "OldIndex", id = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_Old4", url: "GroupBrands-{BName}-BS1-G{CatId}-{num}", defaults: new { controller = "Brand", action = "OldIndex", id = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_Old1", url: "SubGroup2Brands-{BName}-BS1-G{CatId}-{num}", defaults: new { controller = "Brand", action = "OldIndex", id = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_Old2", url: "SubGroup2Brand-{BName}-BS1-G{CatId}-{num}", defaults: new { controller = "Brand", action = "OldIndex", id = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_Old3", url: "SubGroupBrands-{BName}-BS1-G{CatId}-{num}", defaults: new { controller = "Brand", action = "OldIndex", id = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_Old5", url: "SubGroupBrand-{BName}-BS1-G{CatId}-{num}", defaults: new { controller = "Brand", action = "OldIndex", id = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details", url: "TFB/{Id}/{title}/{CatId}", defaults: new { controller = "Brand", action = "Index", CatId = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Brand_details_oldd", url: "brand_{BName}_{CatId}.aspx", defaults: new { controller = "Brand", action = "OldIndex", CatId = UrlParameter.Optional, BName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "SearchResult_ProductCategory_details", url: "SearchResult/{id}/{title}/{page}", defaults: new { controller = "SearchResult", action = "Index", id = UrlParameter.Optional, title = UrlParameter.Optional, page = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Ads_details", url: "Ads/{id}", defaults: new { controller = "Ads", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "shortUrl_details", url: "r/{id}", defaults: new { controller = "r", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "PTag_details", url: "PTag/{id}/{title}", defaults: new { controller = "ProductTag", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "PTag_details2", url: "PTag/{tagName}", defaults: new { controller = "ProductTag", action = "OldIndex" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "POldTag_details", url: "ProductTags-Tag{tagName}", defaults: new { controller = "ProductTag", action = "OldIndex", tagName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "POldTag_details2", url: "ProductTag-Tag{tagName}", defaults: new { controller = "ProductTag", action = "OldIndex", tagName = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "404Page", url: "NotFound-404", defaults: new { controller = "Error", action = "Index" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "500Page", url: "Error-500", defaults: new { controller = "Error", action = "Index2" }, namespaces: new string[] { "TfShop.Controllers" });

            routes.MapRoute(name: "Compare_details", url: "Compare/TFP-{id1}", defaults: new { controller = "Compare", action = "Index", id1 = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Compare_detail2s", url: "Compare/TFP-{id1}/TFP-{id2}", defaults: new { controller = "Compare", action = "Index", id1 = UrlParameter.Optional, id2 = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Compare_detail3s", url: "Compare/TFP-{id1}/TFP-{id2}/TFP-{id3}", defaults: new { controller = "Compare", action = "Index", id1 = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "Compare_detail4s", url: "Compare/TFP-{id1}/TFP-{id2}/TFP-{id3}/TFP-{id4}", defaults: new { controller = "Compare", action = "Index", id1 = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional, id4 = UrlParameter.Optional }, namespaces: new string[] { "TfShop.Controllers" });


            routes.MapRoute(name: "TFMagcategory_details", url: "TFMag/category/{id}/{title}", defaults: new { controller = "TFMag", action = "category" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "TFMagVideocategory_details", url: "TFMag/videoCategory/{id}/{title}", defaults: new { controller = "TFMag", action = "VideoCategory" }, namespaces: new string[] { "TfShop.Controllers" });
            routes.MapRoute(name: "TFMagpodcastcategory_details", url: "TFMag/podcastCategory/{id}/{title}", defaults: new { controller = "TFMag", action = "podcastCategory" }, namespaces: new string[] { "TfShop.Controllers" });

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}/{title}/{page}",
              defaults: new { controller = "home", action = "Index", id = UrlParameter.Optional, title = UrlParameter.Optional, page = UrlParameter.Optional },
              namespaces: new string[] { "TfShop.Controllers" }
          );
        }
    }
}
