using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreLib;
using CoreLib.Infrastructure;
using CoreLib.Infrastructure.CustomAttribute;
using CoreLib.Infrastructure.ModelBinder;
using CoreLib.ViewModel.Xml;
using DataLayer;
using Domain;
using Domain.ViewModel;
using Domain.ViewModel.Site;
using Domain.ViewModels;
using Fasterflect;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Org.BouncyCastle.Crypto;
using PagedList;
using RestSharp.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TfShop.Areas.Admin.ViewModels.Report;
using TfShop.Infrastructure.AuthJTW;
using TfShop.Infrastructure.Filter;
using TfShop.Infrastructure.Helper;
using TfShop.Infrastructure.Security;
using TfShop.Models;
using TfShop.ViewModels;
using TfShop.ViewModels.Api.HeaderVM;
using TfShop.ViewModels.Api.HomePage;
using TfShop.ViewModels.Content;
using TfShop.ViewModels.Home;
using static Stimulsoft.Base.Drawing.Win32;
using static System.Net.WebRequestMethods;
using static TfShop.Controllers.ProfileController;

namespace TfShop.Controllers
{
    //[ApiClientAuthorize]
    //[ApiScope("orders.write")]
    public class ApiV1Controller : Controller
    {
        private TfShopDbContext context = null;
        private UnitOfWork.UnitOfWorkClass uow = null;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public string packagename = "";
        public ProductPackageType ProductPackageType;
        public string offexpiredate = "";
        public int? offerId = null;
        public string offtitle = "";
        public long cprice = 0;
        public long coffvalue = 0;
        public long coffvaluefinal = 0;
        public double ctaxtvalue = 0;
        public bool chasoff = false;
        public bool ProductOfferType = false;
        public int offerQuantity = 0;
        public int offerMaxQuantity = 0;
        public short cofftype = 3;
        public ApiV1Controller()
        {
            context = new TfShopDbContext();
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        public ApiV1Controller(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        #region Header

        [HttpGet]
        public JsonResult GetMeta(int id, int type)
        {
            Meta oMeta = null;
            var setting = GetSetting();
            switch (type)
            {
                //main page
                case 0:
                    oMeta = new Meta() { WebSiteMetaDescription = setting.WebSiteMetaDescription, WebSiteMetakeyword = setting.WebSiteMetakeyword, WebSiteTitle = setting.WebSiteTitle, CanocicalUrl = "https://www.tfshops.com", PageCover = setting.attachmentFileName, WebSiteName = setting.WebSiteName }; break;
                //mag index
                case -1:
                    oMeta = new Meta()
                    {
                        WebSiteMetaDescription = setting.WebSiteMetaDescriptionMag,
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = setting.WebSiteTitleMag,
                        StaticContentUrl = setting.StaticContentDomain,
                        Logo = setting.attachmentFileNameMag,
                        WebSiteName = setting.WebSiteNameMag,
                        CanocicalUrl = "https://www.tfshops.com/tfmag",
                        PageCover = setting.attachmentFileName
                    }; break;
                //mag search
                case -2:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "جستجو در وبلاگ، ویدیوها و پادکست های تی اف مگ.",
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = "جستجو در تی اف مگ",
                        Logo = setting.attachmentFileNameMag,
                        WebSiteName = setting.WebSiteNameMag,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/tfmag/search"
                    };
                    break;
                //compare
                case -3:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "مقایسه محصول",
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = "مقایسه محصول",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/compare"
                    };
                    break;
                //Shopping Search
                case -4:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "جستجوی بیشتر",
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = "جستجوی بیشتر",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/search"
                    };
                    break;
                case -5:
                    var superdealPage = context.Contents.Where(x => x.IsSuperDeal).Single();
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = superdealPage.Descr,
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = superdealPage.Title,
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/superdeal"
                    };
                    break;
                case -6:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "سبد خرید " + setting.WebSiteMetaDescription,
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = "سبد خرید",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/cart"
                    };
                    break;
                case -7:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "ادغام سفارش" + setting.WebSiteMetaDescription,
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = "ادغام سفارش",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/cart/mergeorder"
                    };
                    break;
                case -8:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "پروفایل کاربری",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "پروفایل کاربری",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile"
                    };
                    break;
                case -9:
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "تایید موبایل",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "تایید موبایل",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/verifyPhone"
                    };
                    break;
                case -10:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "تغییر رمز عبور",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "تغییر رمز عبور",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/changepassword"
                    };
                    break;
                case -11:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "ویرایش اطلاعات کاربری",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "ویرایش اطلاعات کاربری",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/edit"
                    };
                    break;
                case -12:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "لیست آدرس‌ها",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "لیست آدرس‌ها",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/addresses"
                    };
                    break;
                case -13:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "افزودن آدرس",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "افزودن آدرس",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/addaddress"
                    };
                    break;
                case -14:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "ویرایش آدرس",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "ویرایش آدرس",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/editaddress"
                    };
                    break;
                case -15:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "محصولات مورد علاقه",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "محصولات مورد علاقه",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/favorites"
                    };
                    break;
                case -16:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "اطلاع رسانی ها",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "اطلاع رسانی ها",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/notices"
                    };
                    break;
                case -17:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "لیست بن‌های تخفیف",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "لیست بن‌های تخفیف",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/giftbon"
                    };
                    break;
                case -18:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "لیست کدهای تخفیف",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "لیست کدهای تخفیف",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/giftcode"
                    };
                    break;
                case -19:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "لیست پیام ها",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "لیست پیام ها",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/messages"
                    };
                    break;
                case -20:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "لیست هدایای دریافتی",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "لیست هدایای دریافتی",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/ordergifts"
                    };
                    break;
                case -21:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "نظرات من",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "نظرات من",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/comments"
                    };
                    break;
                case -22:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "سفارشات من",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "سفارشات من",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/orders"
                    };
                    break;
                case -23:

                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = "جزئیات سفارش",
                        WebSiteMetakeyword = "",
                        WebSiteTitle = "جزئیات سفارش",
                        Logo = setting.attachmentFileName,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/profile/detail"
                    };
                    break;
                //content
                case 1:
                    DateTime dtime = DateTime.Now;
                    var content = context.Contents.AsQueryable().Include(x => x.attachment).AsNoTracking().Where(x => (x.CatId == null || x.Category.IsActive) && x.IsActive && x.Id == id && x.IsActive && (x.StartDate == null || x.StartDate <= dtime)).FirstOrDefault();
                    if (content != null)
                    {
                        oMeta = new Meta()
                        {
                            WebSiteMetaDescription = content.Descr,
                            WebSiteMetakeyword = setting.WebSiteMetakeyword,
                            WebSiteTitle = content.Title,
                            Logo = setting.attachmentFileNameMag,
                            CanocicalUrl = "https://www.tfshops.com/tfmag/" + (content.ContentTypeId == 3 ? "blog" : content.ContentTypeId == 5 ? "video" : "podcast") + "/" + content.Id + "/" + CommonFunctions.NormalizeAddress(content.PageAddress.ToLower()),
                            PageCover = content.attachment != null ? content.attachment.FileName : "default-thumbnail.jpg",
                            WebSiteName = setting.WebSiteNameMag,
                            StaticContentUrl = setting.StaticContentDomain,
                        };
                    }
                    break;
                //category
                case 2:
                    var category = context.Categories.AsQueryable().Include(x => x.attachment).AsNoTracking().Where(x => x.IsActive && x.Id == id).FirstOrDefault();
                    if (category != null)
                    {
                        oMeta = new Meta()
                        {
                            PageCover = category.attachment != null ? category.attachment.FileName : "default-thumbnail.jpg",
                            WebSiteName = setting.WebSiteNameMag,
                            WebSiteMetaDescription = category.Descr,
                            WebSiteMetakeyword = setting.WebSiteMetakeyword,
                            WebSiteTitle = category.Title,
                            Logo = setting.attachmentFileName,
                            StaticContentUrl = setting.StaticContentDomain,
                            CanocicalUrl = "https://www.tfshops.com/tfmag/" + (category.ContentTypeId == 3 ? "category" : category.ContentTypeId == 5 ? "videocategory" : "podcastCategory") + "/" + category.Id + "/" + CommonFunctions.NormalizeAddress(category.PageAddress.ToLower())
                        };
                    }
                    break;
                //content type
                case 3:
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    var contentType = readXML.ListOfXContentType().Where(x => x.LanguageId == 1 && x.Id == id).SingleOrDefault();
                    oMeta = new Meta()
                    {
                        PageCover = setting.attachmentFileName,
                        WebSiteMetaDescription = contentType.Abstract,
                        WebSiteMetakeyword = setting.WebSiteMetakeyword,
                        WebSiteTitle = contentType.Title,
                        Logo = setting.attachmentFileNameMag,
                        WebSiteName = setting.WebSiteName,
                        StaticContentUrl = setting.StaticContentDomain,
                        CanocicalUrl = "https://www.tfshops.com/tfmag/" + (id == 3 ? "blogs" : id == 5 ? "videos" : "podcasts")
                    };
                    break;
                //TFC
                case 4:
                    var tfc = context.ProductCategories.Include(x => x.attachment).AsQueryable().Where(x => x.Id == id).Select(x => new { x.Descr, x.Title, cover = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", x.Id, PageAddress = x.PageAddress.ToLower() }).SingleOrDefault();
                    if (tfc != null)
                        oMeta = new Meta() { WebSiteMetaDescription = tfc.Descr, WebSiteMetakeyword = setting.WebSiteMetakeyword, WebSiteTitle = tfc.Title, CanocicalUrl = string.Format("https://www.tfshops.com/tfc/{0}/{1}", tfc.Id, CommonFunctions.NormalizeAddress(tfc.PageAddress.ToLower())), PageCover = tfc.cover, WebSiteName = setting.WebSiteName };
                    break;
                //TFs
                case 5:
                    var tfs = context.ProductCategories.Include(x => x.attachment).AsQueryable().Where(x => x.Id == id).Select(x => new { x.Descr2, x.Title2, cover = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", x.Id, PageAddress2 = x.PageAddress2.ToLower() }).SingleOrDefault();
                    if (tfs != null)
                        oMeta = new Meta() { WebSiteMetaDescription = tfs.Descr2, WebSiteMetakeyword = setting.WebSiteMetakeyword, WebSiteTitle = tfs.Title2, CanocicalUrl = string.Format("https://www.tfshops.com/tfs/{0}/{1}", tfs.Id, CommonFunctions.NormalizeAddress(tfs.PageAddress2.ToLower())), PageCover = tfs.cover, WebSiteName = setting.WebSiteName };
                    break;
                //TFB
                case 6:
                    var brand = context.Brands.Include(x => x.attachment).AsQueryable().Where(x => x.Id == id).Select(x => new { x.MeteDescription, x.Title, cover = x.AttachementId.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", x.Id, x.Name }).SingleOrDefault();
                    if (brand != null)
                        oMeta = new Meta() { WebSiteMetaDescription = brand.MeteDescription, WebSiteMetakeyword = setting.WebSiteMetakeyword, WebSiteTitle = brand.Title, CanocicalUrl = string.Format("https://www.tfshops.com/tfb/{0}/{1}", brand.Id, CommonFunctions.NormalizeAddress(brand.Name.ToLower())), PageCover = brand.cover, WebSiteName = setting.WebSiteName };
                    break;
                //TFP
                case 7:
                    var tfp = context.Products.Include("ProductImages.Image").Include("ProductImages.ProductPrice").Include("ProductPrices.ProductImages.Image").AsQueryable().Where(x => x.Id == id).ToList().Select(x => new
                    {
                        x.Descr,
                        x.Title,
                        cover =
                        x.ProductPrices.Any(s => s.IsDefault && s.ProductImages.Any(a => a.IsMain)) ?
                        x.ProductImages.Where(s => s.IsMain && s.ProductPrice.IsDefault).First().Image.FileName :
                        x.ProductPrices.Any(s => s.ProductImages.Any(a => a.IsMain)) ?
                        x.ProductImages.Where(s => s.IsMain).First().Image.FileName :
                        x.ProductImages.First().Image.FileName,
                        x.Id,
                        PageAddress = x.PageAddress.ToLower()
                    }).SingleOrDefault();
                    //var tfp = context.Products.Include("ProductImages.Image").Include("ProductPrices.ProductImages.Image").AsQueryable().Where(x => x.Id == id).ToList().SingleOrDefault();
                    if (tfp != null)
                        oMeta = new Meta() { WebSiteMetaDescription = tfp.Descr, WebSiteMetakeyword = setting.WebSiteMetakeyword, WebSiteTitle = tfp.Title, CanocicalUrl = string.Format("https://www.tfshops.com/tfp/{0}/{1}", tfp.Id, CommonFunctions.NormalizeAddress(tfp.PageAddress.ToLower())), PageCover = "", WebSiteName = setting.WebSiteName };
                    break;
                //tag
                case 8:
                    var tag = context.Tags.Include(x => x.TagFAQs).AsQueryable().Where(x => x.Id == id).Select(x => new { x.Id, x.TagName, x.TagFAQs, x.MetaDescription, x.Title, x.Today }).SingleOrDefault();
                    string todayDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(DateTime.Now.Date);
                    string metaTitle = (!String.IsNullOrEmpty(tag.Title) ? tag.Title : tag.TagName) + (tag.Today ? " " + todayDate : "");
                    string metaDescription = !String.IsNullOrEmpty(tag.MetaDescription) ? tag.MetaDescription : "محصولاتی که برچسب " + tag.TagName + " را دارند در این صفحه لیست شده اند.";
                    if (tag != null)
                        oMeta = new Meta() { WebSiteMetaDescription = metaDescription, WebSiteMetakeyword = setting.WebSiteMetakeyword, WebSiteTitle = metaTitle, CanocicalUrl = string.Format("https://www.tfshops.com/ptag/{0}/{1}", tag.Id, CommonFunctions.NormalizeAddress(tag.TagName.ToLower())), PageCover = setting.attachmentFileName, WebSiteName = setting.WebSiteName };
                    break;
                default:
                    break;
            }
            try
            {
                if (oMeta == null)
                {
                    return Json(new
                    {
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    data = oMeta,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getmeta", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetLogo()
        {
            try
            {
                return Json(new
                {
                    data = context.Settings.AsNoTracking().Where(x => x.LanguageId == 1).Select(x => x.attachment.FileName).FirstOrDefault(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getlogo", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetMenu(int? which)
        {
            int w = which.HasValue ? which.Value : 1;
            try
            {
                List<DisplayMenu> DisplayMenus = new List<DisplayMenu>();
                foreach (var item in context.Menus.AsQueryable().AsNoTracking().Include("attachment").Where(x => x.IsActive == true && (x.PlaceShow == 3 || x.PlaceShow == w)).OrderBy(x => x.DisplaySort).ToList())
                {
                    DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                    dm.Id = item.Id;
                    if (item.Cover != null)
                        dm.Cover = item.attachment.FileName;
                    else
                        dm.Cover = null;
                    dm.icon = item.icon;
                    dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == 400 && item.TypeId == 11 ? true : false) : false);
                    dm.menuLink = item.FinalLink;
                    dm.PlaceShow = item.PlaceShow;
                    dm.DisplayOrder = item.DisplaySort;
                    dm.Title = item.Title;
                    dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                    DisplayMenus.Add(dm);
                }



                return Json(new
                {
                    data = DisplayMenus,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getmenu", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpGet]
        public JsonResult GetProductCategories()
        {

            try
            {
                return Json(new
                {
                    data = context.ProductCategories.AsQueryable().AsNoTracking().Include(c => c.Menuattachment).Include(c => c.MenuaIconttachmentMobile).Include(c => c.MenuattachmentMobile).Where(x => x.ParrentId == null && x.hidden == false && x.IsActive && x.LanguageId == 1).OrderBy(x => x.Sort).Select(x => new PrCatVM
                    {
                        Id = x.Id,
                        title = x.Title,
                        cover = x.MenuCover.HasValue ? x.Menuattachment.FileName : "",
                        mobileIcon = x.MenuIconMobile.HasValue ? x.MenuaIconttachmentMobile.FileName : "",
                        mobileCover = x.MenuCoverMobile.HasValue ? x.MenuattachmentMobile.FileName : "",
                        hidden = x.hidden,
                        name = x.Name,
                        pageAdress = x.PageAddress.ToLower(),
                        sort = x.Sort,
                        Child = x.ChildCategory.OrderBy(x2 => x2.Sort).Select(x2 => new PrCatVMChild
                        {
                            Id = x2.Id,
                            title = x2.Title,
                            cover = x2.MenuCover.HasValue ? x2.Menuattachment.FileName : "",
                            mobileIcon = x2.MenuIconMobile.HasValue ? x2.MenuaIconttachmentMobile.FileName : "",
                            mobileCover = x2.MenuCoverMobile.HasValue ? x2.MenuattachmentMobile.FileName : "",
                            hidden = x2.hidden,
                            name = x2.Name,
                            pageAdress = x2.PageAddress.ToLower(),
                            sort = x2.Sort,
                            Child = x2.ChildCategory.OrderBy(x3 => x3.Sort).Select(x3 => new PrCatVMSubChild { Id = x3.Id, title = x3.Title, cover = x3.MenuCover.HasValue ? x3.Menuattachment.FileName : "", mobileIcon = x3.MenuIconMobile.HasValue ? x3.MenuaIconttachmentMobile.FileName : "", mobileCover = x3.MenuCoverMobile.HasValue ? x3.MenuattachmentMobile.FileName : "", hidden = x3.hidden, name = x3.Name, pageAdress = x3.PageAddress.ToLower(), sort = x3.Sort }).ToList()
                        }).ToList()
                    }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductcategories", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetSocial()
        {
            try
            {
                return Json(new
                {
                    data = context.Socials.AsQueryable().Include(x => x.attachment).AsNoTracking().Where(x => x.LanguageId == 1 && x.IsActive).OrderBy(x => x.DisplaySort).Skip(() => 0).Take(() => 10).Select(x => new ViewModels.Api.HeaderVM.socialVM() { cover = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, socialLink = x.Link, icon = x.Icon, Id = x.Id }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getsocial", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetAds(int position, int typeid, int? linkid, DateTime? date, bool whichCover)
        {

            try
            {
                var ads = context.Adverestings.AsQueryable().AsNoTracking().Where(x => x.Position == position && x.TypeId == typeid && x.IsActive);
                if (linkid.HasValue)
                    ads = ads.Where(x => x.LinkId == linkid.Value);
                if (date.HasValue)
                    ads = ads.Where(x => ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null));
                return Json(new
                {
                    data = ads.OrderBy(r => r.DisplaySort).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = whichCover == true ? x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink, LinkId = x.LinkId }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getads", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public async Task<JsonResult> GetSearchArchive()
        {
            try
            {
                DateTime dt = DateTime.Now.AddDays(-15);
                return Json(new
                {
                    data = await context.SearchLogs.AsQueryable().AsNoTracking().Where(x => x.insertDate > dt).OrderByDescending(x => x.insertDate).GroupBy(x => x.keyword).Select(x => new { x.Key, c = x.Count() }).OrderByDescending(x => x.c).Select(x => x.Key).Skip(() => 0).Take(() => 5).ToListAsync(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "getsearcharchive", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [JWTAuthorize]
        public async Task<JsonResult> GetUserSearchArchive()
        {

            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var id = Authentication.ValidateToken(Token);
                if (id != null)
                {
                    DateTime dt = DateTime.Now.AddDays(-15);
                    DateTime nowdateTime = DateTime.Now;
                    return Json(new
                    {
                        data = await context.SearchLogs.AsQueryable().AsNoTracking().Where(x => x.UserId == id && x.insertDate > dt).OrderByDescending(x => x.insertDate).GroupBy(x => x.keyword).Select(x => new { x.Key, c = x.Count() }).OrderByDescending(x => x.c).Select(x => x.Key).Skip(() => 0).Take(() => 5).ToListAsync(),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getusersearcharchive", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [JWTAuthorize]
        public JsonResult GetSuspendOrders()
        {

            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var id = Authentication.ValidateToken(Token);
                if (id != null)
                {
                    DateTime nowdateTime = DateTime.Now;
                    return Json(new
                    {
                        data = context.Orders.AsQueryable().Include("OrderDeliveries").Include("OrderStates").Include("OrderRows").Include("OrderWallets.Wallet.BankAccount").AsNoTracking().Where(x => x.OrderWallets.Any(s => s.Wallet.PaymentType != 4 && s.Wallet.PaymentType != 5 && s.Wallet.PaymentType != 3) && x.UserId == id && x.IsOld == false && x.IsExpire == false && (x.ExpireDate != null && x.ExpireDate > nowdateTime) && !x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23)) && !x.OrderStates.Any(s => s.state >= Domain.OrderStatus.تایید_پرداخت)).ToList().Select(x => new Domain.ViewModel.SuspendOrder() { BankId = x.OrderWallets.First().Wallet.BankAccount.BankId, bankOrderid = x.BankOrderId, customerOrderid = x.CustomerOrderId, deliverId = x.OrderDeliveries.First().Id, price = x.OrderWallets.First().Wallet.Price }),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "getsuspendorders", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        [CorrectArabianLetter(new string[] { "keyword" })]
        public JsonResult MainSearch3(string keyword)
        {
            try
            {
                List<CoreLib.ViewModel.SearchParserString> keywords = new List<CoreLib.ViewModel.SearchParserString>();
                string[] characters = keyword.Split(null);
                if (characters.Length == 1)
                {
                    keywords.Add(new CoreLib.ViewModel.SearchParserString() { val = keyword.Trim(), wordCount = 1 });
                }
                else
                {

                    string temp = "";
                    int i = 1; ;
                    foreach (var item in characters)
                    {
                        if (i > 6)
                            continue;
                        if (!String.IsNullOrEmpty(item.Trim()) && item != "مدل" && item != "سایز" && item != "رنگ")
                        {
                            temp += item + " ";
                            i++;
                        }
                    }
                    keyword = temp;
                    keywords = CommonFunctions.ParseStringOwn(keyword);
                }

                var keywordList1 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementMainKeyword) LIKE N'%" + x.val + "%' OR LOWER(ElementAbstract) LIKE N'%" + x.val + "%' ").ToList();
                string wordlist1 = string.Join(" Or ", keywordList1);

                var keywordList2 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementMainKeyword) LIKE N'%" + x.val + "%'  ").ToList();
                string wordlist2 = string.Join(" Or ", keywordList2);

                var keywordList3 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementTitle) LIKE N'%" + x.val + "%' OR LOWER(model) LIKE N'%" + x.val + "%'  OR LOWER(size) LIKE N'%" + x.val + "%'  OR LOWER(color) LIKE N'%" + x.val + "%'  OR LOWER(garanty) LIKE N'%" + x.val + "%'  ").ToList();
                string wordlist3 = string.Join(" Or ", keywordList3);


                List<MainSearch> SearchLists = uow.SearchEngineFactRepository.Sql(x => new MainSearch(), "exec MainSearch3 @where1,@where2,@where3", new SqlParameter("@where1", wordlist1), new SqlParameter("@where2", wordlist2), new SqlParameter("@where3", wordlist3)).ToList();
                if (!SearchLists.Any())
                {

                    keywords = CommonFunctions.ParseString(keyword);


                    keywordList1 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementMainKeyword) LIKE N'%" + x.val + "%' OR LOWER(ElementAbstract) LIKE N'%" + x.val + "%' ").ToList();
                    wordlist1 = string.Join(" Or ", keywordList1);

                    keywordList2 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementMainKeyword) LIKE N'%" + x.val + "%' ").ToList();
                    wordlist2 = string.Join(" Or ", keywordList2);

                    SearchLists = uow.SearchEngineFactRepository.Sql(x => new MainSearch(), "exec MainSearch3 @where1,@where2,@where3", new SqlParameter("@where1", wordlist1), new SqlParameter("@where2", wordlist2), new SqlParameter("@where3", wordlist2)).ToList();

                }
                else if (!SearchLists.Any(s => s.TypeId == 3) || !SearchLists.Any(s => s.TypeId == 4))
                {
                    keywords = CommonFunctions.ParseString(keyword);


                    keywordList1 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementMainKeyword) LIKE N'%" + x.val + "%' OR LOWER(ElementAbstract) LIKE N'%" + x.val + "%' ").ToList();
                    wordlist1 = string.Join(" Or ", keywordList1);

                    keywordList2 = keywords.OrderBy(x => x.wordCount).Select(x => " LOWER(ElementMainKeyword) LIKE N'%" + x.val + "%' ").ToList();
                    wordlist2 = string.Join(" Or ", keywordList2);

                    SearchLists = uow.SearchEngineFactRepository.Sql(x => new MainSearch(), "exec MainSearch3 @where1,@where2,@where3", new SqlParameter("@where1", wordlist1), new SqlParameter("@where2", wordlist2), new SqlParameter("@where3", wordlist3)).ToList();

                }


                //log
                uow.SearchLogRepository.Insert(new SearchLog
                {
                    insertDate = DateTime.Now,
                    keyword = keyword.Trim(),
                    UserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null,
                    ClientIP = Request.UserHostAddress,
                    Browser = Request.Browser.Browser,
                    UserAgent = GetUserPlatform(Request)
                });
                uow.Save();

                return Json(new
                {
                    data = SearchLists,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "mainsearch3", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    data = "Exception Erorr",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Home
        [HttpGet]
        public JsonResult ShowMainSlider()
        {
            try
            {

                return Json(new
                {
                    data = context.SliderImages.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.Slider.IsActive && x.Slider.LinkId == 400).Select(x => new ViewModels.Api.HomePage.Master() { image = x.attachment.FileName, title = x.Title, link = x.Link }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showmainslider", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult ShowTopbrand(int? skip, int? take)
        {
            try
            {
                int s = skip.HasValue ? skip.Value : 0;
                int t = take.HasValue ? take.Value : 15;
                return Json(new
                {
                    data = context.Brands.AsQueryable().Include("attachmentHomePage").AsNoTracking().Where(x => x.IsShowHomePage == true && x.LanguageId == 1 && x.CoverHomePage.HasValue).OrderBy(aa => Guid.NewGuid()).Skip(() => s).Take(() => t).ToList().Select(x => new ViewModels.Api.HomePage.Master() { image = x.attachmentHomePage.FileName, title = x.Title, link = "/tfb/" + x.Id + "/" + CommonFunctions.NormalizeAddress(x.Name) }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showtopbrand", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult ShowTopbrands(int? skip, int? take)
        {
            try
            {
                int s = skip.HasValue ? skip.Value : 0;
                int t = take.HasValue ? take.Value : 15;
                return Json(new
                {
                    data = context.Brands.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.IsShowHomePage == false && x.LanguageId == 1 && x.AttachementId.HasValue).OrderBy(ss => Guid.NewGuid()).Skip(() => 0).Take(() => 15).ToList().Select(x => new ViewModels.Api.HomePage.Master() { image = x.attachment.FileName, title = x.Title, link = "/tfb/" + x.Id + "/" + CommonFunctions.NormalizeAddress(x.Name) }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showtopbrands", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult ShowTopCategory(int? skip, int? take)
        {
            try
            {

                int s = skip.HasValue ? skip.Value : 0;
                int t = take.HasValue ? take.Value : 4;
                return Json(new
                {
                    data = context.ProductCategories.AsQueryable().Include("attachmentHomePage").AsNoTracking().Where(x => x.hidden == false && x.IsShowHomePage == true && x.LanguageId == 1 && x.IsActive && x.CoverHomePage.HasValue && x.ParrentId != null).OrderBy(aa => Guid.NewGuid()).Skip(() => s).Take(() => t).ToList().Select(x => new ViewModels.Api.HomePage.Master() { image = x.attachmentHomePage.FileName, name = x.Name, title = x.Title, link = "/tfs/" + x.Id + "/" + CommonFunctions.NormalizeAddress(x.PageAddress2.ToLower()) }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showtopcategory", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpGet]
        public JsonResult ShowTopCategories(int? skip, int? take)
        {
            try
            {

                int s = skip.HasValue ? skip.Value : 0;
                int t = take.HasValue ? take.Value : 4;
                return Json(new
                {
                    data = context.ProductCategories.AsQueryable().Include("attachmentHomePage").AsNoTracking().Where(x => x.hidden == false && x.IsShowHomePage == false && x.LanguageId == 1 && x.IsActive && x.CoverHomePage.HasValue && x.ParrentId != null).OrderBy(aa => Guid.NewGuid()).Skip(() => s).Take(() => t).ToList().Select(x => new ViewModels.Api.HomePage.Master() { image = x.attachmentHomePage.FileName, name = x.Name, title = x.Title, link = "/tfs/" + x.Id + "/" + CommonFunctions.NormalizeAddress(x.PageAddress2.ToLower()) }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showtopcategories", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult ShowTopNews(int? skip, int? take)
        {
            try
            {

                int s = skip.HasValue ? skip.Value : 0;
                int t = take.HasValue ? take.Value : 3;
                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.ContentTypeId == 3 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(aa => aa.Id).Skip(() => s).Take(() => t).ToList().Select(x => new ViewModels.Api.HomePage.Master() { insertdate = x.InsertDate, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = "/tfmag/blog/" + x.Id + "/" + CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()) }),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showtopnews", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult ShowTopVideo(int? skip, int? take)
        {
            try
            {

                int s = skip.HasValue ? skip.Value : 0;
                int t = take.HasValue ? take.Value : 3;
                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.ContentTypeId == 5 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(aa => aa.Id).Skip(() => s).Take(() => t).ToList().Select(x => new ViewModels.Api.HomePage.Master() { insertdate = x.InsertDate, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = "/tfmag/video/" + x.Id + "/" + CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()) }),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "showtopvideo", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult NewProducts()
        {
            try
            {
                var NewCategoryProducts = new List<NewCategoryProductVN>();
                List<int> AllPids = new List<int>();
                int i = 0;
                foreach (var item in context.ProductCategories.AsQueryable().AsNoTracking().Where(x => x.hidden == false && x.IsActive && x.LanguageId == 1 && x.ParrentId == null).Include("attachment").OrderBy(s => Guid.NewGuid()).Skip(() => 0).Take(() => 6).Select(x => new { Id = x.Id, Name = x.Name, PageAddress = x.PageAddress.ToLower(), Descr = x.Descr, image = x.attachment != null ? x.attachment.FileName : "" }).ToList())
                {
                    List<int> Pids = context.Database.SqlQuery<int>("exec GetCatProductIds @catid,@count", new SqlParameter("@CatId", item.Id), new SqlParameter("@count", 20)).ToList();
                    AllPids.AddRange(Pids.OrderBy(x => Guid.NewGuid()).Take(4));
                    NewCategoryProducts.Add(new NewCategoryProductVN()
                    {
                        id = i,
                        CatId = item.Id,
                        CatName = item.Name,
                        CatPageAddress = item.PageAddress.ToLower(),
                        CatDescription = item.Descr,
                        CatImage = item.image,
                        ProductIds = Pids
                    });
                    i++;
                }

                NewCategoryProducts.First().ProductItems = productlist(null, null, AllPids, null, false, false, 0, false, true, 0, 24);

                return Json(new
                {
                    data = NewCategoryProducts,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "newproducts", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult MobileMainMenu()
        {
            try
            {
                #region Get HomePage
                Domain.Content HomePage = null;
                if (Session["HomePage"] == null)
                {
                    HomePage = context.Contents.AsQueryable().AsNoTracking().Where(x => x.LanguageId == 1 && x.IsDefault == true).SingleOrDefault();
                    Session["HomePage"] = HomePage;
                }
                else
                    HomePage = Session["HomePage"] as Domain.Content;

                int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);



                #endregion

                var setting = GetSetting();
                var HeaderMainMenu = new List<DisplayMenu>();
                foreach (var item in context.Menus.AsQueryable().AsNoTracking().Include("Homeattachment").Where(x => x.IsActive == true && (x.PlaceShow == 3 || x.PlaceShow == 1)).ToList())
                {
                    DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                    dm.Id = item.Id;
                    if (item.HomeCover != null)
                        dm.Cover = item.Homeattachment.FileName;
                    else
                        dm.Cover = null;
                    dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == HomePageId && item.TypeId == 11 ? true : false) : false);
                    dm.Link = GenerateLink(item.Id, item.TypeId, item.LinkId, item.LinkUniqIdentifier, item.Title, item.OffLink, item.ParrentMenu, setting.StaticContentDomain);
                    dm.PlaceShow = item.PlaceShow;
                    dm.DisplayOrder = item.DisplaySort;
                    dm.Title = item.Title;
                    dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                    HeaderMainMenu.Add(dm);
                }
                return Json(new
                {
                    data = HeaderMainMenu,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "mobilemainmenu", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GenerateLink(int id, int? typeid, int? linkid, Guid? LinkUniqIdentifier, string title, string offlink, Domain.Menu ParrentMenu, string Url)
        {


            XMLReader readXML = new XMLReader(Url);
            string FinallLink = "";
            if (typeid.HasValue)
            {
                switch (typeid.Value)
                {
                    case 1:
                    case 11:
                        var content = context.Contents.Find(linkid);
                        FinallLink = (content != null ? readXML.DetailOfXContentType(content.ContentTypeId).IsVideo ? "/video/" + linkid + "/" + CommonFunctions.NormalizeAddress(context.Contents.Find(linkid).PageAddress.ToLower()) : content.IsSuperDeal ? "/superdeal" : "/content/" + linkid + "/" + CommonFunctions.NormalizeAddress(context.Contents.Find(linkid).PageAddress.ToLower()) : "#"); break;
                    case 2: FinallLink = (context.attachments.Any(x => x.Id == LinkUniqIdentifier) ? "/Content/UploadFiles/" + context.attachments.Find(LinkUniqIdentifier).FileName : "#"); break;
                    case 3:
                        var category = context.Categories.Find(linkid);
                        FinallLink = (category != null ? readXML.DetailOfXContentType(category.ContentTypeId.Value).IsVideo ? "/tfmag/videocategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(context.Categories.Find(linkid).PageAddress.ToLower()) : "/tfmag/category/" + linkid + "/" + CommonFunctions.NormalizeAddress(context.Categories.Find(linkid).PageAddress.ToLower()) : "#"); break;
                    case 4: FinallLink = (context.Tags.Any(x => x.Id == linkid) ? "/tag/" + linkid + "/" + CommonFunctions.NormalizeAddress(context.Tags.Find(linkid).TagName) : "#"); break;
                    //case 5: dm.Link = db.Sliders.Find(item.LinkId); break;
                    case 6: FinallLink = context.Socials.Find(linkid).Link; break;
                    case 7:
                        FinallLink = (readXML.ListOfXContentType().Any(x => x.Id == linkid) ? linkid == 0 ? "/pages" : linkid == 1 ? "/news" : linkid == 2 ? "/tfmag" : linkid == 3 ? "/blog" : linkid == 5 ? "/videos" : "#" : "#"); break;
                    case 8:
                        var cat = context.ProductCategories.Include("ProductType").Where(x => x.Id == linkid).SingleOrDefault();
                        if (cat != null)
                        {
                            if (!cat.ParrentId.HasValue)
                            {
                                switch (cat.ProductType.DataType)
                                {
                                    case 1:
                                    case 2: FinallLink = "/tfc/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress.ToLower()); break;
                                    case 3: FinallLink = "/Filecategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress.ToLower()); break;
                                    case 4: FinallLink = "/Coursecategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress.ToLower()); break;
                                    case 5: FinallLink = "/Tourcategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress.ToLower()); break;
                                    default: FinallLink = "/tfc/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress.ToLower()); break;
                                }
                            }
                            else
                            {
                                switch (cat.ProductType.DataType)
                                {
                                    case 1:
                                    case 2: FinallLink = "/tfs/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()); break;
                                    case 3: FinallLink = "/FileSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()); break;
                                    case 4: FinallLink = "/CourseSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()); break;
                                    case 5: FinallLink = "/TourSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()); break;
                                    default: FinallLink = "/tfs/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()); break;
                                }
                            }
                        }
                        else
                            FinallLink = "#";
                        break;
                    case 9: FinallLink = "/galleries"; break;
                    case 10:
                        if (ParrentMenu != null)
                        {
                            cat = context.ProductCategories.Include("ProductType").Where(x => x.Id == ParrentMenu.LinkId).SingleOrDefault();
                            if (cat != null)
                            {
                                switch (cat.ProductType.DataType)
                                {
                                    case 1:
                                    case 2: FinallLink = "/tfs/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()) + "/List-" + linkid; break;
                                    case 3: FinallLink = "/FileSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()) + "/List-" + linkid; break;
                                    case 4: FinallLink = "/CourseSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()) + "/List-" + linkid; break;
                                    case 5: FinallLink = "/TourSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()) + "/List-" + linkid; break;
                                    default: FinallLink = "/tfs/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2.ToLower()) + "/List-" + linkid; break;
                                }
                            }
                            else
                                FinallLink = "#";
                        }
                        else
                            FinallLink = "#";
                        break;
                    case 12:
                        FinallLink = "/Page/" + id + "/" + CommonFunctions.NormalizeAddress(title); break;
                }
            }
            else
                FinallLink = offlink;

            return FinallLink;
        }

        [HttpGet]
        public JsonResult AmazingProducts()
        {
            try
            {
                var date = DateTime.Now;
                int amaztake = Request.Browser.IsMobileDevice ? 4 : 5;
                //تخفیفات
                var amazingOffers = new List<AmazingOffersVM>();
                foreach (var item in context.Offers.AsQueryable().AsNoTracking().Where(x => x.CodeTypeValueCode == 1 && x.IsActive && x.LanguageId == 1 && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null) && x.ProductOffers.Any()).Include("ProductOffers.ProductPrice.Product").Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 6).ToList())
                {
                    List<int> ids = new List<int>();
                    ids.AddRange(item.ProductOffers.Where(x => x.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsActive && x.ProductPrice.IsDefault && x.ProductPrice.Quantity > 0 && x.ProductPrice.ProductStateId == 1).OrderByDescending(x => x.ProductPrice.Product.Visits).Take(50).OrderBy(s => Guid.NewGuid()).Select(x => x.ProductPrice.ProductId).Take(amaztake).ToList());
                    amazingOffers.Add(new AmazingOffersVM() { Id = item.Id, Color = item.color, Title = item.Title, Cover = item.Cover.HasValue ? item.attachment.FileName : "", EndDate = item.ExpireDate.HasValue ? item.ExpireDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + item.ExpireDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : null, ProductItems = productlist(null, null, ids, null, false, false, 0, false, true, 0, amaztake) });
                }
                return Json(new
                {
                    data = amazingOffers,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "amazingproducts", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region JTW

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthLogin(LoginViewModel model, string returnUrl, string UrlReferrer)
        {
            try
            {
                IdentityManager im = new IdentityManager();
                if (!ModelState.IsValid)
                    return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);

                string userId = await UserJWTIsAuthenticate(model.Mobile.Trim());
                if (!String.IsNullOrEmpty(userId))
                {
                    if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support") || im.IsInRole(userId, "Designer"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else if (!String.IsNullOrEmpty(returnUrl))
                        return Json(new { status = -1, url = returnUrl }, JsonRequestBehavior.AllowGet);
                    else if (!String.IsNullOrEmpty(UrlReferrer))
                        return Json(new { status = -1, url = UrlReferrer }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "User"))
                        return Json(new { status = -1, url = "/profile/edit" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "Seller"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                }

                ApplicationUser userOld = await UserManager.FindByNameAsync(model.Mobile.Trim());
                if (userOld != null)
                {
                    if (userOld.firstTime)
                    {
                        Random generator = new Random();
                        String ra = generator.Next(0, 100000).ToString("D5");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = userOld.UserName;
                        userOld.ActiveTempCode = ra;
                        userOld.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                        UserManager.Update(userOld);

                        iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.فراموشی, userOld.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                        if (!String.IsNullOrEmpty(iPhonemessage.Body))
                            await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, userOld.FirstName, null, true);

                        return Json(new { status = -4, url = "/account/resetpass?mobile=" + model.Mobile.Trim() + "&old=true" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (userOld.Disable || userOld.PhoneNumberConfirmed == false)
                    {
                        Random generator = new Random();
                        String ra = generator.Next(0, 100000).ToString("D5");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = userOld.UserName;
                        userOld.ActiveTempCode = ra;
                        userOld.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                        UserManager.Update(userOld);
                        try
                        {
                            //PanelsmsManager.SendPatternRegister(user.UserName, "u7czo9bprrmhu1o", ra);
                            iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_نام, userOld.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Edit");
                            if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, userOld.FirstName, userOld.LastName, true);
                        }
                        catch (Exception)
                        {

                        }
                        return Json(new { status = -4, url = "/account/verifyPhone?mobile=" + model.Mobile.Trim() }, JsonRequestBehavior.AllowGet);

                    }
                }

                ApplicationUser user = await UserManager.FindAsync(model.Mobile.Trim(), model.Password);
                if (user != null)
                {
                    if (user.Disable || user.PhoneNumberConfirmed == false)
                    {
                        Random generator = new Random();
                        String ra = generator.Next(0, 100000).ToString("D5");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = user.UserName;
                        user.ActiveTempCode = ra;
                        user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                        UserManager.Update(user);
                        try
                        {
                            iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_نام, user.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                            if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, user.FirstName, user.LastName, true);
                        }
                        catch (Exception)
                        {

                        }
                        return Json(new { status = -5, url = "/account/verifyphone?mobile=" + model.Mobile.Trim() }, JsonRequestBehavior.AllowGet);

                        //return View("Disable");
                    }

                    else
                    {
                        var result = await SignInManager.PasswordSignInAsync(model.Mobile.Trim(), model.Password, model.RememberMe, shouldLockout: true);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                string role = im.IsInRole(user.Id, "Admin") ? "Admin" : im.IsInRole(user.Id, "Security") ? "Security" : im.IsInRole(user.Id, "SuperUser") ? "SuperUser" : im.IsInRole(user.Id, "Support") ? "Support" : im.IsInRole(user.Id, "Designer") ? "Designer" : "User";
                                var (jwtToken, refreshToken) = IssueAuthTokens(user, model.Mobile.Trim(), role);

                                //now we have access to the custom fields added.
                                user.LastActivityDate = DateTime.Now;
                                UserManager.Update(user); // Update DB field


                                AuthenticationManager.SignOut();


                                if (returnUrl != null)
                                    return Json(new { status = 1, url = returnUrl, token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                                else
                                {

                                    if (im.IsInRole(user.Id, "Admin") || im.IsInRole(user.Id, "Security") || im.IsInRole(user.Id, "SuperUser") || im.IsInRole(user.Id, "Support"))
                                        return Json(new { status = 1, url = "/", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                                    else if (!String.IsNullOrEmpty(returnUrl))
                                        return Json(new { status = 1, url = returnUrl, token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                                    else if (!String.IsNullOrEmpty(UrlReferrer))
                                        return Json(new { status = 1, url = UrlReferrer, token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                                    else if (im.IsInRole(user.Id, "User"))
                                        return Json(new { status = 1, url = "/profile", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                                    else if (im.IsInRole(user.Id, "Seller"))
                                        return Json(new { status = 1, url = "/profile", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                                    else
                                        return Json(new { status = 1, url = "/profile", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);

                                }
                            case SignInStatus.LockedOut:
                                return Json(new { status = -6, url = "/account/lockout" }, JsonRequestBehavior.AllowGet);
                            case SignInStatus.RequiresVerification:
                                return Json(new { status = -1, url = "/account/sendcode" }, JsonRequestBehavior.AllowGet);
                            case SignInStatus.Failure:
                            default:
                                return Json(new { status = -3, Message = "نام کاربری یا رمز عبور صحیح نیست" }, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
                else
                {
                    return Json(new { status = -3, Message = "نام کاربری یا رمز عبور صحیح نیست" }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "authlogin", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthLoginRegister(LoginViewModel model, string returnUrl, string UrlReferrer)
        {
            IdentityManager im = new IdentityManager();
            try
            {
                string userId = await UserJWTIsAuthenticate(model.Mobile.Trim());
                if (!String.IsNullOrEmpty(userId))
                {
                    if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support") || im.IsInRole(userId, "Designer"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else if (!String.IsNullOrEmpty(returnUrl))
                        return Json(new { status = -1, url = returnUrl }, JsonRequestBehavior.AllowGet);
                    else if (!String.IsNullOrEmpty(UrlReferrer))
                        return Json(new { status = -1, url = UrlReferrer }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "User"))
                        return Json(new { status = -1, url = "/profile/edit" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "Seller"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                }

                ApplicationUser userOld = await UserManager.FindByNameAsync(model.Mobile.Trim());
                if (userOld != null)
                {
                    if (userOld.firstTime) //Login Old User
                    {
                        Random generator = new Random();
                        String ra = generator.Next(0, 100000).ToString("D5");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = userOld.UserName;
                        userOld.ActiveTempCode = ra;
                        userOld.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                        UserManager.Update(userOld);

                        iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.فراموشی, userOld.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                        if (!String.IsNullOrEmpty(iPhonemessage.Body))
                            await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, userOld.FirstName, null, true);

                        return Json(new { status = -4, url = "/account/resetpass?mobile=" + model.Mobile.Trim() + "&old=true" }, JsonRequestBehavior.AllowGet);
                    }
                    else //Login 
                    {
                        if (userOld.Disable || userOld.PhoneNumberConfirmed == false)
                        {
                            Random generator = new Random();
                            String ra = generator.Next(0, 100000).ToString("D5");
                            SmsService sms = new SmsService();
                            IdentityMessage iPhonemessage = new IdentityMessage();
                            iPhonemessage.Destination = userOld.UserName;
                            userOld.ActiveTempCode = ra;
                            userOld.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                            UserManager.Update(userOld);
                            try
                            {
                                iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_نام, userOld.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                                if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                    await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, userOld.FirstName, userOld.LastName, true);
                            }
                            catch (Exception)
                            {

                            }
                            return Json(new { status = -5, url = "/account/verifyphone?mobile=" + model.Mobile.Trim() }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(returnUrl))
                                return Json(new { status = 1, url = "/account/loginstep2?m=" + model.Mobile.Trim() + "&returnUrl=" + returnUrl }, JsonRequestBehavior.AllowGet);
                            else
                                return Json(new { status = 1, url = "/account/loginstep2?m=" + model.Mobile.Trim() }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else //register
                {
                    Random generator = new Random();
                    String ra = generator.Next(0, 100000).ToString("D5");

                    var user = new ApplicationUser { UserName = model.Mobile.Trim(), FirstName = "", LastName = "" };
                    user.CreationDate = DateTime.Now;
                    user.Disable = true;
                    //user.Disable = false;
                    user.EmailConfirmed = false;
                    user.LockoutEnabled = true;
                    user.PhoneNumber = model.Mobile.Trim();
                    user.PhoneNumberConfirmed = false;
                    //user.PhoneNumberConfirmed = true;
                    user.About = "";
                    user.TwoFactorEnabled = false;
                    user.LandlinePhone = "";
                    user.PostalCode = "";
                    user.State = 1;
                    user.City = "";
                    user.Address = "";
                    user.ActiveTempCode = ra;
                    user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);


                    String ra2 = "tf" + generator.Next(11111111, 99999999).ToString("D8");
                    var result = await UserManager.CreateAsync(user, ra2);
                    if (result.Succeeded)
                    {
                        im.AddUserToRole(user.Id, "User");


                        #region Send SMS AND EMail
                        try
                        {

                            SmsService sms = new SmsService();
                            IdentityMessage iPhonemessage = new IdentityMessage();
                            iPhonemessage.Destination = user.UserName;

                            iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_نام, user.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                            if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, user.FirstName, user.LastName, true);
                        }
                        catch (Exception)
                        {

                        }
                        if (!String.IsNullOrEmpty(returnUrl))
                            return Json(new { status = 2, url = "/account/verifyphone?mobile=" + user.PhoneNumber + "&returnUrl=" + returnUrl, second = (user.ActiveTempCodeExpire - DateTime.Now).Value.TotalSeconds * 1000, ActiveTempCodeExpire = user.ActiveTempCodeExpire }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { status = 2, url = "/account/verifyphone?mobile=" + user.PhoneNumber, second = (user.ActiveTempCodeExpire - DateTime.Now).Value.TotalSeconds * 1000, ActiveTempCodeExpire = user.ActiveTempCodeExpire }, JsonRequestBehavior.AllowGet);

                        #endregion
                    }
                    else
                        return Json(new { status = -3, url = "/account/Login", Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "authloginregister", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public async Task<JsonResult> LoginTemp(string m)
        {
            try
            {
                m = m.Trim();
                bool sendsms = false;
                string msg = "";
                SmsService sms = new SmsService();
                IdentityMessage iPhonemessage = new IdentityMessage();
                #region checkBlackList
                if (TfShop.Infrastructure.Sms.Pattern.checkBlackListNumber(m, out sendsms, out msg))
                {
                    iPhonemessage.Destination = m.Trim();
                    try
                    {
                        if (sendsms == false)
                        {
                            iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.پیام_بلک_لیست, null, null, null, null, null, null, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                            if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                await sms.SendSMSAsync(iPhonemessage, "NewUserSms", null, null, null, null, null, true);
                        }
                        return Json(new { status = -1, Message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = -1, Message = msg + " - exception : " + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion

                if (String.IsNullOrEmpty(m))
                    return Json(new { status = -1, Message = "شماره موبایل وارد نشده است!" }, JsonRequestBehavior.AllowGet);
                var user = UserManager.FindByName(m.Trim());
                if (user == null)
                    return Json(new { status = -1, Message = "چنین کاربری پیدا نشد !" }, JsonRequestBehavior.AllowGet);
                ViewBag.m = m;

                if (user.ActiveTempCodeExpire < DateTime.Now || user.ActiveTempCodeExpire == null)
                {

                    Random generator = new Random();
                    String ra = generator.Next(0, 100000).ToString("D5");
                    iPhonemessage.Destination = user.UserName;
                    user.ActiveTempCode = ra;
                    user.ActiveTempCodeExpire = DateTime.Now.AddSeconds(120);
                    UserManager.Update(user);

                    //PanelsmsManager.SendPatternRegister(user.UserName, "w5i4og4c11cm5t9", ra);
                    iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.رمز_یکبار_مصرف_ورود, user.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                    if (!String.IsNullOrEmpty(iPhonemessage.Body))
                        await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, user.FirstName, null, true);
                }

                return Json(new { status = 1, Message = "", ActiveTempCodeExpire = user.ActiveTempCodeExpire }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "logintemp", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthLoginTemp(LoginTempVM model, string Mobile)
        {
            try
            {
                IdentityManager im = new IdentityManager();

                if (!ModelState.IsValid)
                    return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);

                string userId = await UserJWTIsAuthenticate(Mobile);
                if (!String.IsNullOrEmpty(userId))
                {
                    if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support") || im.IsInRole(userId, "Designer"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "User"))
                        return Json(new { status = -1, url = "/profile/edit" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "Seller"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                }

                ApplicationUser user = await UserManager.FindByNameAsync(Mobile);
                if (user != null)
                {
                    if (user.Disable || user.PhoneNumberConfirmed == false)
                    {

                        Random generator = new Random();
                        String ra = generator.Next(0, 100000).ToString("D5");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = user.UserName;
                        user.ActiveTempCode = ra;
                        user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                        UserManager.Update(user);
                        #region Send SMS AND EMail
                        try
                        {
                            iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_نام, user.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                            if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, user.FirstName, user.LastName, true);
                        }
                        catch (Exception)
                        {

                        }
                        return Json(new { status = -5, url = "/account/verifyphone?mobile=" + user.PhoneNumber }, JsonRequestBehavior.AllowGet);

                        #endregion


                    }
                    else
                    {
                        if (user.ActiveTempCode != model.Code.Trim())
                        {

                            return Json(new { status = -1, Message = " کد وارد شده معتبر نیست! " }, JsonRequestBehavior.AllowGet);
                        }
                        await SignInManager.SignInAsync(user, true, true);

                        //now we have access to the custom fields added.
                        user.LastActivityDate = DateTime.Now;
                        user.PhoneNumberConfirmed = true;
                        string role = im.IsInRole(user.Id, "Admin") ? "Admin" : im.IsInRole(user.Id, "Security") ? "Security" : im.IsInRole(user.Id, "SuperUser") ? "SuperUser" : im.IsInRole(user.Id, "Support") ? "Support" : im.IsInRole(user.Id, "Designer") ? "Designer" : "User";
                        var (jwtToken, refreshToken) = IssueAuthTokens(user, user.PhoneNumber, role);
                        UserManager.Update(user);
                        if (im.IsInRole(user.Id, "Admin") || im.IsInRole(user.Id, "Security") || im.IsInRole(user.Id, "SuperUser") || im.IsInRole(user.Id, "Support") || im.IsInRole(user.Id, "Designer"))
                            return Json(new { status = 1, url = "/", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                        else if (im.IsInRole(user.Id, "User"))
                            return Json(new { status = 1, url = "/profile", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                        else if (im.IsInRole(user.Id, "Seller"))
                            return Json(new { status = 1, url = "/profile", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { status = 1, url = "/", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { status = 1, url = "/account/Login" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "authlogintemp", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthVerifyPhone(string userName, string code, string returnUrl)
        {

            try
            {
                IdentityManager im = new IdentityManager();
                string userId = await UserJWTIsAuthenticate(userName.Trim());
                if (!String.IsNullOrEmpty(userId))
                {
                    if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support") || im.IsInRole(userId, "Designer"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "User"))
                        return Json(new { status = -1, url = "/profile/edit" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "Seller"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                }

                var user = UserManager.FindByName(userName.Trim());

                if (code == null)
                {
                    return Json(new { status = -1, Message = "کد فعال سازی را وارد نمایید." }, JsonRequestBehavior.AllowGet);
                }
                else if (user == null)
                {
                    return Json(new { status = -1, Message = " نام کاربری ( شماره تلفن همراه) وارد شده ، صحیح نیست." }, JsonRequestBehavior.AllowGet);
                }
                else if (user.ActiveTempCode != code)
                {
                    return Json(new { status = -1, Message = " کد وارد شده معتبر نیست! " }, JsonRequestBehavior.AllowGet);
                }
                else if (user.ActiveTempCodeExpire <= DateTime.Now)
                {
                    return Json(new { status = -1, Message = " کد وارد شده منقضی شده است. دوباره درخواست تغییر رمز عبور دهید. " }, JsonRequestBehavior.AllowGet);
                }
                user.PhoneNumberConfirmed = true;
                user.ActiveTempCode = null;
                user.ActiveTempCodeExpire = null;
                user.Disable = false;
                user.LastActivityDate = DateTime.Now;
                string role = im.IsInRole(user.Id, "Admin") ? "Admin" : im.IsInRole(user.Id, "Security") ? "Security" : im.IsInRole(user.Id, "SuperUser") ? "SuperUser" : im.IsInRole(user.Id, "Support") ? "Support" : im.IsInRole(user.Id, "Designer") ? "Designer" : "User";
                var (jwtToken, refreshToken) = IssueAuthTokens(user, user.PhoneNumber, role);
                UserManager.Update(user);

                if (!String.IsNullOrEmpty(returnUrl))

                    return Json(new { status = 1, url = "/profile/edit?returnurl=" + returnUrl, token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = 1, url = "/profile/edit?m=1", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "authverifyphone", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -1, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                IdentityManager im = new IdentityManager();
                string userId = await UserJWTIsAuthenticate(model.Mobile.Trim());
                if (!String.IsNullOrEmpty(userId))
                {
                    if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support") || im.IsInRole(userId, "Designer"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "User"))
                        return Json(new { status = -1, url = "/profile/edit" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "Seller"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                }

                bool sendsms = false;
                string msg = "";
                SmsService sms = new SmsService();
                IdentityMessage iPhonemessage = new IdentityMessage();

                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.Mobile.Trim());
                    if (user == null)
                    {
                        return Json(new { status = -1, Message = " نام کاربری( شماره تلفن همراه) وارد شده وجود ندارد. " }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (user.Disable)
                        {

                            Random generator = new Random();
                            String ra = generator.Next(0, 100000).ToString("D5");
                            iPhonemessage.Destination = user.UserName;
                            user.ActiveTempCode = ra;
                            user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                            UserManager.Update(user);
                            try
                            {
                                iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_نام, user.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                                if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                    await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, user.FirstName, user.LastName, true);
                            }
                            catch (Exception)
                            {

                            }
                            return Json(new { status = -5, url = "/account/verifyphone?mobile=" + user.PhoneNumber }, JsonRequestBehavior.AllowGet);

                        }
                        else if (await UserManager.IsLockedOutAsync(user.Id))
                        {

                            return Json(new { status = -1, Message = " حساب کاربری وارد شده به دلیل اشتباه وارد کردن رمز عبور به مدت 10 دقیقه قفل شده است. پس از آن اقدام نمایید." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            #region checkBlackList
                            if (TfShop.Infrastructure.Sms.Pattern.checkBlackListNumber(model.Mobile, out sendsms, out msg))
                            {
                                iPhonemessage.Destination = model.Mobile;
                                try
                                {
                                    if (sendsms == false)
                                    {
                                        iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.پیام_بلک_لیست, user.Id, null, null, null, null, null, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                                        if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                            await sms.SendSMSAsync(iPhonemessage, "NewUserSms", null, null, null, user.FirstName, user.LastName, true);
                                    }

                                    return Json(new { status = -1, url = "/account/lockout", Message = msg }, JsonRequestBehavior.AllowGet);
                                }
                                catch (Exception)
                                {
                                    return Json(new { status = -1, url = "/account/lockout", Message = msg }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            #endregion

                            Random generator = new Random();
                            String ra = generator.Next(0, 100000).ToString("D5");
                            iPhonemessage.Destination = user.UserName;
                            user.ActiveTempCode = ra;
                            user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(3);
                            UserManager.Update(user);

                            iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.فراموشی, user.Id, null, null, null, null, ra, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/edit");
                            if (!String.IsNullOrEmpty(iPhonemessage.Body))
                                await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, user.FirstName, null, true);
                            //NikSmsManager.SingleSms(iPhonemessage.Body, user.UserName, "09128605712", "Aa@123456", "blacklist");


                            if (user.EmailConfirmed && !String.IsNullOrEmpty(user.Email))
                            {
                                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                                var callbackUrl = "/account/resetpassword?userId=" + user.Id + "&code=" + code;

                                #region Create Html Body
                                string EmailBodyHtml = "";

                                var oSetting = context.Settings.Include("attachment").Where(x => x.LanguageId == 1).SingleOrDefault();
                                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(context.Settings.Include(x => x.attachment).First().attachment.FileName, "درخواست تغییر رمز عبور", "جهت تغییر رمز عبور خود، روی لینک مقابل کلیک کنید. در صورتیکه تمایل به تغییر آن ندارید این ایمیل را نادیده بگیرید <a href='" + callbackUrl + "'> لینک تغییر رمز عبور</a>", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                                EmailBodyHtml = Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);

                                #endregion
                                //EmailService es = new EmailService();
                                //IdentityMessage imessage = new IdentityMessage();
                                //imessage.Body = EmailBodyHtml;
                                //imessage.Destination = user.Email;
                                //imessage.Subject = " درخواست تغییر رمز عبور ";
                                //await es.SendAsync(imessage);
                            }
                            return Json(new { status = 1, url = "/account/resetpass", ActiveTempCodeExpire = user.ActiveTempCodeExpire }, JsonRequestBehavior.AllowGet);
                        }
                    }


                }
                else
                {
                    return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "authforgotpassword", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -1, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }


        //resetpassword From SMS Code
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthResetPass(ResetPasswordViewModel model, bool? old)
        {
            try
            {
                IdentityManager im = new IdentityManager();

                var user = await UserManager.FindByNameAsync(model.Mobile.Trim());
                string userId = user.Id;

                if (!ModelState.IsValid)
                {
                    return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);
                }
                if (user == null)
                {
                    return Json(new { status = -1, Message = " نام کاربری ( شماره تلفن همراه) وارد شده ، صحیح نیست." }, JsonRequestBehavior.AllowGet);
                }
                else if (user.ActiveTempCode != model.Code.Trim())
                {
                    return Json(new { status = -1, Message = " کد وارد شده معتبر نیست! " }, JsonRequestBehavior.AllowGet);
                }
                else if (user.ActiveTempCodeExpire <= DateTime.Now)
                {
                    return Json(new { status = -1, Message = " کد وارد شده منقضی شده است. دوباره درخواست تغییر رمز عبور دهید. " }, JsonRequestBehavior.AllowGet);
                }
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
                if (result.Succeeded)
                {
                    string role = im.IsInRole(user.Id, "Admin") ? "Admin" : im.IsInRole(user.Id, "Security") ? "Security" : im.IsInRole(user.Id, "SuperUser") ? "SuperUser" : im.IsInRole(user.Id, "Support") ? "Support" : im.IsInRole(user.Id, "Designer") ? "Designer" : "User";
                    var (jwtToken, refreshToken) = IssueAuthTokens(user, model.Mobile.Trim(), role);

                    user.ActiveTempCode = null;
                    user.ActiveTempCodeExpire = null;
                    user.firstTime = false;
                    user.Disable = false;
                    user.LastActivityDate = DateTime.Now;
                    UserManager.Update(user);


                    return Json(new { status = 1, url = "/", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -1, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        //resetpassword From Email Link
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<JsonResult> AuthResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);

                IdentityManager im = new IdentityManager();
                string userId = await UserJWTIsAuthenticate(model.Mobile.Trim());
                if (!String.IsNullOrEmpty(userId))
                {
                    if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support") || im.IsInRole(userId, "Designer"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "User"))
                        return Json(new { status = -1, url = "/profile/edit" }, JsonRequestBehavior.AllowGet);
                    else if (im.IsInRole(userId, "Seller"))
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = -1, url = "/" }, JsonRequestBehavior.AllowGet);
                }


                var user = await UserManager.FindByNameAsync(model.Mobile.Trim());
                if (user == null)
                {
                    return Json(new { status = -1, Message = " نام کاربری ( شماره تلفن همراه) وارد شده ، صحیح نیست. " }, JsonRequestBehavior.AllowGet);
                }
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code.Trim(), model.Password);
                if (result.Succeeded)
                {

                    ApplicationUser userr = context.Users.Where(x => x.UserName == model.Mobile.Trim() && !x.EmailConfirmed).FirstOrDefault();
                    if (userr != null)
                    {


                        userr.EmailConfirmed = true;
                        await context.SaveChangesAsync();
                    }
                    string role = im.IsInRole(user.Id, "Admin") ? "Admin" : im.IsInRole(user.Id, "Security") ? "Security" : im.IsInRole(user.Id, "SuperUser") ? "SuperUser" : im.IsInRole(user.Id, "Support") ? "Support" : im.IsInRole(user.Id, "Designer") ? "Designer" : "User";
                    var (jwtToken, refreshToken) = IssueAuthTokens(user, model.Mobile.Trim(), role);
                    user.LastActivityDate = DateTime.Now;
                    UserManager.Update(user);


                    return Json(new { status = 1, url = "/account/resetpasswordConfirmation", token = jwtToken, refreshToken = refreshToken }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = -3, Message = "خطایی رخ داد" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "authresetpassword", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -1, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> AuthLogOut()
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var id = Authentication.ValidateToken(Token);
                if (id != null)
                {
                    ApplicationUser user = context.Users.Find(id);
                    user.JWTtoken = null;
                    user.JWTtokenExpire = null;
                    // رفرش‌توکن هم باطل بشه - وگرنه با خروج از حساب، هرکسی که این رفرش‌توکن رو
                    // (مثلاً از کوکی مرورگر قبلی) داشته باشه می‌تونه بازم access token تازه بگیره.
                    user.RefreshToken = null;
                    user.RefreshTokenExpire = null;
                    await context.SaveChangesAsync();
                    AuthenticationManager.SignOut();
                    return Json(new { status = 1, Message = "Loged Out" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "authlogout", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        // معادل صدور یه access token تازه بدون نیاز به لاگین دوباره (رمز/OTP) - وقتی access token
        // فعلی (که عمرش فقط config:JwtExpireDays روزه) منقضی شده باشه. برخلاف بقیه‌ی اکشن‌ها، عمداً
        // [JWTAuthorize] نداره چون دقیقاً همون لحظه‌ای صدا زده می‌شه که AuthToken هدر منقضی شده و
        // دیگه معتبر نیست؛ اعتبارسنجی این‌جا کاملاً بر پایه‌ی خودِ رفرش‌توکنه (هش‌شده، مقایسه‌شده با
        // چیزی که توی دیتابیس ذخیره شده). رفرش‌توکن هم rotate می‌شه (هر استفاده، توکنِ قبلی رو باطل
        // و یه توکن جدید صادر می‌کنه) که اگه یه رفرش‌توکنِ قدیمی لو رفته باشه، عمر مفیدش محدود بمونه.
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> AuthRefreshToken(string refreshToken)
        {
            try
            {
                if (String.IsNullOrEmpty(refreshToken))
                    return Json(new { status = 10, Message = "Refresh Token Expired!" }, JsonRequestBehavior.AllowGet);

                string hashed = Authentication.HashToken(refreshToken);
                var user = context.Users.FirstOrDefault(x => x.RefreshToken == hashed);
                if (user == null || !user.RefreshTokenExpire.HasValue || user.RefreshTokenExpire.Value <= DateTime.Now)
                    return Json(new { status = 10, Message = "Refresh Token Expired!" }, JsonRequestBehavior.AllowGet);

                IdentityManager im = new IdentityManager();
                string role = im.IsInRole(user.Id, "Admin") ? "Admin" : im.IsInRole(user.Id, "Security") ? "Security" : im.IsInRole(user.Id, "SuperUser") ? "SuperUser" : im.IsInRole(user.Id, "Support") ? "Support" : im.IsInRole(user.Id, "Designer") ? "Designer" : "User";
                var (newJwtToken, newRefreshToken) = IssueAuthTokens(user, user.UserName, role);
                user.LastActivityDate = DateTime.Now;
                // نکته: قبلاً اینجا UserManager.Update(user) صدا زده می‌شد. UserManager از یه
                // DbContext کاملاً جدا (اون که IdentityManager/im.IsInRole چند خط بالاتر ازش
                // استفاده کرده) میاد - نه همین `context`ی که `user` رو چند خط بالاتر باهاش لود
                // کردیم. چون im.IsInRole همون‌جا خودش یه نمونه‌ی دیگه از همین ApplicationUser رو
                // توی DbContext داخلیِ UserManager لود/track می‌کنه، صدازدنِ UserManager.Update(user)
                // با شیء `user`ی که از `context` اومده، باعث خطای EF می‌شد: «Attaching an entity of
                // type 'ApplicationUser' failed because another entity of the same type already has
                // the same primary key value» - یعنی این متد همیشه (نه گاهی) throw می‌کرد، پس رفرش‌توکن
                // عملاً هیچ‌وقت کار نمی‌کرد. چون `user` از قبل توسط همین `context` track شده،
                // SaveChangesAsync مستقیم (بدون UserManager) کافی و درسته.
                await context.SaveChangesAsync();

                return Json(new { status = 1, token = newJwtToken, refreshToken = newRefreshToken, statusCode = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "authrefreshtoken", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [JWTAuthorize]
        public async Task<JsonResult> AuthUserInfo()
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var id = Authentication.ValidateToken(Token);
                if (id != null)
                {
                    var user = await context.Users.AsQueryable().AsNoTracking().Where(x => x.Id == id).Select(x => new { x.FirstName, x.LastName }).SingleOrDefaultAsync();
                    if (user == null)
                        return Json(new { status = 10, Message = "user not found!" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = 1, Name = user.FirstName, Family = user.LastName }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "authuserinfo", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { status = -3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // بعد از هر لاگین موفق (AuthLogin/AuthLoginTemp/AuthVerifyPhone/AuthResetPass/AuthResetPassword)
        // صدا زده می‌شه: هم access token (JWT، معادل قبلی) و هم یه refresh token جدید صادر می‌کنه، و
        // هردو رو (JWTtoken خام برای بک‌آفیس + هش رفرش‌توکن) روی خودِ user ست می‌کنه. توجه: خودِ
        // UserManager.Update(user)/SaveChanges رو باید بعد از این متد، همون‌جایی که قبلاً بود صدا بزنید
        // - این متد فقط فیلدها رو روی آبجکت user (که از قبل tracked هست) ست می‌کنه.
        private (string jwtToken, string refreshToken) IssueAuthTokens(ApplicationUser user, string username, string role)
        {
            var jwtToken = Authentication.GenerateJWTAuthetication(username, role, user.Id);
            var refreshToken = Authentication.GenerateRefreshToken();

            int refreshDays;
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["config:RefreshTokenExpireDays"], out refreshDays) || refreshDays <= 0)
                refreshDays = 30;

            user.JWTtoken = jwtToken;
            user.JWTtokenExpire = DateTime.Now.AddDays(1);
            user.RefreshToken = Authentication.HashToken(refreshToken);
            user.RefreshTokenExpire = DateTime.Now.AddDays(refreshDays);

            return (jwtToken, refreshToken);
        }

        private async Task<string> UserJWTIsAuthenticate(string username)
        {
            try
            {
                // قبلاً این متد فقط چک می‌کرد که کاربرِ متناظر با این username/mobile یه JWTtoken
                // ذخیره‌شده‌ی هنوز منقضی‌نشده توی دیتابیس داره یا نه (JWTtokenExpire همیشه ۱ روز بعد
                // از هر لاگین موفق تنظیم می‌شه - AddDays(1)). این کاملاً مستقل از این بود که آیا
                // درخواست فعلی واقعاً متعلق به یه session معتبر همون کاربره یا نه؛ نتیجه‌ش این بود که
                // هر کاربری که ظرف ۲۴ ساعت گذشته حتی یک‌بار لاگین کرده بود، با هر تلاش بعدی برای ورود
                // (حتی از مرورگر/دستگاه دیگه، حتی بعد از خروج واقعی) بدون این‌که رمز عبورش پرسیده بشه
                // مستقیم ریدایرکت می‌شد - بدون این‌که واقعاً authenticate هم بشه (چون توکنی صادر
                // نمی‌شد)، یعنی عملاً ورود خراب می‌شد (این دقیقاً همون باگ گزارش‌شده‌ست: دکمه‌ی ورود
                // به‌جای پرسیدن رمز عبور، کاربر رو به صفحه‌ی اصلی ریدایرکت می‌کرد).
                // چک درست: باید ببینیم خودِ همین درخواست (هدر AuthToken) یه JWT معتبر داره یا نه، و
                // اون توکن واقعاً متعلق به همین username باشه - نه این‌که صرفاً یه توکن قدیمی برای این
                // username توی دیتابیس هنوز منقضی نشده باشه.
                string currentToken = Request.Headers.GetValues("AuthToken")?.FirstOrDefault();
                string currentUserId = TfShop.Infrastructure.AuthJTW.Authentication.ValidateToken(currentToken);
                if (String.IsNullOrEmpty(currentUserId))
                    return "";

                ApplicationUser user = await UserManager.FindByNameAsync(username);
                if (user != null && user.Id == currentUserId)
                    return user.Id;

                return "";
            }
            catch
            {

                return "";
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion

        #region Mag

        #region Header


        [HttpGet]
        public JsonResult GetLogoMag()
        {
            try
            {
                return Json(new
                {
                    data = context.Settings.AsNoTracking().Where(x => x.LanguageId == 1).Select(x => x.attachmentLogoMag.FileName).FirstOrDefault(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getlogomag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpGet]
        public JsonResult GetMagMenu(int? which)
        {
            int w = which.HasValue ? which.Value : 1;
            try
            {
                List<DisplayMenu> DisplayMenus = new List<DisplayMenu>();
                foreach (var item in context.Menus.AsQueryable().AsNoTracking().Include("attachment").Where(x => x.IsActive == true && (x.PlaceShow == 12 || x.PlaceShow == w)).OrderBy(x => x.DisplaySort).ToList())
                {
                    DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                    dm.Id = item.Id;
                    if (item.Cover != null)
                        dm.Cover = item.attachment.FileName;
                    else
                        dm.Cover = null;
                    dm.icon = item.icon;
                    dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == 400 && item.TypeId == 11 ? true : false) : false);
                    dm.menuLink = item.FinalLink;
                    dm.PlaceShow = item.PlaceShow;
                    dm.DisplayOrder = item.DisplaySort;
                    dm.Title = item.Title;
                    dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                    DisplayMenus.Add(dm);
                }



                return Json(new
                {
                    data = DisplayMenus,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getmagmenu", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        #endregion

        #region Index


        [HttpGet]
        public JsonResult getMainMagAds(string[] position, bool? m, int? t)
        {
            try
            {
                t = t.HasValue ? t.Value : 9;
                int[] ps = Array.ConvertAll(position[0].ToString().Split(','), s => int.Parse(s));
                DateTime dtime = DateTime.Now;
                if (m.HasValue && m.Value)
                {
                    return Json(new
                    {
                        data = context.Adverestings.AsQueryable().AsNoTracking().Where(x => ps.Contains(x.Position) && x.TypeId == t && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).Include("attachment2").Include("attachment").OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        data = context.Adverestings.AsQueryable().AsNoTracking().Where(x => ps.Contains(x.Position) && x.TypeId == t && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).Include("attachment").OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }



            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getmainmagads", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpGet]
        public JsonResult getBlogMainMag()
        {
            try
            {

                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").Include("Blogattachment").AsNoTracking().Where(x => x.ContentTypeId == 3 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.ChiefEditor == false && x.BlogMain == true && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 5).Select(x => new Master2() { id = x.Id, image = x.BlogCover.HasValue ? x.Blogattachment.FileName : x.attachment.FileName, title = x.Title, pageaddress = x.PageAddress.ToLower() }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getblogmainmag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult getBlogEditorMag()
        {
            try
            {

                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.ContentTypeId == 3 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.BlogMain == false && x.ChiefEditor == true && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 10).ToList().Select(x => new Master2() { insertdate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate), id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower() }),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getblogeditormag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult getBlogNewMag(int c)
        {
            try
            {

                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").Include("Category").AsNoTracking().Where(x => x.ContentTypeId == 3 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.BlogMain == false && x.ChiefEditor == false && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Id).Skip(() => 0).Take(() => c).Select(x => new Master3() { id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), abst = x.Abstract, visits = x.Visits, readMinuts = x.ReadMinuts, insertdate = x.InsertDate, catid = x.CatId, cattitle = x.CatId.HasValue ? x.Category.Title : "", catpageaddress = x.CatId.HasValue ? x.Category.PageAddress.ToLower() : "" }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getblognewmag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult getBlogHotMag()
        {
            try
            {

                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").Include("Blogattachment").AsNoTracking().Where(x => x.ContentTypeId == 3 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.ChiefEditor == false && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Visits).Skip(() => 0).Take(() => 10).Select(x => new Master2() { id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower() }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getbloghotmag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult getVideoHotMag()
        {
            try
            {

                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").Include("Blogattachment").AsNoTracking().Where(x => x.ContentTypeId == 5 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.ChiefEditor == false && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Visits).Skip(() => 0).Take(() => 10).ToList().Select(x => new Master4() { id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), duration = TimeSpan.FromSeconds(x.duration).ToString(@"mm\:ss") }),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getvideohotmag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult getVideoPodcastHotMag()
        {
            try
            {

                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    data = context.Contents.AsQueryable().Include("attachment").Include("VideoAttachment").Include("AudioAttachment").AsNoTracking().Where(x => (x.ContentTypeId == 5 || x.ContentTypeId == 7) && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.ChiefEditor == false && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 6).Select(x => new Master5() { id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), audio = x.Audio.HasValue, abst = x.Abstract, visits = x.Visits, videosrc = x.Video.HasValue ? x.VideoAttachment.FileName : "", audiosrc = x.Audio.HasValue ? x.AudioAttachment.FileName : "" }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getvideopodcasthotmag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult LoadMorePr(int c)
        {
            try
            {
                DateTime dtime = DateTime.Now;
                c = c * 24;


                return Json(new
                {
                    c = (c / 24) + 1,
                    data = context.Contents.AsQueryable().Include("attachment").Include("Blogattachment").Include("Category").AsNoTracking().Where(x => x.ContentTypeId == 3 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.BlogMain == false && x.ChiefEditor == false && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(s => s.Id).Skip(() => (c)).Take(() => 24).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "loadmorepr", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region search, category 

        [HttpGet]
        public JsonResult MagSearch(int? page, int? pagesize, int? sort, int? type, string title, int? contenttypeid, int? catid)
        {
            try
            {
                DateTime dtime = DateTime.Now;
                int pageSize = pagesize.HasValue ? pagesize.Value : 12;
                int pageNumber = (page ?? 1);
                List<int> ids = new List<int>();
                if (type.HasValue)
                {
                    int t = 0;
                    switch (type)
                    {
                        case 1: t = 3; break;
                        case 2: t = 5; break;
                        case 3: t = 7; break;
                    }
                    var topcontent = context.Contents.AsQueryable().AsNoTracking().Include("Blogattachment").Include("attachment").Include("Category").Where(x => x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false && x.ContentTypeId == t && (x.StartDate == null || x.StartDate <= dtime));
                    if (catid.HasValue)
                    {
                        List<int> cids = context.Database.SqlQuery<int>("exec GetContentSubCats @CatId", new SqlParameter("@CatId", catid.Value)).ToList();
                        topcontent = topcontent.Where(x => cids.Contains(x.CatId.Value));
                    }
                    ids = topcontent.OrderByDescending(x => x.Id).Skip(0).Take(5).Select(x => x.Id).ToList();
                }
                var contents = context.Contents.AsQueryable().AsNoTracking().Where(x => !ids.Contains(x.Id) && (x.ContentTypeId == 3 || x.ContentTypeId == 5 || x.ContentTypeId == 7) && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).Include("attachment").Include("Category").Include("Tags");
                if (!String.IsNullOrEmpty(title))
                {
                    contents = contents.Where(x => (x.Title.Contains(title) || x.Abstract.Contains(title) || x.Tags.Any(s => s.Title.Contains(title))));
                    ViewBag.title = title;
                }
                if (catid.HasValue)
                {
                    List<int> CatIds = context.Database.SqlQuery<int>("exec GetContentSubCats @CatId", new SqlParameter("@CatId", catid.Value)).ToList();
                    contents = contents.Where(x => CatIds.Contains(x.CatId.Value));
                }
                switch (type)
                {
                    case 1: contents = contents.Where(x => x.ContentTypeId == 3); break;
                    case 2: contents = contents.Where(x => x.ContentTypeId == 5); break;
                    case 3: contents = contents.Where(x => x.ContentTypeId == 7); break;
                }
                switch (sort)
                {
                    case 1: contents = contents.OrderByDescending(x => x.Id); break;
                    case 2: contents = contents.OrderByDescending(x => x.ChiefEditor); break;
                    case 3: contents = contents.OrderByDescending(x => x.Visits); break;
                    case 4: contents = contents.OrderByDescending(x => x.Comments.Count); break;
                    default: contents = contents.OrderByDescending(x => x.Id); break;
                }

                if (contenttypeid.HasValue)
                {
                    var setting = GetSetting();
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    var contentType = readXML.ListOfXContentType().Where(x => x.LanguageId == 1 && x.Id == contenttypeid).SingleOrDefault();
                    Domain.ViewModels.MagContentType magContentType = new MagContentType()
                    {
                        contentType = contentType,
                    };
                    return Json(new
                    {
                        title = magContentType.contentType.Name,
                        descr = magContentType.contentType.Abstract,
                        c = contents.Count(),
                        data = contents.Select(x => new Master6() { duration = x.duration, contenttype = x.ContentTypeId == 3 ? "blog" : x.ContentTypeId == 5 ? "video" : "podcast", id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), abst = x.Abstract, visits = x.Visits, readMinuts = x.ReadMinuts, catid = x.CatId, cattitle = x.CatId.HasValue ? x.Category.Title : "", catpageaddress = x.CatId.HasValue ? x.Category.PageAddress.ToLower() : "" }).ToPagedList(pageNumber, pageSize),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json(new
                    {

                        c = contents.Count(),
                        data = contents.Select(x => new Master6() { duration = x.duration, contenttype = x.ContentTypeId == 3 ? "blog" : x.ContentTypeId == 5 ? "video" : "podcast", id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), abst = x.Abstract, visits = x.Visits, readMinuts = x.ReadMinuts, catid = x.CatId, cattitle = x.CatId.HasValue ? x.Category.Title : "", catpageaddress = x.CatId.HasValue ? x.Category.PageAddress.ToLower() : "" }).ToPagedList(pageNumber, pageSize),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "magsearch", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult MagContentTypes(int id)
        {
            try
            {
                DateTime dtime = DateTime.Now;
                return Json(new
                {
                    ads = context.Adverestings.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.TypeId == 8 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    data = context.Contents.AsQueryable().AsNoTracking().Include("Blogattachment").Include("attachment").Include("User").Include("Category").Where(x => x.ContentTypeId == id && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(x => x.Id).Skip(0).Take(5).Select(x => new Master6() { duration = x.duration, contenttype = x.ContentTypeId == 3 ? "blog" : x.ContentTypeId == 5 ? "video" : "podcast", id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), abst = x.Abstract, visits = x.Visits, readMinuts = x.ReadMinuts, catid = x.CatId, cattitle = x.CatId.HasValue ? x.Category.Title : "", catpageaddress = x.CatId.HasValue ? x.Category.PageAddress.ToLower() : "" }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "magcontenttypes", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult MagCat(int id)
        {
            try
            {
                DateTime dtime = DateTime.Now;
                List<int> CatIds = context.Database.SqlQuery<int>("exec GetContentSubCats @CatId", new SqlParameter("@CatId", id)).ToList();

                return Json(new
                {
                    ads = context.Adverestings.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.TypeId == 2 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    data = context.Contents.AsQueryable().AsNoTracking().Include("Blogattachment").Include("attachment").Include("Category").Where(x => x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false && CatIds.Contains(x.CatId.Value) && (x.StartDate == null || x.StartDate <= dtime)).OrderByDescending(x => x.Id).Skip(0).Take(5).Select(x => new Master6() { duration = x.duration, contenttype = x.ContentTypeId == 3 ? "blog" : x.ContentTypeId == 5 ? "video" : "podcast", id = x.Id, image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg", title = x.Title, pageaddress = x.PageAddress.ToLower(), abst = x.Abstract, visits = x.Visits, readMinuts = x.ReadMinuts, catid = x.CatId, cattitle = x.CatId.HasValue ? x.Category.Title : "", catpageaddress = x.CatId.HasValue ? x.Category.PageAddress.ToLower() : "" }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "magcat", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }



        #endregion

        #region content

        [HttpGet]
        public JsonResult GetContent(int? id, string title)
        {
            if (!id.HasValue)
                return Json(new
                {
                    url = "/tfmag/blogs",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);

            if (uow.RedirectUrlRepository.Any(x => x.Id, x => x.SourceTypeId == 1 && x.SourceLinkId == id.Value))
            {
                return Json(new
                {
                    url = uow.RedirectUrlRepository.Get(x => x.DestinationUrl, x => x.SourceTypeId == 1 && x.SourceLinkId == id.Value).First(),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);
            }

            //if (String.IsNullOrEmpty(title))
            //title = title.Replace("-", " ").ToLower();


            // چنین محتوایی وجود دارد؟
            var cont = uow.ContentRepository.Get(x => x, x => x.Id == id.Value && x.IsActive).FirstOrDefault();
            string conttype = cont.ContentTypeId == 5 ? "video" : cont.ContentTypeId == 7 ? "podcast" : "blog";
            if (cont == null)
            {
                return Json(new
                {
                    url = "/tfmag",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }
            if (String.IsNullOrEmpty(title))
            {
                return Json(new
                {
                    url = string.Format("/tfmag/" + conttype + "/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(cont.PageAddress.ToLower())),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);
            }

            string incomingTitle = Server.UrlDecode(title ?? "").ToLower();
            string normalizedDbTitle = CommonFunctions.NormalizeAddress(cont.PageAddress.ToLower());
            if (incomingTitle != normalizedDbTitle)
            {
                title = title.Replace("-", " ").ToLower();
                return Json(new
                {
                    url = string.Format("/tfmag/" + conttype + "/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(cont.PageAddress.ToLower())),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DateTime dtime = DateTime.Now;
                var ct = context.Contents.AsQueryable().Include(x => x.VideoAttachment).Include(x => x.AudioAttachment).Include(x => x.attachment).Include(x => x.ContentRating).Include(x => x.Sources).Include(x => x.Tags).Include(x => x.ContentFAQs).Include("OtherImages.attachment").Include(x => x.Category).Include(x => x.Comments).Where(x => (x.CatId == null || x.Category.IsActive) && x.IsActive && x.Id == id && x.IsActive && (x.StartDate == null || x.StartDate <= dtime)).FirstOrDefault();
                if (ct == null)
                {
                    return Json(new
                    {
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                var setting = GetSetting();
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                // var ContentType = readXML.DetailOfXContentType(ct.ContentTypeId);
                //if (ContentType.IsVideo)
                //{
                //    return Json(new
                //    {
                //        url = string.Format("/tfmag/video/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(ct.PageAddress.ToLower())),
                //        statusCode = 301
                //    }, JsonRequestBehavior.AllowGet);
                //}
                //if (ContentType.IsAudio)
                //{
                //    return Json(new
                //    {
                //        url = string.Format("/tfmag/podcast/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(ct.PageAddress.ToLower())),
                //        statusCode = 301
                //    }, JsonRequestBehavior.AllowGet);
                //}
                //if (ContentType.Id == 9)
                //{
                //    return Json(new
                //    {
                //        url = string.Format("/Ads/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(ct.PageAddress.ToLower())),
                //        statusCode = 301
                //    }, JsonRequestBehavior.AllowGet);
                //}
                if (ct.Category != null)
                {
                    if (ct.Category.IsActive == false)
                    {
                        return Json(new
                        {
                            url = "/tfmag",
                            statusCode = 404
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                singleContent content = new singleContent()
                {
                    contenttypeid = ct.ContentTypeId,
                    duration = ct.duration,
                    contenttype = ct.ContentTypeId == 3 ? "blog" : ct.ContentTypeId == 5 ? "video" : "podcast",
                    id = ct.Id,
                    image = ct.Cover.HasValue ? ct.attachment.FileName : "default-thumbnail.jpg",
                    video = ct.Video.HasValue ? ct.VideoAttachment.FileName : "",
                    audio = ct.Audio.HasValue ? ct.AudioAttachment.FileName : "",
                    title = ct.Title,
                    pageaddress = ct.PageAddress.ToLower(),
                    abst = ct.Abstract,
                    visits = ct.Visits,
                    readMinuts = ct.ReadMinuts,
                    insertdate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(ct.InsertDate),
                    inserttime = ct.InsertDate.ToString("HH:mm"),
                    catid = ct.CatId,
                    cattitle = ct.CatId.HasValue ? ct.Category.Title : "",
                    catpageaddress = ct.CatId.HasValue ? ct.Category.PageAddress.ToLower() : "",
                    data = ct.Data,
                    hasContent = ct.HasContact,
                    rate = ct.ContentRating != null ? ct.ContentRating.AverageRating : 0,
                    rateCount = ct.ContentRating != null ? ct.ContentRating.TotalRaters : 0,
                    likeCount = ct.Link,
                    sources = ct.Sources.Select(x => new TitleLink() { title = x.Title, link = x.Link }).ToList(),
                    tags = ct.Tags.Select(x => new TitleLink() { title = x.TagName, link = x.Id.ToString() }).ToList(),
                    fags = ct.ContentFAQs.Select(x => new TitleLink() { title = x.Question, link = x.Answer }).ToList(),
                    images = ct.OtherImages.Select(x => new TitleLink() { title = x.attachment.FileName, link = x.attachment.Title }).ToList(),
                    comments = ct.Comments.Where(x => x.IsActive).Select(x => new singleComment()
                    {
                        Id = x.Id,
                        ParrentId = x.ParrentId,
                        FullName = x.FullName,
                        InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate),
                        InsertTime = x.InsertDate.ToString("HH:mm"),
                        Message = x.Message,
                        NegativeRating = x.NegativeRating,
                        PositiveRating = x.PositiveRating,
                        ChildComments = x.ChildComment.Select(s => new singleComment()
                        {
                            Id = s.Id,
                            ParrentId = s.ParrentId,
                            FullName = s.FullName,
                            InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(s.InsertDate),
                            InsertTime = s.InsertDate.ToString("HH:mm"),
                            Message = s.Message,
                            NegativeRating = s.NegativeRating,
                            PositiveRating = s.PositiveRating,
                        }).ToList()
                    }).ToList()

                };
                List<breadcrumbCat> catitems = new List<breadcrumbCat>();
                if (content.catid.HasValue)
                {
                    catitems.Add(new breadcrumbCat() { catid = content.catid, cattitle = content.cattitle, catpageaddress = content.catpageaddress.ToLower(), contenttypeid = content.contenttypeid });
                }
                // معادلِ VideoObject ای که سورس قدیمی (TFMagController.GetVideoGoogle، مصرف‌شده توی
                // TFMag/video.cshtml) فقط برای محتوای ویدیویی می‌ساخت - اینجا هم دقیقاً همون فیلدها،
                // با استفاده از داده‌ای که همین الان توی ct (با Include هایِ VideoAttachment/attachment)
                // لود شده، بدون کوئری اضافه. برخلاف نسخه‌ی قدیمی (که یه Any() سراسری بدون فیلترِ id
                // داشت)، اینجا مستقیم رو خودِ محتوای جاری چک می‌شه.
                var productGoogleVideos = ct.ContentTypeId == 5 && ct.Video.HasValue && ct.Cover.HasValue && ct.VideoAttachment != null
                    ? new List<ProductVideoGoogleList>
                      {
                          new ProductVideoGoogleList
                          {
                              @context = "https://schema.org/",
                              @type = "VideoObject",
                              name = ct.Title,
                              description = ct.Abstract,
                              thumbnailUrl = new List<string> { "https://www.tfshops.com/Content/UploadFiles/" + ct.attachment.FileName },
                              uploadDate = string.Format("{0}T{1}+3:30", ct.VideoAttachment.InsertDate.ToString("yyyy-MM-dd"), ct.VideoAttachment.InsertDate.ToString("HH:mm:ss")),
                              duration = string.Format("PT{0}M{1}S", (ct.VideoAttachment.duration / 60).ToString(), (ct.VideoAttachment.duration % 60).ToString()),
                              contentUrl = "https://www.tfshops.com/Content/UploadFiles/" + ct.VideoAttachment.FileName,
                              embedUrl = "https://www.tfshops.com/embed/cvideo/" + ct.Id,
                              interactionStatistic = new interactionStatistic
                              {
                                  @type = "InteractionCounter",
                                  interactionType = new interactionType { type = "WatchAction" },
                                  userInteractionCount = ct.Visits
                              }
                          }
                      }
                    : new List<ProductVideoGoogleList>();

                return Json(new
                {
                    ads = context.Adverestings.AsQueryable().Include("attachment").AsNoTracking().Where(x => x.TypeId == (content.contenttypeid == 0 ? 7 : 1) && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    data = content,
                    cat = catitems,
                    ProductGoogleVideos = productGoogleVideos,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getcontent", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GetContentads(int id, string[] position)
        {
            try
            {
                DateTime dtime = DateTime.Now;
                int[] ps = Array.ConvertAll(position[0].ToString().Split(','), s => int.Parse(s));

                return Json(new
                {
                    //ads = context.Adverestings.AsQueryable().Include("attachment").AsNoTracking().Where(x => ps.Contains(x.Position) && x.TypeId == 1 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    ads = context.Adverestings.AsQueryable().Include("attachment").AsNoTracking().Where(x => ps.Contains(x.Position) && x.TypeId == 1 && (x.LinkId == id || x.LinkId == 0) && ((x.ExpireDate != null && x.ExpireDate >= dtime) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtime) || x.StartDate == null)).OrderBy(x => x.DisplaySort).Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getcontentads", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GetContentRelateds(int id)
        {
            try
            {
                var setting = GetSetting();
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var contenttypes = readXML.ListOfXContentType().Where(x => x.Id == 3 || x.Id == 5 || x.Id == 7).OrderBy(x => x.Name);
                List<ContentTypeVM> ContentTypeViewModels = new List<ViewModels.Content.ContentTypeVM>();
                foreach (var item in contenttypes)
                {
                    ViewModels.Content.ContentTypeVM newitem = new ViewModels.Content.ContentTypeVM();
                    newitem.Abstract = item.Abstract;
                    newitem.Id = item.Id;
                    newitem.LanguageId = item.LanguageId;
                    newitem.IsVideo = item.IsVideo;
                    newitem.Name = item.ShortName;
                    newitem.Title = item.Title;
                    List<int> ids = context.Database.SqlQuery<int>("exec GetRelatedContent2 @ContentId,@contentTypeId,@ResultCount,@Lang", new SqlParameter("@ContentId", id), new SqlParameter("@contentTypeId", item.Id), new SqlParameter("@ResultCount", 3), new SqlParameter("@Lang", 1)).ToList();
                    newitem.Contents = context.Contents.AsQueryable().Include("attachment").AsNoTracking().Where(x => ids.Contains(x.Id)).Select(x => new RelatedContent() { id = x.Id, ContentTypeId = x.ContentTypeId, title = x.Title, pageaddress = x.PageAddress.ToLower(), image = x.attachment != null ? x.attachment.FileName : "default-thumbnail.jpg", }).ToList();
                    ContentTypeViewModels.Add(newitem);
                }


                return Json(new
                {
                    contentTypes = ContentTypeViewModels,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getcontentrelateds", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public virtual async Task<JsonResult> PlusComment(int CommentId)
        {
            try
            {
                if (context.Comments.Find(CommentId) != null)
                {

                    Comment cm = context.Comments.Find(CommentId);
                    if (cm != null)
                    {
                        cm.PositiveRating++;
                        await context.SaveChangesAsync();

                        return Json(new
                        {
                            count = cm.PositiveRating,
                            Message = "ثبت شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = "خطا.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    return Json(new
                    {
                        Message = "خطا.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (System.Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "pluscomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual async Task<JsonResult> MinusComment(int CommentId)
        {
            try
            {
                if (context.Comments.Find(CommentId) != null)
                {


                    Comment cm = context.Comments.Find(CommentId);
                    if (cm != null)
                    {
                        cm.NegativeRating++;
                        await context.SaveChangesAsync();

                        return Json(new
                        {
                            count = cm.PositiveRating,
                            Message = "ثبت شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = "خطا.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "خطا.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (System.Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "minuscomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public async Task<JsonResult> AddContactUs(TfShop.ViewModels.Api.Mag.singleContent ContactUs)
        {
            try
            {
                if (String.IsNullOrEmpty(ContactUs.addContactBtn))
                {
                    return Json(new
                    {
                        Message = "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if (!TfShop.Infrastructure.Helper.Recaptcha.ReCaptchaPassed(ContactUs.addContactBtn))
                    if (false)
                    {
                        return Json(new
                        {
                            Message = "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        context.ContactUs.Add(new Domain.ContactUs()
                        {
                            InsertDate = DateTime.Now,
                            ContentId = ContactUs.ContentId,
                            Email = ContactUs.Email,
                            FullName = ContactUs.FullName,
                            Message = ContactUs.Message,
                            Tele = ContactUs.Tele,
                            IsVisit = false
                        });
                        await context.SaveChangesAsync();


                        var content = context.Contents.Find(ContactUs.ContentId);
                        //Send Mail To Related Admin
                        #region Create Html Body
                        string EmailBodyHtml = "";

                        var oSetting = context.Settings.Include(x => x.attachment).FirstOrDefault();
                        string contentUrl = "https://www.tfshops.com";
                        if (!content.IsDefault)
                            contentUrl = "https://www.tfshops.com" + "/tfmag/blog/" + content.Id + "/" + CommonFunctions.NormalizeAddress(content.Title);
                        CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "ثبت پیام تماس با مای جدید در سایت", "مدیر گرامی پیام تماس بامای  جدیدی توسط ، " + ContactUs.FullName + "، در سایت Submited . شما میتوانید با مراجعه به پنل مدیریت آن را ببینید.لینک صفحه مربوط به کامنت : <br/> <a href='" + contentUrl + "'>" + content.Title + "</a><br/>پیام : <br/><p>" + ContactUs.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                        EmailBodyHtml = TfShop.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                        #endregion

                        //#region SendMail
                        //var emails = context.AdministratorPermissions.Include(x => x.User).Where(x => x.ModuleId == 4 && x.NotificationEmail == true).Select(x => x.User.Email).Distinct();

                        //EmailService es = new EmailService();
                        //await es.SendMultiDestinationAsync(" ثبت پیام تماس با مای جدید در سایت ", EmailBodyHtml, emails.ToList());


                        //#endregion

                        return Json(new
                        {
                            Message = "ارسال شد",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addcontactus", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "ارسال شد...",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
        }



        [HttpPost]
        public virtual async Task<JsonResult> AddComment(TfShop.ViewModels.Api.Mag.singleComment ocomment)
        {
            try
            {
                // if (Request.Form["addCommentBtn"] == null)
                if (false)
                {
                    return Json(new
                    {
                        Message = "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if (!TfShop.Infrastructure.Helper.Recaptcha.ReCaptchaPassed(Request.Form["addCommentBtn"]))
                    if (false)
                    {
                        return Json(new
                        {
                            Message = "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        context.Comments.Add(new Comment()
                        {

                            FullName = CommonFunctions.CorrectArabianLetter(ocomment.FullName),
                            Message = ocomment.Message,
                            Email = ocomment.Email,
                            ContentId = ocomment.ContentId,
                            ParrentId = ocomment.ParrentId,
                            InsertDate = DateTime.Now,
                            IsActive = false,
                            NegativeRating = 0,
                            PositiveRating = 0,
                            Visited = false
                        });
                        await context.SaveChangesAsync();


                        try
                        {
                            var content = context.Contents.Find(ocomment.ContentId);
                            //Send Mail To Related Admin
                            #region Create Html Body
                            string EmailBodyHtml = "";

                            var oSetting = context.Settings.Include(x => x.attachment).FirstOrDefault();
                            string contentUrl = "https://www.tfshops.com/tfmag/blog/" + ocomment.ContentId + "/" + CommonFunctions.NormalizeAddress(content.Title);
                            CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "ثبت کامنت جدید در سایت", "مدیر گرامی نظر جدیدی توسط ، " + ocomment.FullName + "، در سایت ثبت شد . شما میتوانید با مراجعه به پنل مدیریت آن را دیده و تایید نمایید .لینک صفحه مربوط به کامنت : <br/> <a href='" + contentUrl + "'>" + content.Title + "</a><br/> نظر : <br/> <p>" + ocomment.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                            EmailBodyHtml = TfShop.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                            #endregion

                            //#region SendMail
                            //var emails = context.AdministratorPermissions.Include(x => x.User).Where(x => x.ModuleId == 4 && x.NotificationEmail == true).Select(x => x.User.Email).Distinct();

                            //EmailService es = new EmailService();
                            //await es.SendMultiDestinationAsync(" ثبت کامنت جدید در سایت ", EmailBodyHtml, emails.ToList());

                            //#endregion
                        }
                        catch (Exception)
                        {

                        }

                        return Json(new
                        {
                            Message = "ثبت شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);


                    }
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addcomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual async Task<JsonResult> AddRate(int ContentId, double value)
        {
            UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            userActivitiesObj.SaveUserActivity(null, ContentId);
            try
            {
                if (context.Contents.Find(ContentId) != null)
                {
                    ContentRating cr = context.ContentRatings.Find(ContentId);
                    if (cr != null)
                    {
                        cr.Rating += value;
                        cr.TotalRaters++;
                        cr.AverageRating = Math.Round(Convert.ToDouble(cr.Rating * 1.0 / cr.TotalRaters * 1.0), 2);
                        await context.SaveChangesAsync();

                        return Json(new
                        {
                            TotalRaters = cr.TotalRaters,
                            AverageRating = cr.AverageRating,
                            Message = "ثبت شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ContentRating newCr = new ContentRating();

                        newCr.Rating = value;
                        newCr.TotalRaters = 1;
                        newCr.AverageRating = value;
                        newCr.ContentID = ContentId;
                        context.ContentRatings.Add(newCr);
                        await context.SaveChangesAsync();


                        return Json(new
                        {
                            TotalRaters = newCr.TotalRaters,
                            AverageRating = newCr.AverageRating,
                            Message = "ثبت شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }



                }
                else
                {
                    return Json(new
                    {
                        Message = "خطا.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (System.Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addrate", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                return Json(new
                {
                    Message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion

        #region Basket
        private BasketStep1V2 GetBasket(List<Basket> BasketItems, short step, string userid, int? cityId = null, bool extraprice = false)
        {
            try
            {
                if (BasketItems == null)
                    return null;
                if (BasketItems.Any())
                {
                    BasketStep1V2 basketStep1 = new BasketStep1V2();
                    //محصولاتِ در سبد
                    basketStep1.ProductBasketItems = uow.ProductPriceRepository.ProductBasketItemV2(BasketItems);

                    if (basketStep1.ProductBasketItems == null)
                        return new BasketStep1V2() { NotvalidBox = true };

                    foreach (var item in basketStep1.ProductBasketItems)
                    {
                        BasketItems.Where(x => x.ProductPriceId == item.Id).First().Price = item.rawprice;
                        if (BasketItems.Where(x => x.ProductPriceId == item.Id).First().Quantity == 0)
                            BasketItems.Remove(BasketItems.Where(x => x.ProductPriceId == item.Id).First());
                    }

                    //باکس های قابل انتخاب محصولات
                    basketStep1.ProductBoxes = uow.ProductPriceRepository.GetBasketSendwayBoxesv22(BasketItems);
                    if (basketStep1.ProductBoxes == null)
                        return new BasketStep1V2() { NotvalidBox = true };

                    var freeProductSendWay = uow.ProductSendWayRepository.Get(x => x, x => x.FreeOff).FirstOrDefault();

                    //هزینه و روش های ارسال باکس های محصولات در سبد
                    if (userid != null)
                    {
                        int? userCityid = null;
                        if (cityId.HasValue)
                            userCityid = cityId;
                        else if (uow.UserRepository.GetByID(userid).CityId.HasValue)
                            userCityid = uow.UserRepository.GetByID(userid).CityId;
                        else if (!userCityid.HasValue)
                            userCityid = 304;//تهران


                        foreach (var item in basketStep1.ProductBoxes)
                        {
                            List<int> productIds = uow.ProductPriceRepository.Get(x => x.Product.Id, x => item.ProductPriceIdList.Contains(x.Id), null, "Product").ToList();
                            var extraPrice = basketStep1.ProductBasketItems.Max(x => x.extraprice);
                            item.sendWayBoxPrices = new List<SendWayBoxPriceVm>();
                            var sendways = uow.ProductSendWayRepository.GetQueryList().Include("ProductSendWaySelects").Include("ProductSendWayBoxes").Include("Image").AsNoTracking().Where(a => a.IsActive && ((a.FreeOff && a.ProductSendWayBoxes.Any(x => x.SendWayBoxID == item.SendwayBox.Id)) || (a.IsActive && a.ProductSendWayBoxes.Any(x => x.SendWayBoxID == item.SendwayBox.Id))));
                            if (userCityid == 304)
                                sendways = sendways.Where(x => x.DisableTehran == false);
                            foreach (var prItem in productIds)
                            {
                                sendways = sendways.Where(a => a.ProductSendWaySelects.Any(q => q.ProductId == prItem));
                            }
                            if (!sendways.Any())
                                sendways = uow.ProductSendWayRepository.GetQueryList().Include("ProductSendWaySelects").Include("ProductSendWayBoxes").Include("Image").AsNoTracking().Where(a => a.IsActive && ((a.FreeOff && a.ProductSendWayBoxes.Any(x => x.SendWayBoxID == item.SendwayBox.Id)) || (a.IsActive && a.ProductSendWayBoxes.Any(x => x.SendWayBoxID == item.SendwayBox.Id))));

                            //delete normal peiyk if there is alopeyk
                            bool existalopeik = sendways.Any(x => x.Id == 11);

                            foreach (var item2 in sendways.Where(x => existalopeik == true ? x.Id != 2 : 1 == 1).ToList())
                            {
                                bool freesend = false;
                                if (step == 1)
                                {
                                    int? cost = uow.ProductPriceRepository.GetSendwayCostV2(item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id, item2.Id, basketStep1.ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), 0, out freesend, basketStep1.ProductBasketItems.Select(x => Convert.ToInt32(x.ProductPackageType)).ToList());
                                    if (cost.HasValue)
                                    {
                                        if (cost.Value >= 0)
                                        {
                                            item.sendWayBoxPrices.Add(new SendWayBoxPriceVm()
                                            {
                                                productSendWay = new ProductSendWayVM() { DeliverSelectable = item2.DeliverSelectable, Title = item2.Title, DeliverSelectDescr = item2.DeliverSelectDescr, DeliveryHourDay = item2.DeliveryHourDay, DeliveryStartDay = item2.DeliveryHourDay, DeliveryStartHour = item2.DeliveryHourDay, Descr = item2.Descr, DisableTehran = item2.DisableTehran, DisplaySort = item2.DisplaySort, ExtraPriceTitle = item2.ExtraPriceTitle, ExtraPriceValue = item2.ExtraPriceValue, FreeOff = item2.FreeOff, HasExtraPrice = item2.HasExtraPrice, HasExtraPrice2 = item2.HasExtraPrice2, Id = item2.Id, Image = item2.Image != null ? item2.Image.FileName : "", InvoiceType = item2.InvoiceType, IsActive = item2.IsActive, IsDefault = item2.IsDefault, IsFree = item2.IsFree, IsIrPost = item2.IsIrPost, PasKeraye = item2.PasKeraye },
                                                cost = item2.PasKeraye ? 0 : cost.Value,
                                                PasKeraye = item2.PasKeraye,
                                                IsDefault = item2.IsDefault,
                                                Id = item2.Id,
                                                BoxMass = item.SendwayBox.Height * item.SendwayBox.Width * item.SendwayBox.Lenght,
                                                FreeSend = freesend,
                                                show = item2.Id == 7 ? false : true
                                            });

                                            basketStep1.ProductBoxes.Where(x => x.SendwayBox.Id == item.SendwayBox.Id).First().sendWayBoxPrices = item.sendWayBoxPrices;
                                        }
                                    }


                                }
                                else
                                {
                                    int Extraprice2 = item2.HasExtraPrice2 ? item2.ExtraPriceValue : 0;
                                    int InsuranceCost = (item2.HasExtraPrice && extraprice ? Convert.ToInt32(Math.Ceiling((basketStep1.ProductBasketItems.Sum(x => x.finalPrice) * 0.005) * 0.001) * 1000) : 0);
                                    InsuranceCost = InsuranceCost > 0 ? InsuranceCost > 100000 ? 100000 : InsuranceCost < 15000 ? 15000 : InsuranceCost : 0;
                                    int PackageCost = (item2.HasExtraPrice ? Convert.ToInt32(Math.Ceiling((extraPrice + uow.SendwayBoxRepository.GetByID(item.SendwayBox.Id).BoxPrice) * 0.001) * 1000) : 0);

                                    int? cost = uow.ProductPriceRepository.GetSendwayCostV2(item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id, item2.Id, basketStep1.ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), PackageCost, out freesend, basketStep1.ProductBasketItems.Select(x => Convert.ToInt32(x.ProductPackageType)).ToList()) + InsuranceCost + PackageCost + Extraprice2;

                                    if (cost.HasValue)
                                    {
                                        if (cost.Value >= 0)
                                        {
                                            item.sendWayBoxPrices.Add(new SendWayBoxPriceVm()
                                            {
                                                Id = item2.Id,
                                                productSendWay = new ProductSendWayVM() { DeliverSelectable = item2.DeliverSelectable, Title = item2.Title, DeliverSelectDescr = item2.DeliverSelectDescr, DeliveryHourDay = item2.DeliveryHourDay, DeliveryStartDay = item2.DeliveryHourDay, DeliveryStartHour = item2.DeliveryHourDay, Descr = item2.Descr, DisableTehran = item2.DisableTehran, DisplaySort = item2.DisplaySort, ExtraPriceTitle = item2.ExtraPriceTitle, ExtraPriceValue = item2.ExtraPriceValue, FreeOff = item2.FreeOff, HasExtraPrice = item2.HasExtraPrice, HasExtraPrice2 = item2.HasExtraPrice2, Id = item2.Id, Image = item2.Image != null ? item2.Image.FileName : "", InvoiceType = item2.InvoiceType, IsActive = item2.IsActive, IsDefault = item2.IsDefault, IsFree = item2.IsFree, IsIrPost = item2.IsIrPost, PasKeraye = item2.PasKeraye },
                                                cost = item2.PasKeraye ? 0 : cost.Value,
                                                PasKeraye = item2.PasKeraye,
                                                IsDefault = item2.IsDefault,
                                                BoxMass = item.SendwayBox.Height * item.SendwayBox.Width * item.SendwayBox.Lenght,
                                                deliveryDateTimes = item2.DeliverSelectable ? GetDeliveryDateTimes(item2.Id, item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id) : null,
                                                DeliverSelectDescr = item2.DeliverSelectDescr,
                                                DeliverSelectable = item2.DeliverSelectable,
                                                FreeSend = freesend,
                                                InsuranceCost = InsuranceCost,
                                                PackageCost = PackageCost,
                                                Extraprice2Cost = Extraprice2,
                                                DisplaySort = item2.DisplaySort,
                                                show = item2.Id == 7 ? false : true
                                            });

                                            basketStep1.ProductBoxes.Where(x => x.SendwayBox.Id == item.SendwayBox.Id).First().sendWayBoxPrices = item.sendWayBoxPrices;
                                        }
                                    }
                                }
                            }
                        }

                        if (basketStep1.ProductBoxes.Any(a => !a.sendWayBoxPrices.Any()))
                        {
                            foreach (var item in basketStep1.ProductBoxes)
                            {
                                List<int> productIds = uow.ProductPriceRepository.Get(x => x.Product.Id, x => item.ProductPriceIdList.Contains(x.Id), null, "Product").ToList();
                                var extraPrice = basketStep1.ProductBasketItems.Max(x => x.extraprice);
                                item.sendWayBoxPrices = new List<SendWayBoxPriceVm>();
                                var sendways = uow.ProductSendWayRepository.GetQueryList().Include("ProductSendWaySelects").Include("ProductSendWayBoxes").Include("Image").AsNoTracking().Where(a => (a.FreeOff && a.ProductSendWayBoxes.Any(x => x.SendWayBoxID == item.SendwayBox.Id)) || (a.IsActive && a.ProductSendWayBoxes.Any(x => x.SendWayBoxID == item.SendwayBox.Id)));
                                if (userCityid == 304)
                                    sendways = sendways.Where(x => x.DisableTehran == false);
                                foreach (var item2 in sendways)
                                {
                                    bool freesend = false;
                                    if (step == 1)
                                    {
                                        int? cost = uow.ProductPriceRepository.GetSendwayCostV2(item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id, item2.Id, basketStep1.ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), 0, out freesend, basketStep1.ProductBasketItems.Select(x => x.ProductPackageType).ToList());
                                        if (cost.HasValue)
                                        {
                                            if (cost.Value >= 0)
                                            {
                                                item.sendWayBoxPrices.Add(new SendWayBoxPriceVm()
                                                {
                                                    productSendWay = new ProductSendWayVM() { DeliverSelectable = item2.DeliverSelectable, Title = item2.Title, DeliverSelectDescr = item2.DeliverSelectDescr, DeliveryHourDay = item2.DeliveryHourDay, DeliveryStartDay = item2.DeliveryHourDay, DeliveryStartHour = item2.DeliveryHourDay, Descr = item2.Descr, DisableTehran = item2.DisableTehran, DisplaySort = item2.DisplaySort, ExtraPriceTitle = item2.ExtraPriceTitle, ExtraPriceValue = item2.ExtraPriceValue, FreeOff = item2.FreeOff, HasExtraPrice = item2.HasExtraPrice, HasExtraPrice2 = item2.HasExtraPrice2, Id = item2.Id, Image = item2.Image != null ? item2.Image.FileName : "", InvoiceType = item2.InvoiceType, IsActive = item2.IsActive, IsDefault = item2.IsDefault, IsFree = item2.IsFree, IsIrPost = item2.IsIrPost, PasKeraye = item2.PasKeraye },
                                                    cost = item2.PasKeraye ? 0 : cost.Value,
                                                    PasKeraye = item2.PasKeraye,
                                                    IsDefault = item2.IsDefault,
                                                    Id = item2.Id,
                                                    BoxMass = item.SendwayBox.Height * item.SendwayBox.Width * item.SendwayBox.Lenght,
                                                    FreeSend = freesend,
                                                    show = item2.Id == 7 ? false : true
                                                });

                                                basketStep1.ProductBoxes.Where(x => x.SendwayBox.Id == item.SendwayBox.Id).First().sendWayBoxPrices = item.sendWayBoxPrices;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        int Extraprice2 = item2.HasExtraPrice2 ? item2.ExtraPriceValue : 0;
                                        int InsuranceCost = (item2.HasExtraPrice && extraprice ? Convert.ToInt32(Math.Ceiling((basketStep1.ProductBasketItems.Sum(x => x.finalPrice) * 0.005) * 0.001) * 1000) : 0);
                                        InsuranceCost = InsuranceCost > 0 ? InsuranceCost > 100000 ? 100000 : InsuranceCost < 15000 ? 15000 : InsuranceCost : 0;
                                        int PackageCost = (item2.HasExtraPrice ? Convert.ToInt32(Math.Ceiling((extraPrice + uow.SendwayBoxRepository.GetByID(item.SendwayBox.Id).BoxPrice) * 0.001) * 1000) : 0);

                                        int? cost = uow.ProductPriceRepository.GetSendwayCostV2(item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id, item2.Id, basketStep1.ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), PackageCost, out freesend, basketStep1.ProductBasketItems.Select(x => x.ProductPackageType).ToList()) + InsuranceCost + PackageCost + Extraprice2;

                                        if (cost.HasValue)
                                        {
                                            if (cost.Value >= 0)
                                            {
                                                item.sendWayBoxPrices.Add(new SendWayBoxPriceVm()
                                                {
                                                    Id = item2.Id,
                                                    productSendWay = new ProductSendWayVM() { DeliverSelectable = item2.DeliverSelectable, Title = item2.Title, DeliverSelectDescr = item2.DeliverSelectDescr, DeliveryHourDay = item2.DeliveryHourDay, DeliveryStartDay = item2.DeliveryHourDay, DeliveryStartHour = item2.DeliveryHourDay, Descr = item2.Descr, DisableTehran = item2.DisableTehran, DisplaySort = item2.DisplaySort, ExtraPriceTitle = item2.ExtraPriceTitle, ExtraPriceValue = item2.ExtraPriceValue, FreeOff = item2.FreeOff, HasExtraPrice = item2.HasExtraPrice, HasExtraPrice2 = item2.HasExtraPrice2, Id = item2.Id, Image = item2.Image != null ? item2.Image.FileName : "", InvoiceType = item2.InvoiceType, IsActive = item2.IsActive, IsDefault = item2.IsDefault, IsFree = item2.IsFree, IsIrPost = item2.IsIrPost, PasKeraye = item2.PasKeraye },
                                                    cost = item2.PasKeraye ? 0 : cost.Value,
                                                    PasKeraye = item2.PasKeraye,
                                                    IsDefault = item2.IsDefault,
                                                    BoxMass = item.SendwayBox.Height * item.SendwayBox.Width * item.SendwayBox.Lenght,
                                                    deliveryDateTimes = item2.DeliverSelectable ? GetDeliveryDateTimes(item2.Id, item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id) : null,
                                                    DeliverSelectDescr = item2.DeliverSelectDescr,
                                                    DeliverSelectable = item2.DeliverSelectable,
                                                    FreeSend = freesend,
                                                    InsuranceCost = InsuranceCost,
                                                    Extraprice2Cost = Extraprice2,
                                                    PackageCost = PackageCost,
                                                    DisplaySort = item2.DisplaySort,
                                                    show = item2.Id == 7 ? false : true
                                                });

                                                basketStep1.ProductBoxes.Where(x => x.SendwayBox.Id == item.SendwayBox.Id).First().sendWayBoxPrices = item.sendWayBoxPrices;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (!basketStep1.ProductBoxes.Any(a => a.sendWayBoxPrices.Any(b => b.FreeSend == true)))
                        {
                            bool freesend = false;
                            int InsuranceCost = 0;
                            InsuranceCost = 0;
                            int PackageCost = 0;
                            int Extraprice2 = 0;
                            int? cost = uow.ProductPriceRepository.GetSendwayCostV2(basketStep1.ProductBasketItems.Select(x => x.Id).ToList(), userCityid.Value, basketStep1.ProductBasketItems.First().SenwayBoxId.HasValue ? basketStep1.ProductBasketItems.First().SenwayBoxId.Value : 0, freeProductSendWay.Id, basketStep1.ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), PackageCost, out freesend, basketStep1.ProductBasketItems.Select(x => x.ProductPackageType).ToList()) + InsuranceCost + PackageCost + Extraprice2;

                            if (freesend)
                            {
                                if (cost >= 0)
                                {
                                    foreach (var item in basketStep1.ProductBoxes)
                                    {
                                        basketStep1.ProductBoxes.Where(x => x.SendwayBox.Id == item.SendwayBox.Id).First().sendWayBoxPrices.Add(new SendWayBoxPriceVm()
                                        {
                                            Id = freeProductSendWay.Id,
                                            productSendWay = new ProductSendWayVM() { DeliverSelectable = freeProductSendWay.DeliverSelectable, Title = freeProductSendWay.Title, DeliverSelectDescr = freeProductSendWay.DeliverSelectDescr, DeliveryHourDay = freeProductSendWay.DeliveryHourDay, DeliveryStartDay = freeProductSendWay.DeliveryHourDay, DeliveryStartHour = freeProductSendWay.DeliveryHourDay, Descr = freeProductSendWay.Descr, DisableTehran = freeProductSendWay.DisableTehran, DisplaySort = freeProductSendWay.DisplaySort, ExtraPriceTitle = freeProductSendWay.ExtraPriceTitle, ExtraPriceValue = freeProductSendWay.ExtraPriceValue, FreeOff = freeProductSendWay.FreeOff, HasExtraPrice = freeProductSendWay.HasExtraPrice, HasExtraPrice2 = freeProductSendWay.HasExtraPrice2, Id = freeProductSendWay.Id, Image = freeProductSendWay.Image != null ? freeProductSendWay.Image.FileName : "", InvoiceType = freeProductSendWay.InvoiceType, IsActive = freeProductSendWay.IsActive, IsDefault = freeProductSendWay.IsDefault, IsFree = freeProductSendWay.IsFree, IsIrPost = freeProductSendWay.IsIrPost, PasKeraye = freeProductSendWay.PasKeraye },
                                            cost = 0,
                                            PasKeraye = freeProductSendWay.PasKeraye,
                                            IsDefault = freeProductSendWay.IsDefault,
                                            BoxMass = item.SendwayBox.Height * item.SendwayBox.Width * item.SendwayBox.Lenght,
                                            deliveryDateTimes = freeProductSendWay.DeliverSelectable ? GetDeliveryDateTimes(freeProductSendWay.Id, item.ProductPriceIdList, userCityid.Value, item.SendwayBox.Id) : null,
                                            DeliverSelectDescr = freeProductSendWay.DeliverSelectDescr,
                                            DeliverSelectable = freeProductSendWay.DeliverSelectable,
                                            FreeSend = true,
                                            InsuranceCost = 0,
                                            PackageCost = 0,
                                            Extraprice2Cost = 0,
                                            DisplaySort = freeProductSendWay.DisplaySort,
                                            show = freeProductSendWay.Id == 7 ? false : true

                                        });
                                    }
                                }
                            }
                        }
                    }

                    //delete normal peiyk if there is alopeyk
                    ViewBag.FreesendSendway = freeProductSendWay;

                    List<int> ids = BasketItems.Select(x => x.ProductId).ToList();
                    int countIds = ids.Count;

                    basketStep1.Sellers = uow.ProductSellerRepository.GetQueryList().Include(x => x.Seller.User).AsNoTracking()
                        .Where(ps => ids.Contains(ps.ProductId) && ps.Seller.IsActive).GroupBy(ps => ps.Seller).Where(g => g.Count() == countIds).Select(g => g.Key).Include(x => x.User).Select(x => new SellerVM() { Id = x.Id, IsActive = x.IsActive }).ToList();


                    return basketStep1;

                }
                else
                {
                    TfShop.Infrastructure.EventLog.Logger.Add(5, "api", "GetBasket", false, 500, "سبد خالی ست", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                    return null;
                }
            }
            catch (Exception ex)
            {
                TfShop.Infrastructure.EventLog.Logger.Add(5, "api", "GetBasket", false, 500, ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                return null;
            }
        }
        private List<DeliveryDateTime> GetDeliveryDateTimes(int ProductSendwayId, List<int> ProductPriceIds, int city, int SendwayBoxId)
        {
            List<DeliveryDateTime> DeliveryDateTimes = new List<DeliveryDateTime>();
            try
            {

                #region محاسبه تاریخ و زمان شروع بازه
                //حداکثر تاریخ تحویل محصولات انتخاب شده
                int maxProductDeliveryTimeout = uow.ProductPriceRepository.Max(x => x.DeliveryTimeout, x => ProductPriceIds.Contains(x.Id));
                DateTime T1 = DateTime.Now.AddDays(maxProductDeliveryTimeout);
                // به علاوه مدت زمان انتظار روش ارسال
                var ProductSendWayDeliveryTimeout = uow.ProductSendWayRepository.Get(x => new { x.Id, x.DeliveryStartDay, x.DeliveryStartHour, x.DeliveryHourDay, x.ProductSendWayWorkTimes }, x => x.Id == ProductSendwayId, null, "ProductSendWayWorkTimes").FirstOrDefault();
                T1 = T1.AddDays(ProductSendWayDeliveryTimeout.DeliveryStartDay).AddHours(ProductSendWayDeliveryTimeout.DeliveryStartHour);


                DateTime T2 = DateTime.Now, T3 = T1;


                //اگر غیر از تهران است یک روز اضافه کن
                if (uow.CityRepository.GetByID(city).ProvinceId != 8 && ProductSendwayId == 10)
                {
                    T2 = T2.AddDays(1);
                    T1 = T1.AddDays(1);
                    T3 = T1;
                    TfShop.Infrastructure.EventLog.Logger.Add(5, "Cart", "add day karaj", true, 500, "Add Day karaj", System.DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                }

                short TodayDay = (short)T2.PersionDayOfWeek();
                var Todaytime = T2.TimeOfDay;
                //ساعت کاری امروز تمام شده ؟
                if (!ProductSendWayDeliveryTimeout.ProductSendWayWorkTimes.Any(x => x.WeekDay == TodayDay && x.EndTime > Todaytime))
                {
                    T2 = T2.AddDays(1);
                    T1 = T1.AddDays(1);
                    T3 = T1;
                }
                else
                {
                    // T1 = T1.AddDays(1);
                    TodayDay = (short)T2.PersionDayOfWeek();
                    if (!ProductSendWayDeliveryTimeout.ProductSendWayWorkTimes.Any(x => x.WeekDay == TodayDay)) // اگر این روز جزو روزهای کاری نیست
                    {
                        T2 = T2.AddDays(1);
                        T1 = T1.AddDays(1);
                        T3 = T1;
                    }
                }
                //بررسی روزهای تعطیلی فروشگاه
                while (T2 < T1)
                {
                    if (ProductSendWayDeliveryTimeout.Id == 10)
                    {
                        if (uow.HolidayRepository.Get(x => x, x => x.InPersonDelivery == false && x.InsertDate == T2.Date).Any())
                            T1 = T1.AddDays(1);
                    }
                    else
                    {
                        if (uow.HolidayRepository.Get(x => x, x => x.InsertDate == T2.Date).Any())
                            T1 = T1.AddDays(1);
                    }
                    T2 = T2.AddDays(1);
                }
                //بررسی روزهای کاری روش ارسال
                short T1WeekDay = (short)T1.PersionDayOfWeek();
                if (!ProductSendWayDeliveryTimeout.ProductSendWayWorkTimes.Any(x => x.WeekDay == T1WeekDay)) // اگر این روز جزو روزهای کاری نیست
                {
                    //اولین روز کاری بعد از این را انتخاب کن
                    var T1WeekDayNext = ProductSendWayDeliveryTimeout.ProductSendWayWorkTimes.Where(x => x.WeekDay > T1WeekDay).OrderBy(x => x.WeekDay).FirstOrDefault();
                    if (T1WeekDayNext != null)
                        T1 = T1.AddDays((T1WeekDayNext.WeekDay - T1WeekDay));

                }
                #endregion

                var ddd = (T1.AddDays(ProductSendWayDeliveryTimeout.DeliveryHourDay).Date - DateTime.Now.Date).TotalDays;
                var ddd2 = ProductSendWayDeliveryTimeout.DeliveryHourDay;
                #region ساخت لیست بازه زمانی برای انتخاب کاربر
                // پیمایش تا انتهای روز انتخابی برای روش ارسال
                for (int i = 0; i <= (T3 != T1 ? (T1.AddDays(ProductSendWayDeliveryTimeout.DeliveryHourDay).Date - DateTime.Now.Date).TotalDays : ProductSendWayDeliveryTimeout.DeliveryHourDay); i++)
                {
                    // چند شنبه ؟
                    short dayofweek = (short)T3.PersionDayOfWeek();
                    TimeSpan currenttime = DateTime.Now.AddMinutes(30).TimeOfDay;
                    short todayDayofweek = (short)DateTime.Now.PersionDayOfWeek();
                    var PswTimes = ProductSendWayDeliveryTimeout.ProductSendWayWorkTimes.Where(x => x.WeekDay == dayofweek && x.IsActive);
                    if (dayofweek == todayDayofweek)
                        PswTimes = ProductSendWayDeliveryTimeout.ProductSendWayWorkTimes.Where(x => x.WeekDay == dayofweek && x.IsActive && x.StartTime > currenttime);

                    bool offday = false;
                    string offdaytitle = "";
                    //پیمایش زمان های روز بدست آمده
                    foreach (var item in PswTimes)
                    {
                        if (ProductSendWayDeliveryTimeout.Id == 10)
                        {
                            if (uow.HolidayRepository.Get(x => x, x => x.InPersonDelivery == false && x.InsertDate == T3.Date).Any())
                            {
                                offday = true;
                                offdaytitle = uow.HolidayRepository.Get(x => x, x => x.InPersonDelivery == false && x.InsertDate == T3.Date).First().Description;
                            }
                            else
                                offday = false;
                        }
                        else
                        {
                            if (uow.HolidayRepository.Get(x => x, x => x.InsertDate == T3.Date).Any())
                            {
                                offday = true;
                                offdaytitle = uow.HolidayRepository.Get(x => x, x => x.InsertDate == T3.Date).First().Description;
                            }
                            else
                                offday = false;
                        }
                        int? limitation = uow.ProductSendWayDetailRepository.Get(x => x.Limitation, x => x.ProductSendWayBoxId == SendwayBoxId && x.CityId == city).FirstOrDefault();
                        DateTime date = T3.Date;

                        DeliveryDateTimes.Add(new DeliveryDateTime
                        {
                            Id = item.Id,
                            offDay = offday,
                            offDaytitle = offdaytitle,
                            dateTime = T3.Date,
                            starttime = item.StartTime,
                            endtime = item.EndTime,
                            completionCapacity = limitation.HasValue ? limitation.Value > 0 ? uow.OrderDeliveryRepository.Count(x => x.ProductSendWayWorkTimeId == item.Id && x.RequestDate == date) >= limitation : false : false
                        });


                    }

                    T3 = T3.AddDays(1);

                }
                #endregion
            }
            catch (Exception ex)
            {
                TfShop.Infrastructure.EventLog.Logger.Add(5, "api", "GetDeliveryDateTimes", true, 500, "Error: " + ex.Message, System.DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

            }

            return DeliveryDateTimes;
        }
        private BasketStep2VM GetBasketStep2(List<Basket> BasketItems, string userid, int? cityId = null, bool extraprice = false)
        {
            try
            {

                var usr = uow.UserRepository.Get(x => new { userid = x.Id, CityEntity = x.CityEntity, address = x.Address, x.PostalCode, phonenumber = x.PhoneNumber, landlinephone = x.LandlinePhone, fullname = x.FirstName + " " + x.LastName }, x => x.Id == userid, null, "CityEntity.Province").Single();
                BasketStep2VM BasketStep2 = new BasketStep2VM();

                BasketStep2.basketStep1 = GetBasket(BasketItems, 2, userid, cityId, extraprice);

                // قبلاً اینجا `x => new UserAddressVM()` بود — یعنی هیچ فیلدی map نمی‌شد و همه‌ی
                // آدرس‌های کاربر با فیلدهای خالی برمی‌گشتن. الان دقیقاً همون فیلدهایی که
                // _Step2.cshtml لازم داره (شامل نام استان که قبلاً هم توی UserAddressVM اصلاً
                // فیلد نداشت) map می‌شن.
                BasketStep2.UserAddresses = uow.UserAddressRepository.Get(x => new UserAddressVM
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    LandlinePhone = x.LandlinePhone,
                    PostalCode = x.PostalCode,
                    CityId = x.CityId,
                    CityEntity = x.CityId.HasValue ? x.CityEntity.Name : null,
                    ProvinceName = x.CityId.HasValue ? x.CityEntity.Province.Name : null,
                    Address = x.Address,
                    AddressNumber = x.AddressNumber,
                    AddressUnit = x.AddressUnit,
                    FooterGoogleMapLongitude = x.FooterGoogleMapLongitude,
                    FooterGoogleMapLatitude = x.FooterGoogleMapLatitude,
                    UserId = x.UserId,
                    Title = x.Title
                }, x => x.UserId == userid, x => x.OrderBy(s => s.CityId).ThenByDescending(s => s.Id), "CityEntity,CityEntity.Province").ToList();
                BasketStep2.UserDefaultAddress = new UserAddressVM()
                {
                    Address = usr.address,
                    FullName = usr.fullname,
                    LandlinePhone = usr.landlinephone,
                    PhoneNumber = usr.phonenumber,
                    PostalCode = usr.PostalCode,
                    UserId = userid

                };
                if (cityId.HasValue)
                {
                    BasketStep2.UserDefaultAddress.CityId = usr.CityEntity.Id;
                    BasketStep2.UserDefaultAddress.CityEntity = usr.CityEntity.Name;
                    BasketStep2.UserDefaultAddress.ProvinceName = usr.CityEntity.Province?.Name;
                }
                BasketStep2.UserAddresses.Add(BasketStep2.UserDefaultAddress);

                return BasketStep2;
            }
            catch (Exception ex)
            {
                TfShop.Infrastructure.EventLog.Logger.Add(5, "Cart", "GetBasketStep2", false, 500, ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }

        }

        // معادل JSON اکشن قدیمی CartController.shipping (Step2 چک‌آوت: انتخاب آدرس/روش ارسال).
        // BasketItems عیناً مثل GetCart/MergeOrder توی بدنه‌ی JSON پست می‌شه (نه از کوکی سمت
        // سرور خونده می‌شه، چون فرانت جدید سبد رو توی localStorage نگه می‌داره).
        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> Shipping(List<Basket> BasketItems, int? aid)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new
                {
                    url = "/account/login?returnUrl=/cart/shipping",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);

            try
            {
                if (BasketItems == null || !BasketItems.Any())
                    return Json(new { url = "/cart", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                // معادل چک UserHasAnyPersonalInfo توی CartController.shipping: بدون نام/تاییدشدن
                // موبایل، اجازه‌ی ادامه‌ی خرید نیست.
                if (!await uow.UserAddressRepository.UserHasAnyPersonalInfo(userid))
                    return Json(new
                    {
                        url = "/profile/edit?returnurl=/cart/shipping",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                bool notAddress = !await uow.UserAddressRepository.UserHasAnyAddress(userid);

                // معادل تشخیص cityid توی CartController.shipping: یا از آدرس صریحاً انتخاب‌شده
                // (aid)، یا از شهر پیش‌فرض پروفایل کاربر، یا از اولین آدرس ثبت‌شده‌ش.
                int? cityid = null;
                if (aid.HasValue)
                {
                    cityid = uow.UserAddressRepository.GetByID(aid.Value)?.CityId;
                }
                else if (!notAddress)
                {
                    var userCityAddress = uow.UserRepository.Get(x => new { x.CityEntity, x.Address }, x => x.Id == userid, null, "CityEntity").SingleOrDefault();
                    if (userCityAddress?.CityEntity == null || string.IsNullOrEmpty(userCityAddress.Address))
                    {
                        var firstAddress = uow.UserAddressRepository.Get(x => x, x => x.UserId == userid, null, "CityEntity").FirstOrDefault();
                        cityid = firstAddress?.CityId;
                    }
                    else
                    {
                        cityid = userCityAddress.CityEntity.Id;
                    }
                }

                var step2 = GetBasketStep2(BasketItems, userid, cityid, false);
                if (step2 == null || step2.basketStep1 == null)
                    return Json(new { url = "/cart", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                // معادل "return View(\"boxNotValid\")": فرانت با همین statusCode تشخیص می‌ده و
                // پیام/صفحه‌ی مناسب رو نشون می‌ده.
                if (step2.basketStep1.NotvalidBox)
                    return Json(new { statusCode = 501 }, JsonRequestBehavior.AllowGet);

                // روش ارسال رایگان (برای بنر/گزینه‌ی ارسال رایگان توی _Sendway) — با پروجکشن lean
                // (نه entity خام Domain.ProductSendWay) که همون الگوی GetBasket برای ProductSendWayVM هست.
                var freesendSendwayEntity = uow.ProductSendWayRepository.GetQueryList().AsNoTracking().Include("Image").Where(x => x.IsFree).FirstOrDefault();
                var freesendSendway = freesendSendwayEntity == null ? null : new ProductSendWayVM
                {
                    Id = freesendSendwayEntity.Id,
                    Title = freesendSendwayEntity.Title,
                    Descr = freesendSendwayEntity.Descr,
                    Image = freesendSendwayEntity.Image != null ? freesendSendwayEntity.Image.FileName : null,
                    DeliverSelectDescr = freesendSendwayEntity.DeliverSelectDescr,
                    DeliverSelectable = freesendSendwayEntity.DeliverSelectable,
                    HasExtraPrice = freesendSendwayEntity.HasExtraPrice,
                    HasExtraPrice2 = freesendSendwayEntity.HasExtraPrice2,
                    IsIrPost = freesendSendwayEntity.IsIrPost,
                    IsFree = freesendSendwayEntity.IsFree,
                    FreeOff = freesendSendwayEntity.FreeOff,
                    IsDefault = freesendSendwayEntity.IsDefault,
                    DisplaySort = freesendSendwayEntity.DisplaySort
                };

                // فقط مقدار «تا ارسال رایگان چقدر مونده» برگردونده می‌شه (طبق درخواست کاربر — فعلاً
                // جایی توی UI نمایش داده نمی‌شه، بعداً اضافه می‌شه).
                long remain = 0;
                if (cityid.HasValue && step2.basketStep1.ProductBasketItems.Any())
                {
                    // توجه: روی ProductBasketItemV2 فیلد ProductPackageType از نوع int هست (نه enum
                    // مثل نسخه‌ی قدیمی ProductBasketItem)، برای همین باید صریح cast بشه.
                    var packages = step2.basketStep1.ProductBasketItems.Select(x => (ProductPackageType)x.ProductPackageType).ToList();
                    remain = uow.ProductPriceRepository.CheckFreeSendway(
                        step2.basketStep1.ProductBasketItems.Select(x => x.Id).ToList(),
                        cityid.Value,
                        step2.basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity),
                        packages,
                        step2.basketStep1.ProductBasketItems.FirstOrDefault()?.SenwayBoxId ?? 0);
                }

                return Json(new
                {
                    aid = aid,
                    notAddress = notAddress,
                    userDefaultAddress = step2.UserDefaultAddress,
                    userAddresses = step2.UserAddresses,
                    basketStep1 = new
                    {
                        ProductBasketItems = step2.basketStep1.ProductBasketItems,
                        Sellers = step2.basketStep1.Sellers,
                        validBuyOneTimePerOffer = step2.basketStep1.validBuyOneTimePerOffer,
                        NotvalidBox = step2.basketStep1.NotvalidBox,
                        // ProductBoxes رو مستقیم پاس نمی‌دیم چون deliveryDateTimes توش DateTime/TimeSpan
                        // خام داره — JavaScriptSerializer پیش‌فرض این‌ها رو یا فرمت عجیب "/Date(...)/ "
                        // یا کل آبجکت TimeSpan سریالایز می‌کنه، نه یه رشته‌ی ساده‌ی قابل استفاده توی
                        // فرانت. این‌جا صریح به رشته تبدیل می‌شن.
                        ProductBoxes = step2.basketStep1.ProductBoxes.Select(box => new
                        {
                            box.SendwayBox,
                            box.ProductPriceIdList,
                            sendWayBoxPrices = box.sendWayBoxPrices.Select(price => new
                            {
                                price.Id,
                                price.productSendWay,
                                price.cost,
                                price.InsuranceCost,
                                price.PackageCost,
                                price.PasKeraye,
                                price.IsDefault,
                                price.DeliverSelectDescr,
                                price.DeliverSelectable,
                                price.BoxMass,
                                price.FreeSend,
                                price.show,
                                price.DisplaySort,
                                price.Extraprice2Cost,
                                deliveryDateTimes = price.deliveryDateTimes == null ? null : price.deliveryDateTimes.Select(d => new
                                {
                                    d.Id,
                                    d.offDay,
                                    d.offDaytitle,
                                    date = d.dateTime.ToString("yyyy-MM-dd"),
                                    startHour = d.starttime.ToString(@"hh\:mm"),
                                    endHour = d.endtime.ToString(@"hh\:mm"),
                                    d.completionCapacity
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    },
                    freesendSendway = freesendSendway,
                    remain = remain,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "shipping", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // معادل JSON اکشن قدیمی CartController.GotoStep3: اعتبارسنجی نهایی قبل از رفتن به مرحله‌ی
        // پرداخت (آیا بازه‌ی زمانی انتخابی هنوز خالیه، آیا موجودی/قیمت محصولات عوض نشده). برخلاف
        // نسخه‌ی قدیمی که shippings رو توی کوکی سمت سرور ذخیره می‌کرد، این‌جا فقط validate می‌کنه؛
        // فرانت (که سبد رو توی localStorage نگه می‌داره) خودش shippings رو روی آیتم‌ها ست می‌کنه.
        [HttpPost]
        [JWTAuthorize]
        public JsonResult GotoStep3(List<Basket> BasketItems, BasketShipping[] shippings)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new
                {
                    url = "/account/login?returnUrl=/cart/shipping",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);

            if (BasketItems == null || !BasketItems.Any() || shippings == null || !shippings.Any())
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                #region چک کردن تکمیل نشدن بازه زمانی انتخابی
                bool validDeliveryDate = true;
                var userCityid = uow.UserRepository.GetByID(userid)?.CityId;
                foreach (var item in shippings)
                {
                    var sendway = uow.ProductSendWayRepository.GetByID(item.sendwayId);
                    if (sendway != null && sendway.DeliverSelectable)
                    {
                        var wt = uow.ProductSendWayWorkTimeRepository.GetByID(item.sendwayWorktimeId);
                        if (wt != null)
                        {
                            DateTime dtSelect = Convert.ToDateTime(item.deliveryDate).Date.AddHours(wt.StartTime.TotalHours);
                            if (DateTime.Now.AddMinutes(30) >= dtSelect)
                            {
                                validDeliveryDate = false;
                            }
                            else
                            {
                                int? limitation = uow.ProductSendWayDetailRepository.Get(x => x.Limitation, x => x.ProductSendWayBoxId == item.sendwayBoxId && x.CityId == userCityid).FirstOrDefault();
                                if (limitation.HasValue && limitation > 0)
                                {
                                    DateTime dt = Convert.ToDateTime(item.deliveryDate);
                                    if (uow.OrderDeliveryRepository.Count(x => x.ProductSendWayWorkTimeId == item.sendwayWorktimeId && x.RequestDate == dt) >= limitation)
                                        validDeliveryDate = false;
                                }
                            }
                        }
                    }
                }
                if (!validDeliveryDate)
                    return Json(new
                    {
                        message = "متاسفانه دیگر امکان انتخاب این بازه زمانی تحویل وجود ندارد.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                #endregion

                #region چک کردن موجودی‌ها و تغییر قیمت/تعداد
                bool validExist = true, validprice = true, validQuantity = true, validOfferOnTimeBuy = true;
                var removeIds = new List<int>();
                foreach (var item in BasketItems)
                {
                    var prp = uow.ProductPriceRepository.Get(x => new { x.MaxBasketCount, x.Quantity, x.Price }, x => x.Id == item.ProductPriceId).FirstOrDefault();
                    if (prp == null)
                    {
                        removeIds.Add(item.ProductPriceId);
                        validExist = false;
                        continue;
                    }

                    var proffer = uow.ProductOfferRepository.Get(x => x, a => a.Quantity > 0 && a.Value > 0 && a.ProductPrice.ProductStateId < 3 && a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null));

                    if (uow.ProductPriceRepository.Any(x => x.Id, x => x.Id == item.ProductPriceId && x.ProductStateId > 2))
                    {
                        removeIds.Add(item.ProductPriceId);
                        validExist = false;
                    }
                    if (prp.Price != item.Price)
                        validprice = false;

                    if (proffer.Any())
                    {
                        int offerId = proffer.First().OfferId;
                        if (uow.OrderRowRepository.Get(x => x.Id, x => x.Order.UserId == userid && x.ProductPriceId == item.ProductPriceId && x.ProductOffer.Offer.CodeTypeValueCode == 1 && x.ProductOffer.OfferId == offerId && x.Order.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت) && !x.Order.OrderStates.Any(a => a.state == OrderStatus.لغو_شده), null, "ProductOffer,Order.OrderStates").Any())
                        {
                            validOfferOnTimeBuy = false;
                            removeIds.Add(item.ProductPriceId);
                        }

                        var proffercurrent = proffer.First();
                        int max = proffercurrent.MaxBasketCount < proffercurrent.Quantity ? proffercurrent.MaxBasketCount : proffercurrent.Quantity;
                        if (item.Quantity > max)
                            validQuantity = false;
                    }
                    else
                    {
                        if (prp.MaxBasketCount < item.Quantity || prp.Quantity < item.Quantity)
                            validQuantity = false;
                    }
                }

                if (!validExist || !validOfferOnTimeBuy)
                {
                    return Json(new
                    {
                        message = !validOfferOnTimeBuy
                            ? "یکی از محصولات موجود در سبد شما به علت خرید در تخفیف فعلی، حذف شد. پس از اتمام تخفیف می توانید دوباره از آن محصول خرید نمایید."
                            : "برخی از کالاهای شما ناموجود شدند. پس از بررسی، اقدام به ادامه فرآیند خرید نمایید.",
                        removeProductPriceIds = removeIds.Distinct().ToList(),
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!validprice)
                {
                    var correctedPrices = BasketItems.Select(item => new
                    {
                        ProductPriceId = item.ProductPriceId,
                        Price = uow.ProductPriceRepository.Get(x => x.Price, x => x.Id == item.ProductPriceId).First()
                    }).ToList();

                    return Json(new
                    {
                        message = "برخی از کالاهای شما تغییر قیمت داشته اند. پس از بررسی و در صورت تمایل اقدام به ادامه فرآیند خرید نمایید.",
                        correctedPrices = correctedPrices,
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!validQuantity)
                {
                    var correctedQuantities = new List<object>();
                    foreach (var item in BasketItems)
                    {
                        var prp = uow.ProductPriceRepository.Get(x => new { x.MaxBasketCount, x.Quantity }, x => x.Id == item.ProductPriceId).First();
                        var proffer = uow.ProductOfferRepository.Get(x => x, a => a.Quantity > 0 && a.Value > 0 && a.ProductPrice.ProductStateId < 3 && a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null));

                        int max;
                        bool ok;
                        if (proffer.Any())
                        {
                            var proffercurrent = proffer.First();
                            max = proffercurrent.MaxBasketCount < proffercurrent.Quantity ? proffercurrent.MaxBasketCount : proffercurrent.Quantity;
                            ok = item.Quantity <= max;
                        }
                        else
                        {
                            max = prp.MaxBasketCount < item.Quantity ? prp.MaxBasketCount : prp.Quantity;
                            ok = prp.MaxBasketCount >= item.Quantity && prp.Quantity >= item.Quantity;
                        }

                        if (!ok)
                            correctedQuantities.Add(new { ProductPriceId = item.ProductPriceId, Quantity = max });
                    }

                    return Json(new
                    {
                        message = "برخی از کالاهای شما تغییر موجودی داشته اند. پس از بررسی و در صورت تمایل اقدام به ادامه فرآیند خرید نمایید.",
                        correctedQuantities = correctedQuantities,
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                return Json(new { statusCode = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "gotostep3", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    message = "خطایی رخ داد !",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // معادل JSON اکشن قدیمی CartController.Payment/GetBasketStep3 (Step3 چک‌آوت: نمایش خلاصه‌ی
        // سفارش + انتخاب بانک برای پرداخت). طبق تصمیم فعلی، فقط مسیر «پرداخت اینترنتی بانکی» پیاده‌سازی
        // شده — کد تخفیف/بن و روش‌های دیگه (کارت‌به‌کارت/دیجی‌پی/ترب) فعلاً جزو این پاسخ نیستن.
        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> PaymentInfo(List<Basket> BasketItems, BasketShipping[] shippings, string codeGift, int? bonCount, int? bankid, int? paymentType)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { url = "/account/login?returnUrl=/cart/payment", statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                if (BasketItems == null || !BasketItems.Any())
                    return Json(new { url = "/cart", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                if (!await uow.UserAddressRepository.UserHasAnyPersonalInfo(userid))
                    return Json(new { url = "/profile/edit?returnurl=/cart/payment", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                if (shippings == null || !shippings.Any())
                    return Json(new { url = "/cart/shipping", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                // معادل چک بازه‌ی زمانی توی CartController.Payment/GotoStep3
                bool validDeliveryDate = true;
                var userCityid = uow.UserRepository.GetByID(userid)?.CityId;
                foreach (var item in shippings)
                {
                    var sendway = uow.ProductSendWayRepository.GetByID(item.sendwayId);
                    if (sendway != null && sendway.DeliverSelectable)
                    {
                        var wt = uow.ProductSendWayWorkTimeRepository.GetByID(item.sendwayWorktimeId);
                        if (wt == null || wt.ProductSendWayId != item.sendwayId)
                        {
                            validDeliveryDate = false;
                        }
                        else
                        {
                            DateTime dtSelect = Convert.ToDateTime(item.deliveryDate).Date.AddHours(wt.StartTime.TotalHours);
                            if (DateTime.Now.AddMinutes(30) >= dtSelect)
                                validDeliveryDate = false;
                        }
                    }
                }
                if (!validDeliveryDate)
                    return Json(new { url = "/cart?m=1", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                int? addressCityId = null;
                var firstAddressId = shippings.Where(x => x.addressId > 0).Select(x => (int?)x.addressId).FirstOrDefault();
                if (firstAddressId.HasValue)
                    addressCityId = uow.UserAddressRepository.GetByID(firstAddressId.Value)?.CityId;

                var basketStep1 = GetBasket(BasketItems, 2, userid, addressCityId);
                if (basketStep1 == null)
                    return Json(new { url = "/cart", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                var sendwayIds = shippings.Select(x => x.sendwayId).ToList();
                // فقط بانک‌هایی که برای این روش‌های ارسال فعالن و پرداخت آنلاین دارن (طبق تصمیم فعلی)
                var bankAccounts = uow.BankAccountRepository.GetQueryList().AsNoTracking()
                    .Include("Image")
                    .Include("ProductSendWayBankSelects")
                    .Include("BankAccountOnlineInfo")
                    .Where(x => x.IsActive && x.LanguageId == 1 && x.BankAccountOnlineInfo != null && x.ProductSendWayBankSelects.Any(a => sendwayIds.Contains(a.ProductSendWayId)))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(b => new
                    {
                        b.Id,
                        b.Title,
                        b.BankId,
                        b.MinBasketPrice,
                        b.MaxBasketPrice,
                        Image = b.Image != null ? b.Image.FileName : null
                    })
                    .ToList();

                string offPriceType;
                long offPrice;
                ComputeOffPrice(userid, GetBasketCatIds(BasketItems), basketStep1.ProductBasketItems, codeGift, bonCount ?? 0, out offPriceType, out offPrice);

                DateTime dateTime = DateTime.Now;
                int userCurrentBon = uow.UserRepository.Get(x => x, x => x.Id == userid, null, "UserBons").Single().UserBons.Where(x => x.state == true && x.ExpireDate > dateTime && x.Value > x.UsedValue).Sum(x => x.Value - x.UsedValue);
                int? userMaxBon = basketStep1.ProductBasketItems.Sum(x => x.MaxBon);

                // معادل bool IsEstelam توی BasketStep3 قدیمی — کارت‌به‌کارت اصلاً به کالای استعلامی
                // پیشنهاد نمی‌شه (_Step3.cshtml: @if (IsEstelam == false))
                bool isEstelam = basketStep1.ProductBasketItems.Any(x => x.productStateId == 2);

                // معادل long basketprice توی _Step3.cshtml — جمع سبد منهای تخفیفِ نوع «کد_بن_تخفیف»
                // (تخفیف نوع ارسال‌رایگان چیزی از basketprice کم نمی‌کنه)
                long basketPriceForCard = basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity) - (offPriceType == "کد_بن_تخفیف" ? offPrice : 0);

                // معادل Model.BankAccounts.Where(x => x.HasCardToCard && ...) توی _Step3.cshtml —
                // شرط isEstelam داخل همون Where تک‌کوئری تا نیازی به دو تا نوع مختلف (ternary) نباشه؛
                // EF6 نمی‌تونه IQueryable<anonymous> رو به object کست کنه (Cast<object> قبلی همینجا
                // exception می‌داد و باعث خطای عمومی "خطایی رخ داد" می‌شد)
                var cardBankAccounts = uow.BankAccountRepository.GetQueryList().AsNoTracking()
                    .Include("Imagecard")
                    .Where(x => !isEstelam && x.IsActive && x.LanguageId == 1 && x.HasCardToCard
                        && (x.MinBasketPricecard == 0 || basketPriceForCard >= x.MinBasketPricecard)
                        && (x.MaxBasketPricecard == 0 || basketPriceForCard <= x.MaxBasketPricecard))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(b => new
                    {
                        b.Id,
                        Title = b.Titlecard,
                        Description = b.Descriptioncard,
                        b.CardNumber,
                        b.AccountNumber,
                        b.AccountSHBN,
                        b.AccountName,
                        ExpireHours = b.CardNumberHours,
                        Image = b.AttachementIdcard.HasValue && b.Imagecard != null ? b.Imagecard.FileName : null
                    })
                    .ToList();

                // معادل ViewBag.SameCity توی Payment (GET) — دقیقاً همون رفتار حلقه‌ی قدیمی که هر بار
                // SameCity رو overwrite می‌کنه، یعنی آخرین آیتم shippings تعیین‌کننده‌ست
                var settingForCity = GetSetting();
                bool sameCity = false;
                foreach (var item in shippings)
                {
                    int adid = item.addressId > 0
                        ? (uow.UserAddressRepository.GetByID(item.addressId)?.CityId ?? 0)
                        : (userCityid ?? 0);
                    sameCity = adid == settingForCity.CityId;
                }

                // معادل !Model.basketStep1.ProductBasketItems.Any(a => a.CancelInPlacePayment) — بعضی
                // محصولات اصلاً اجازه‌ی پرداخت درجا (POS/نقدی) نمی‌دن
                bool canPayOnDelivery = sameCity && !basketStep1.ProductBasketItems.Any(x => x.CancelInPlacePayment);

                // معادل foreach روی HasCourierDeliveryPos توی _Step3.cshtml — بازه‌ی قیمتی روی
                // basketPriceForCard چک می‌شه (تقریب همون sumPrice قدیمی، بدون احتساب هزینه‌ی ارسال؛
                // چون این بازه‌ها تقریباً همیشه صفر/بدون‌محدودیت تنظیم می‌شن)
                var posBankAccounts = uow.BankAccountRepository.GetQueryList().AsNoTracking()
                    .Where(x => canPayOnDelivery && x.IsActive && x.LanguageId == 1 && x.HasCourierDeliveryPos
                        && basketPriceForCard >= x.DeliveryPosPriceMin && basketPriceForCard <= x.DeliveryPosPriceMax)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(b => new { b.Id, ExtraPrice = b.DeliveryPosPrice })
                    .ToList();

                // معادل foreach روی HasCourierDeliveryCash — بدون محدودیت بازه‌ی قیمتی
                var cashBankAccounts = uow.BankAccountRepository.GetQueryList().AsNoTracking()
                    .Where(x => canPayOnDelivery && x.IsActive && x.LanguageId == 1 && x.HasCourierDeliveryCash)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(b => new { b.Id })
                    .ToList();

                // معادل foreach روی x.torob توی _Step3.cshtml — بازه‌ی Min/MaxBasketPricetorob
                var torobBankAccounts = uow.BankAccountRepository.GetQueryList().AsNoTracking()
                    .Include("Imagetorob")
                    .Where(x => x.IsActive && x.LanguageId == 1 && x.torob
                        && (x.MinBasketPricetorob == 0 || basketPriceForCard >= x.MinBasketPricetorob)
                        && (x.MaxBasketPricetorob == 0 || basketPriceForCard <= x.MaxBasketPricetorob))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(b => new
                    {
                        b.Id,
                        Title = b.Titletorob,
                        Description = b.Descriptiontorob,
                        Image = b.AttachementIdtorob.HasValue && b.Imagetorob != null ? b.Imagetorob.FileName : null
                    })
                    .ToList();

                // معادل foreach روی x.dgpay — بازه‌ی Min/MaxBasketPricedgpay
                var dgpayBankAccounts = uow.BankAccountRepository.GetQueryList().AsNoTracking()
                    .Include("Imagedgpay")
                    .Where(x => x.IsActive && x.LanguageId == 1 && x.dgpay
                        && (x.MinBasketPricedgpay == 0 || basketPriceForCard >= x.MinBasketPricedgpay)
                        && (x.MaxBasketPricedgpay == 0 || basketPriceForCard <= x.MaxBasketPricedgpay))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(b => new
                    {
                        b.Id,
                        Title = b.Titledigipay,
                        Description = b.Descriptiondgpay,
                        Image = b.AttachementIddgpay.HasValue && b.Imagedgpay != null ? b.Imagedgpay.FileName : null
                    })
                    .ToList();

                // معادل CartController.selectPosPay(id, typeid) — ریزحساب هزینه ارسال (بیمه/بسته‌بندی/
                // پس‌کرایه/هزینه ارسال کارتخوان) + مالیات (کمیسیون درگاه اقساطی)، بر اساس بانک/نوع
                // پرداختِ *انتخاب‌شده*. تا وقتی کاربر چیزی انتخاب نکرده (bankid خالیه)، این بخش‌ها صفر
                // برمی‌گردن؛ فرانت بعد از اولین انتخاب (حتی پیش‌فرض)، دوباره همین اکشن رو با
                // bankid/paymentType واقعی صدا می‌زنه تا این ارقام به‌روز بشن.
                var selectedBankAccount = bankid.HasValue && bankid.Value > 0
                    ? uow.BankAccountRepository.GetByID(bankid.Value)
                    : null;

                long sumShippingCost = 0;
                var basketShippingRows = new List<object>();

                foreach (var item in shippings)
                {
                    int adid = item.addressId > 0
                        ? (uow.UserAddressRepository.GetByID(item.addressId)?.CityId ?? 0)
                        : (userCityid ?? 0);
                    var psendway = uow.ProductSendWayRepository.GetByID(item.sendwayId);
                    if (psendway == null)
                        continue;

                    // item.ProductPackageType (روی BasketShipping) از نوع enum هست، ولی روی
                    // ProductBasketItemV2 به‌صورت int ذخیره شده — برای همین اینجا باید کست بشه
                    var groupItems = basketStep1.ProductBasketItems.Where(x => x.ProductPackageType == (int)item.ProductPackageType).ToList();
                    if (!groupItems.Any())
                        continue;

                    int extraPriceMax = groupItems.Max(x => x.extraprice);
                    int extraprice2 = psendway.HasExtraPrice2 ? psendway.ExtraPriceValue : 0;

                    long insuranceCostRaw = (psendway.HasExtraPrice && item.extraprice)
                        ? Convert.ToInt64(Math.Ceiling((groupItems.Sum(x => (double)x.finalPrice) * 0.005) * 0.001) * 1000)
                        : 0;
                    long insuranceCost = insuranceCostRaw > 0 ? Math.Max(15000, Math.Min(100000, insuranceCostRaw)) : 0;

                    var sendwayBox = uow.SendwayBoxRepository.GetByID(item.sendwayBoxId);
                    int packageCost = psendway.HasExtraPrice
                        ? Convert.ToInt32(Math.Ceiling((extraPriceMax + (sendwayBox != null ? sendwayBox.BoxPrice : 0)) * 0.001) * 1000)
                        : 0;

                    bool freesend;
                    int? sendwayCost = uow.ProductPriceRepository.GetSendwayCost(
                        groupItems.Select(x => x.Id).ToList(),
                        adid,
                        item.sendwayBoxId,
                        psendway.Id,
                        groupItems.Sum(x => x.productWeight * x.UserQuantity),
                        basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity),
                        packageCost,
                        out freesend,
                        groupItems.Select(x => (Domain.ProductPackageType)x.ProductPackageType).ToList());

                    long fullCost = (sendwayCost ?? 0) + insuranceCost + packageCost + extraprice2;
                    long displayCost = psendway.PasKeraye ? 0 : fullCost;

                    long deliveryPosPriceCost = (selectedBankAccount != null && selectedBankAccount.HasCourierDeliveryPos && paymentType == 4)
                        ? selectedBankAccount.DeliveryPosPrice
                        : 0;

                    // معادل «if (Model.OffPriceType == کدتخفیف_ارسال_رایگان) { sumcost = 0; رایگان }»
                    // توی _Step3Aside.cshtml — با کد تخفیف ارسال‌رایگان، کل هزینه‌ی ارسال (شامل بیمه/
                    // بسته‌بندی/کارتخوان) این بسته صفر می‌شه، نه فقط هزینه‌ی خام ارسال
                    bool freeSendByCode = offPriceType == "کدتخفیف_ارسال_رایگان";
                    if (freeSendByCode)
                    {
                        displayCost = 0;
                        insuranceCost = 0;
                        packageCost = 0;
                        extraprice2 = 0;
                        deliveryPosPriceCost = 0;
                    }

                    sumShippingCost += !psendway.PasKeraye ? displayCost + deliveryPosPriceCost : packageCost;

                    // معادل «زمان تحویل» توی _Step3Summary.cshtml — همون‌جا سرور رشته‌ی فارسی
                    // آماده می‌سازه (نه DateTime/TimeSpan خام) که با اصل «فقط دیتای لازم» هم‌خونیه
                    string deliveryDateFormatted = null;
                    string deliveryTimeRange = null;
                    if (!string.IsNullOrEmpty(item.deliveryDate))
                    {
                        DateTime parsedDeliveryDate;
                        if (DateTime.TryParse(item.deliveryDate, out parsedDeliveryDate) && parsedDeliveryDate != DateTime.MinValue)
                        {
                            deliveryDateFormatted = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsiWithoutYear(parsedDeliveryDate);
                            var workTime = uow.ProductSendWayWorkTimeRepository.GetByID(item.sendwayWorktimeId);
                            if (workTime != null)
                                deliveryTimeRange = workTime.StartTime.ToString(@"hh\:mm") + " - " + workTime.EndTime.ToString(@"hh\:mm");
                        }
                    }

                    basketShippingRows.Add(new
                    {
                        packageLabel = groupItems.First().PacakgeType,
                        sendwayTitle = psendway.Title,
                        cost = displayCost,
                        insuranceCost = insuranceCost,
                        packageCost = packageCost,
                        extraPriceCost = extraprice2,
                        extraPriceTitle = psendway.ExtraPriceTitle,
                        pasKeraye = freeSendByCode ? false : psendway.PasKeraye,
                        deliveryPosPriceCost = deliveryPosPriceCost,
                        deliveryDateFormatted = deliveryDateFormatted,
                        deliveryTimeRange = deliveryTimeRange
                    });
                }

                // معادل step3.commision توی selectPosPay — درصد کمیسیون بسته به نوع پرداخت انتخاب‌شده
                // از فیلد متناظرش روی BankAccount خونده می‌شه (ترب/کارت/دیجی‌پی/عادی)
                long commission = 0;
                if (selectedBankAccount != null)
                {
                    int commissionPercent = paymentType == 7 ? selectedBankAccount.Commisiontorob
                        : paymentType == 3 ? selectedBankAccount.Commisioncard
                        : paymentType == 8 ? selectedBankAccount.Commisiondgpay
                        : selectedBankAccount.Commision;
                    if (commissionPercent > 0)
                    {
                        long basketTotalForCommission = basketStep1.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity) + sumShippingCost - (offPriceType == "کد_بن_تخفیف" ? offPrice : 0);
                        long rawCommission = Convert.ToInt64(Math.Ceiling(basketTotalForCommission * (commissionPercent / 100.0)));
                        commission = Convert.ToInt64(Math.Ceiling(rawCommission * 0.001) * 1000);
                    }
                }

                return Json(new
                {
                    basketStep1 = new
                    {
                        ProductBasketItems = basketStep1.ProductBasketItems,
                        validBuyOneTimePerOffer = basketStep1.validBuyOneTimePerOffer,
                        NotvalidBox = basketStep1.NotvalidBox
                    },
                    bankAccounts = bankAccounts,
                    cardBankAccounts = cardBankAccounts,
                    basketShippings = basketShippingRows,
                    sumShippingCost = sumShippingCost,
                    commission = commission,
                    posBankAccounts = posBankAccounts,
                    cashBankAccounts = cashBankAccounts,
                    torobBankAccounts = torobBankAccounts,
                    dgpayBankAccounts = dgpayBankAccounts,
                    offPrice = offPrice,
                    offPriceType = offPriceType,
                    userCurrentBon = userCurrentBon,
                    userMaxBon = userMaxBon,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "paymentinfo", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // محاسبه‌ی مبلغ تخفیف (کد تخفیف کاربر/کد تخفیف عمومی/بن) — دقیقاً همون منطق CartController.Payment
        // و OrderService.AddOrder (که این عدد رو دوباره، مستقل، حین ثبت سفارش محاسبه می‌کنه؛ این نسخه فقط
        // برای نمایش توی خلاصه‌ی سفارشه، نه منبع حقیقت نهایی).
        private void ComputeOffPrice(string userid, List<int> catIds, IEnumerable<ProductBasketItemV2> items, string codeGift, int bon, out string offPriceType, out long offPrice)
        {
            offPriceType = null;
            offPrice = 0;

            if (items.Any(x => x.hasoff))
            {
                offPriceType = "محصول";
                offPrice = items.Sum(x => x.offFinalValue * x.UserQuantity);
                return;
            }

            if (!string.IsNullOrEmpty(codeGift))
            {
                var offer = uow.UserCodeGiftRepository.Get(x => new { x.MaxValue, x.OfferId, x.Value, x.CodeType, x.Offer.CodeTypeValueCode, x.Offer.offerProductCategories }, x => x.UserId == userid && x.Code == codeGift, null, "Offer.offerProductCategories").FirstOrDefault();
                if (offer != null)
                {
                    if (offer.CodeTypeValueCode == 3)
                    {
                        offPriceType = "کدتخفیف_ارسال_رایگان";
                        offPrice = 0;
                    }
                    else
                    {
                        long basketSum = items.Sum(x => x.finalPrice * x.UserQuantity);
                        if (offer.offerProductCategories.Any())
                        {
                            var offercats = offer.offerProductCategories.Select(a => a.CatId).ToList();
                            var offercatsWithSubcats = new List<int>();
                            foreach (var c in offercats)
                                offercatsWithSubcats.AddRange(uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", c)).ToList());
                            basketSum = items.Where(x => offercatsWithSubcats.Contains(x.CatId)).Sum(x => x.finalPrice * x.UserQuantity);
                        }
                        offPriceType = "کد_بن_تخفیف";
                        long ofp = Convert.ToInt64(Math.Ceiling(offer.CodeType == 1 ? offer.Value : Math.Ceiling(((offer.Value / 100.0) * basketSum) * 0.001) * 1000));
                        if (ofp > offer.MaxValue && offer.MaxValue > 0) ofp = offer.MaxValue;
                        offPrice = ofp;
                    }
                    return;
                }

                var generalCodeGift = uow.GeneralCodeGiftRepository.Get(x => new { x.generalCode, x.MaxValue, x.OfferId, x.Value, x.CodeType, x.Offer.CodeTypeValueCode, x.Offer.offerProductCategories }, x => (x.Code == codeGift || x.GeneralCodeGiftLists.Any(b => b.Code == codeGift)), null, "Offer.offerProductCategories").FirstOrDefault();
                if (generalCodeGift != null)
                {
                    if (generalCodeGift.generalCode == GeneralCodeType.تخفیف_ارسال_رایگان)
                    {
                        offPriceType = "کدتخفیف_ارسال_رایگان";
                        offPrice = 0;
                    }
                    else
                    {
                        long basketSum = items.Sum(x => x.finalPrice * x.UserQuantity);
                        if (generalCodeGift.offerProductCategories.Any())
                        {
                            var offercats = generalCodeGift.offerProductCategories.Select(a => a.CatId).ToList();
                            var offercatsWithSubcats = new List<int>();
                            foreach (var c in offercats)
                                offercatsWithSubcats.AddRange(uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", c)).ToList());
                            basketSum = items.Where(x => offercatsWithSubcats.Contains(x.CatId)).Sum(x => x.finalPrice * x.UserQuantity);
                        }
                        offPriceType = "کد_بن_تخفیف";
                        long ofp = Convert.ToInt64(Math.Ceiling(generalCodeGift.CodeType == 1 ? generalCodeGift.Value : Math.Ceiling(((generalCodeGift.Value / 100.0) * basketSum) * 0.001) * 1000));
                        if (ofp > generalCodeGift.MaxValue && generalCodeGift.MaxValue > 0) ofp = generalCodeGift.MaxValue;
                        offPrice = ofp;
                    }
                }
                return;
            }

            if (bon > 0)
            {
                offPriceType = "کد_بن_تخفیف";
                offPrice = uow.SettingRepository.Get(x => x.BonPrice, x => x.LanguageId == 1).First() * bon;
            }
        }

        // معادل GetParentCats برای همه‌ی محصولات سبد — برای اعتبارسنجی محدودیت دسته‌بندی کد تخفیف
        // (GiftCatValid)
        private List<int> GetBasketCatIds(List<Basket> BasketItems)
        {
            var productIds = BasketItems.Select(x => x.ProductId).ToList();
            var catids = uow.ProductRepository.Get(x => x.ProductCategories, x => productIds.Contains(x.Id), null, "ProductCategories");
            var catIds = new List<int>();
            foreach (var item in catids)
            {
                var parrentCatIds = uow.ContentRepository.SqlQuery("exec GetParentCats @CatId", new SqlParameter("@CatId", item.FirstOrDefault().Id)).ToList();
                catIds.Add(parrentCatIds.Last());
            }
            return catIds;
        }

        // معادل JSON اکشن قدیمی CartController.CheckGiftCode — فقط اعتبارسنجی می‌کنه (کد رو جایی ذخیره
        // نمی‌کنه، چون سبد سمت کلاینت/localStorage نگه داشته می‌شه، نه کوکی سمت سرور). کدهای statusCode
        // دقیقاً همون کدهای قدیمی‌ان (404 نامعتبر، 405 شروع‌نشده، 406 منقضی، 407 اتمام سقف استفاده،
        // 408 مخصوص اولین سفارش، 409 دسته‌بندی نامعتبر، 200 معتبر) تا فرانت بتونه پیام دقیق نشون بده.
        [HttpPost]
        [JWTAuthorize]
        public JsonResult CheckGiftCode(List<Basket> BasketItems, string code)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            if (BasketItems == null || !BasketItems.Any() || string.IsNullOrEmpty(code))
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                code = code.Trim();
                DateTime date = DateTime.Now;
                var catIds = GetBasketCatIds(BasketItems);

                var usercode = uow.UserCodeGiftRepository.Get(x => new { x.Offer.offerProductCategories, x.Offer, x.UserCodeGiftLogs, x.Code, x.CountUse, x.IsActive, x.MaxValue, x.Offer.ExpireDate, x.Offer.StartDate, eeExpireDate = x.ExpireDate }, x => x.UserId == userid && x.Code == code && x.Offer.IsDeleted == false, null, "Offer,UserCodeGiftLogs,Offer.offerProductCategories").FirstOrDefault();
                if (usercode == null)
                {
                    var generalcode = uow.GeneralCodeGiftRepository.Get(x => new { x.Offer.offerProductCategories, x.generalCode, x.Offer, x.GeneralCodeGiftLogs, x.Code, x.CountUse, x.IsActive, x.MaxValue, x.Offer.ExpireDate, x.Offer.StartDate }, x => (x.Code == code || x.GeneralCodeGiftLists.Any(b => b.Code == code)) && x.Offer.IsDeleted == false, null, "Offer,GeneralCodeGiftLists,GeneralCodeGiftLogs,GeneralCodeGiftLogs.Order,Offer.offerProductCategories").FirstOrDefault();
                    if (generalcode == null) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                    if (!generalcode.IsActive) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                    if (!generalcode.Offer.IsActive) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                    if (!generalcode.Offer.state) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                    if (generalcode.StartDate != null && generalcode.StartDate > date) return Json(new { statusCode = 405 }, JsonRequestBehavior.AllowGet);
                    if (generalcode.ExpireDate != null && generalcode.ExpireDate < date) return Json(new { statusCode = 406 }, JsonRequestBehavior.AllowGet);
                    if (generalcode.GeneralCodeGiftLogs.Where(a => a.state == true && ((a.Order.UserId == userid && a.GeneralCodeGift.CountUseSelect == true) || a.GeneralCodeGift.CountUseSelect == false)).Count() >= generalcode.CountUse && generalcode.CountUse > 0)
                        return Json(new { statusCode = 407 }, JsonRequestBehavior.AllowGet);
                    if (generalcode.generalCode == GeneralCodeType.تخفیف_اولین_سفارش && uow.OrderRepository.Any(x => x.Id, x => x.UserId == userid && x.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت)))
                        return Json(new { statusCode = 408 }, JsonRequestBehavior.AllowGet);
                    if (!uow.OrderRepository.GiftCatValid(generalcode.offerProductCategories.Select(x => x.CatId).ToList(), catIds))
                        return Json(new { statusCode = 409 }, JsonRequestBehavior.AllowGet);

                    return Json(new { statusCode = 200 }, JsonRequestBehavior.AllowGet);
                }

                if (usercode.eeExpireDate != null && usercode.eeExpireDate < date) return Json(new { statusCode = 406 }, JsonRequestBehavior.AllowGet);
                if (!usercode.IsActive) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                if (!usercode.Offer.IsActive) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                if (!usercode.Offer.state) return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                if (usercode.StartDate != null && usercode.StartDate > date) return Json(new { statusCode = 405 }, JsonRequestBehavior.AllowGet);
                if (usercode.ExpireDate != null && usercode.ExpireDate < date) return Json(new { statusCode = 406 }, JsonRequestBehavior.AllowGet);
                if (usercode.UserCodeGiftLogs.Where(a => a.state == true).Count() >= usercode.CountUse && usercode.CountUse > 0)
                    return Json(new { statusCode = 407 }, JsonRequestBehavior.AllowGet);
                if (!uow.OrderRepository.GiftCatValid(usercode.offerProductCategories.Select(x => x.CatId).ToList(), catIds))
                    return Json(new { statusCode = 409 }, JsonRequestBehavior.AllowGet);

                return Json(new { statusCode = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, statusCode = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        // معادل JSON اکشن قدیمی CartController.CheckBon — فقط اعتبارسنجی می‌کنه.
        [HttpPost]
        [JWTAuthorize]
        public JsonResult CheckBon(List<Basket> BasketItems, int count)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            if (BasketItems == null || !BasketItems.Any())
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                DateTime date = DateTime.Now;
                int userCurrentBon = uow.UserRepository.Get(x => x, x => x.Id == userid, null, "UserBons").Single().UserBons.Where(x => x.state == true && x.ExpireDate > date && x.Value > x.UsedValue).Sum(x => x.Value - x.UsedValue);
                var basketStep1 = GetBasket(BasketItems, 2, userid, null);
                if (basketStep1 == null)
                    return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);
                int? userMaxBon = basketStep1.ProductBasketItems.Sum(x => x.MaxBon);

                if (userCurrentBon < 1)
                    return Json(new { statusCode = 405 }, JsonRequestBehavior.AllowGet);
                if (count > userCurrentBon)
                    return Json(new { statusCode = 406 }, JsonRequestBehavior.AllowGet);
                if (count > userMaxBon && userMaxBon > 0)
                    return Json(new { statusCode = 406 }, JsonRequestBehavior.AllowGet);

                return Json(new { statusCode = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, statusCode = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        // پروجکشن سبک List<Domain.ViewModels.ProductBox> (نه V2!) از روی داده‌ی V2 که همین‌جا داریم —
        // چون uow.OrderRepository.AddOrder فقط همین نوع V1 قدیمی رو قبول می‌کنه. فقط همون فیلدهایی که
        // AddOrder واقعاً می‌خونه (SendwayBox.ProductPackageType/Id، sendWayBoxPrices[].productSendWay.Id،
        // BoxMass، IsDefault، PasKeraye، cost، ProductPriceIdList) پر می‌شن؛ بقیه دست‌نخورده/پیش‌فرض می‌مونن
        // چون AddOrder اصلاً بهشون دسترسی نداره.
        private List<Domain.ViewModels.ProductBox> ToLegacyProductBoxes(BasketStep1V2 basketStep1)
        {
            return basketStep1.ProductBoxes.Select(box => new Domain.ViewModels.ProductBox
            {
                SendwayBox = new Domain.SendwayBox
                {
                    Id = box.SendwayBox.Id,
                    ProductPackageType = (Domain.ProductPackageType)box.SendwayBox.ProductPackageType
                },
                ProductPriceIdList = box.ProductPriceIdList,
                sendWayBoxPrices = box.sendWayBoxPrices.Select(price => new Domain.ViewModels.SendWayBoxPrice
                {
                    Id = price.Id,
                    productSendWay = new Domain.ProductSendWay { Id = price.productSendWay.Id },
                    cost = price.cost,
                    InsuranceCost = price.InsuranceCost,
                    PackageCost = price.PackageCost,
                    PasKeraye = price.PasKeraye,
                    IsDefault = price.IsDefault,
                    BoxMass = price.BoxMass,
                    FreeSend = price.FreeSend,
                    show = price.show,
                    DisplaySort = price.DisplaySort,
                    Extraprice2Cost = price.Extraprice2Cost
                }).ToList()
            }).ToList();
        }

        // معادل JSON اکشن قدیمی CartController.AddOrder — واقعاً رکورد Order/OrderDelivery/OrderRow/Wallet
        // رو با استفاده از همون متد تست‌شده‌ی uow.OrderRepository.AddOrder می‌سازه (منطق داخلی محاسبه‌ی
        // قیمت/کد تخفیف/بن عیناً همون‌جاست، این‌جا فقط صدا زده می‌شه).
        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> AddOrder(List<Basket> BasketItems, BasketShipping[] shippings, int bankid, string userDescription, bool sendFactor, string codeGift, int? bonCount, int? paymentType)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            if (!await uow.UserAddressRepository.UserHasAnyPersonalInfo(userid))
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            if (bankid < 1 || BasketItems == null || !BasketItems.Any() || shippings == null || !shippings.Any())
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            var setting = GetSetting();

            try
            {
                // OrderService.AddOrder فقط از BasketItems.First().shippings می‌خونه (دقیقاً همون
                // قراردادِ نسخه‌ی قدیمی که کل سبد رو با هم توی یه کوکی نگه می‌داشت)
                BasketItems.First().shippings = shippings.ToList();

                int? addressCityId = null;
                var firstAddressId = shippings.Where(x => x.addressId > 0).Select(x => (int?)x.addressId).FirstOrDefault();
                if (firstAddressId.HasValue)
                    addressCityId = uow.UserAddressRepository.GetByID(firstAddressId.Value)?.CityId;

                var basketStep1 = GetBasket(BasketItems, 2, userid, addressCityId);
                if (basketStep1 == null)
                    return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

                // معادل چک بازه‌ی زمانی + موجودی/قیمت/تعداد توی CartController.AddOrder
                bool validDeliveryDate = true;
                var userCityid = uow.UserRepository.GetByID(userid)?.CityId;
                foreach (var item in shippings)
                {
                    var sendway = uow.ProductSendWayRepository.GetByID(item.sendwayId);
                    if (sendway != null && sendway.DeliverSelectable)
                    {
                        var wt = uow.ProductSendWayWorkTimeRepository.GetByID(item.sendwayWorktimeId);
                        if (wt == null || wt.ProductSendWayId != item.sendwayId)
                        {
                            validDeliveryDate = false;
                        }
                        else
                        {
                            DateTime dtSelect = Convert.ToDateTime(item.deliveryDate).Date.AddHours(wt.StartTime.TotalHours);
                            if (DateTime.Now.AddMinutes(30) >= dtSelect)
                                validDeliveryDate = false;
                            else
                            {
                                int? limitation = uow.ProductSendWayDetailRepository.Get(x => x.Limitation, x => x.ProductSendWayBoxId == item.sendwayBoxId && x.CityId == userCityid).FirstOrDefault();
                                if (limitation.HasValue && limitation > 0)
                                {
                                    DateTime dt = Convert.ToDateTime(item.deliveryDate);
                                    if (uow.OrderDeliveryRepository.Count(x => x.Order.OrderStates.Any(o => o.state == OrderStatus.تایید_پرداخت) && x.ProductSendWayWorkTimeId == item.sendwayWorktimeId && x.RequestDate == dt) >= limitation)
                                        validDeliveryDate = false;
                                }
                            }
                        }
                    }
                }
                if (!validDeliveryDate)
                    return Json(new { statusCode = 4041 }, JsonRequestBehavior.AllowGet);

                bool validExist = true, validprice = true, validQuantity = true;
                foreach (var item in BasketItems)
                {
                    var prp = uow.ProductPriceRepository.Get(x => new { x.MaxBasketCount, x.Quantity, x.Price }, x => x.Id == item.ProductPriceId).FirstOrDefault();
                    if (prp == null || uow.ProductPriceRepository.Any(x => x.Id, x => x.Id == item.ProductPriceId && x.ProductStateId > 2))
                    {
                        validExist = false;
                        continue;
                    }
                    if (prp.Price != item.Price)
                        validprice = false;
                    if (prp.MaxBasketCount < item.Quantity || prp.Quantity < item.Quantity)
                        validQuantity = false;
                }
                if (!validExist)
                    return Json(new { statusCode = 4042 }, JsonRequestBehavior.AllowGet);
                if (!validprice || !validQuantity)
                    return Json(new { statusCode = 4043 }, JsonRequestBehavior.AllowGet);

                var productBoxes = ToLegacyProductBoxes(basketStep1);

                // نوع پرداخت: 1=آنلاین بانکی (پیش‌فرض، سازگار با نسخه‌ی قبلی)، 3=کارت‌به‌کارت.
                // بقیه‌ی مقادیر (2/4/5/6/7/8) توی OrderService.AddOrder پشتیبانی می‌شن ولی هنوز
                // مسیر فرانت‌شون پیاده نشده.
                int payType = (paymentType.HasValue && paymentType.Value > 0) ? paymentType.Value : 1;

                bool isEstelam, validFreeSend;
                string customerOrderId;
                long orderId = uow.OrderRepository.AddOrder(userid, null, BasketItems, productBoxes, userDescription, sendFactor, bankid, payType, codeGift ?? "", bonCount ?? 0, setting.LanguageId ?? 1, "", out isEstelam, out validFreeSend, out customerOrderId);

                if (!validFreeSend)
                {
                    var orderdeliveries = uow.OrderDeliveryRepository.Get(x => x, x => x.Order.BankOrderId == orderId);
                    uow.OrderDeliveryRepository.Delete(orderdeliveries.ToList());
                    uow.Save();
                    var orderToRemove = uow.OrderRepository.Get(x => x, x => x.BankOrderId == orderId);
                    if (orderToRemove.Any())
                    {
                        uow.OrderRepository.Delete(orderToRemove.First());
                        uow.Save();
                    }
                    return Json(new { statusCode = 4043 }, JsonRequestBehavior.AllowGet);
                }

                if (orderId <= 0)
                    return Json(new { statusCode = 500 }, JsonRequestBehavior.AllowGet);

                var bankBankId = uow.BankAccountRepository.Get(x => x.BankId, x => x.Id == bankid).SingleOrDefault();

                return Json(new
                {
                    bankid = bankBankId,
                    isEstelam = isEstelam,
                    orderid = orderId,
                    customerOrderId = customerOrderId,
                    paymentType = payType,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addorder", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // معادل JSON اکشن قدیمی CartController.OnlinePay — هم مسیر تازه‌ساخته‌شده (از AddOrder)
        // و هم مسیر «لینک مستقیم دستی» (کاربر با BankOrderId سفارشِ از قبل ساخته‌شده مستقیماً وارد
        // میشه، دقیقاً مثل ~/Cart/OnlinePay/{id}?bankId=X قدیمی که برای مشتریان ارسال میشد) از همین
        // اکشن رد میشن. بر اساس PaymentType واقعیِ سفارش تصمیم میگیره: آنلاین(1)/ترب(7)/دیجی‌پی(8)
        // مستقیم به درگاه وصل میشن؛ کارت‌به‌کارت(3)/POS(4)/نقدی(5) اطلاعات لازم برای ادامه‌ی همون
        // مرحله رو برمی‌گردونن (نه فقط خطا)؛ فیش بانکی(6) فعلاً پشتیبانی نمیشه.
        // کال‌بک درگاه‌ها (BankPaymentCallBack/SamanPaymentCallBack/torobcallback/dgpaycallback) طبق
        // تصمیم قبلی روی بک‌اند قدیمی باقی می‌مونه چون خودِ درگاه مستقیم به اون URL هیت می‌زنه.
        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> OnlinePay(long id, int bankId)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { url = "/account/login", statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                var setting = uow.SettingRepository.Get(x => new { x.SiteClosed }, x => x.LanguageId == 1).FirstOrDefault();
                if (setting != null && setting.SiteClosed)
                    return Json(new { url = "/cart/temporarilyClosed", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                DateTime dtTime = DateTime.Now;
                string include = "OrderStates,OrderWallets,OrderWallets.Wallet,OrderWallets.Wallet.BankAccount.BankAccountOnlineInfo,User,OrderRows.Product.ProductCategories,OrderRows.Product.Brand";

                var order = uow.OrderRepository.Get(x => x, x => !x.OrderStates.Any(s => s.state > OrderStatus.تایید_سفارش) && x.ExpireDate > dtTime && x.BankOrderId == id && x.UserId == userid, null, include).FirstOrDefault();

                if (order == null)
                {
                    // معادل fallback روی CreateOrderKey توی نسخه‌ی قدیمی — برای مواردی که BankOrderId
                    // مستقیم گم شده (مثلاً بعد از ادغام سفارش)
                    var firstLastOrderId = uow.CreateOrderKeyRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (firstLastOrderId != null && firstLastOrderId.OrderId.HasValue)
                        order = uow.OrderRepository.Get(x => x, x => !x.OrderStates.Any(s => s.state > OrderStatus.تایید_سفارش) && x.ExpireDate > dtTime && x.Id == firstLastOrderId.OrderId.Value && x.UserId == userid, null, include).FirstOrDefault();
                }

                if (order == null)
                {
                    // پیام دقیق‌تر برای لینک دستی: سفارش هست ولی دیگه قابل‌پرداخت نیست
                    var rawOrder = uow.OrderRepository.Get(x => x, x => x.BankOrderId == id && x.UserId == userid, null, "OrderStates").FirstOrDefault();
                    if (rawOrder != null)
                    {
                        if (rawOrder.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت))
                            return Json(new { Message = "این سفارش قبلاً پرداخت شده است.", statusCode = 4091 }, JsonRequestBehavior.AllowGet);
                        if (rawOrder.OrderStates.Any(s => s.state == OrderStatus.لغو_شده || s.state == OrderStatus.درخواست_لغو))
                            return Json(new { Message = "این سفارش لغو شده است.", statusCode = 4092 }, JsonRequestBehavior.AllowGet);
                        if (rawOrder.ExpireDate <= dtTime)
                            return Json(new { Message = "مهلت پرداخت این سفارش به پایان رسیده است.", statusCode = 4093 }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Message = "سفارش یافت نشد.", url = "/", statusCode = 404 }, JsonRequestBehavior.AllowGet);
                }

                if (!order.OrderStates.Any(x => x.state == OrderStatus.تایید_سفارش))
                    return Json(new { url = "/cart/waitingforconfirmation", statusCode = 404 }, JsonRequestBehavior.AllowGet);

                var wallet = order.OrderWallets.First().Wallet;

                if (wallet.PaymentType == 7)
                    return await TorobPayStart(order, wallet);

                if (wallet.PaymentType == 8)
                    return await DgpayPayStart(order, wallet);

                if (wallet.PaymentType == 3)
                {
                    // کارت‌به‌کارت — سفارش از قبل ساخته شده؛ فقط اطلاعات کارت رو برمی‌گردونیم تا
                    // فرانت همون UI آپلود رسید رو نشون بده (بدون نیاز به AddOrder دوباره)
                    var cardAccount = wallet.BankAccount;
                    return Json(new
                    {
                        paymentType = 3,
                        orderid = order.BankOrderId,
                        cardBank = cardAccount == null ? null : new
                        {
                            cardAccount.Id,
                            Title = cardAccount.Titlecard,
                            Description = cardAccount.Descriptioncard,
                            cardAccount.CardNumber,
                            cardAccount.AccountNumber,
                            cardAccount.AccountSHBN,
                            cardAccount.AccountName,
                            ExpireHours = cardAccount.CardNumberHours
                        },
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

                if (wallet.PaymentType == 4 || wallet.PaymentType == 5)
                    return Json(new { paymentType = wallet.PaymentType, orderid = order.BankOrderId, statusCode = 200 }, JsonRequestBehavior.AllowGet);

                if (wallet.PaymentType == 6)
                    return Json(new { Message = "این سفارش با روش پرداخت فیش بانکی ثبت شده که فعلاً از طریق سایت جدید پشتیبانی نمی‌شود؛ لطفاً با پشتیبانی تماس بگیرید.", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                if (wallet.PaymentType != 1)
                    return Json(new { Message = "این سفارش با روش پرداخت آنلاین بانکی ثبت نشده.", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                // اگه bankId مشخص نشده بود (مثلاً از لینک دستی که فقط id رو داره)، از خودِ حساب بانکیِ
                // متصل به سفارش استخراجش می‌کنیم
                int effectiveBankId = bankId > 0 ? bankId : (wallet.BankAccount?.BankId ?? 0);

                if (effectiveBankId == 1)
                {
                    // ملت — دقیقاً همون متد تست‌شده‌ی CartController که خودش با وب‌سرویس بانک صحبت می‌کنه
                    var step1 = uow.BankAccountRepository.MellatPayMentStart(wallet.Price, wallet.BankAccount.BankAccountOnlineInfo.CallbackUrl, order.BankOrderId);
                    string[] resultArray = step1.Split(',');
                    if (resultArray[0] == "0")
                    {
                        return Json(new
                        {
                            paymentType = 1,
                            gatewayUrl = "https://bpm.shaparak.ir/pgwchannel/startpay.mellat",
                            fields = new { RefId = resultArray[1], MobileNo = order.User.UserName },
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Message = "عدم اتصال به بانک " + resultArray[0], statusCode = 500 }, JsonRequestBehavior.AllowGet);
                }
                else if (effectiveBankId == 8)
                {
                    // سامان
                    var token = uow.BankAccountRepository.SamanPayMentStart(wallet.Price, wallet.BankAccount.BankAccountOnlineInfo.CallbackUrl, order.BankOrderId);
                    return Json(new
                    {
                        paymentType = 1,
                        gatewayUrl = "https://sep.shaparak.ir/payment.aspx",
                        fields = new { token = token },
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Message = "این بانک فعلاً پشتیبانی نمی‌شود.", statusCode = 500 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "onlinepay", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // پورت مستقیم از بخش «//torob» توی CartController.OnlinePay — همون سه مرحله‌ی oauth/eligible/token؛
        // فقط به‌جای return Redirect(url)، gatewayUrl رو برمی‌گردونه تا فرانت خودش window.location بزنه.
        private async Task<JsonResult> TorobPayStart(Domain.Order order, Wallet wallet)
        {
            var model = wallet.BankAccount;
            if (model == null || !model.torob)
                return Json(new { Message = "عدم فعالسازی درگاه ترب", statusCode = 500 }, JsonRequestBehavior.AllowGet);

            try
            {
                using (var client = new System.Net.WebClient())
                {
                    client.Headers.Clear();
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(model.torob_client_id + ":" + model.torob_client_secret);
                    client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(plainTextBytes));
                    var vm = new { username = model.torob_username, password = model.torob_password };
                    client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                    string json = client.UploadString("https://cpg.torobpay.com/api/online/v1/oauth/token", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vm));
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<torob_token>(json);
                    if (data.access_token == null)
                        return Json(new { Message = "عدم اتصال به درگاه ترب", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                    using (var client2 = new System.Net.WebClient())
                    {
                        client2.Headers.Clear();
                        client2.Headers.Add("Authorization", "Bearer " + data.access_token);
                        client2.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                        string json2 = client2.DownloadString("https://cpg.torobpay.com/api/online/offer/v1/eligible?amount=" + wallet.Price * 10);
                        var data2 = Newtonsoft.Json.JsonConvert.DeserializeObject<torobEligible>(json2);
                        if (!data2.successful)
                            return Json(new { Message = "عدم اتصال به درگاه ترب - حداقل خرید 20 هزار تومان - حداکثر 100 میلیون تومان", statusCode = 500 }, JsonRequestBehavior.AllowGet);
                    }

                    client.Headers.Clear();
                    client.Headers.Add("Authorization", "Bearer " + data.access_token);
                    client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");

                    var vm2 = new { amount = wallet.Price * 10, mobile = order.User.PhoneNumber, paymentMethodTypeDto = "CREDIT_ONLINE", returnURL = model.torob_callback, transactionId = order.BankOrderId, cartList = new List<cartList>() };
                    vm2.cartList.Add(new cartList
                    {
                        cartId = "TF-" + order.BankOrderId.ToString(),
                        taxAmount = 0,
                        totalAmount = Convert.ToInt32(wallet.Price * 10),
                        shippingAmount = 0,
                        isTaxIncluded = false,
                        isShipmentIncluded = true,
                        cartItems = new List<cartItem>()
                    });
                    foreach (var item in order.OrderRows)
                    {
                        vm2.cartList.First().cartItems.Add(new cartItem
                        {
                            id = item.ProductId.ToString(),
                            name = item.Product.Title.Replace("×", "-"),
                            amount = Convert.ToInt32(item.Price) * 10,
                            count = item.Quantity,
                            category = item.Product.ProductCategories.First().Name.Replace("×", "-")
                        });
                    }

                    string json3 = client.UploadString("https://cpg.torobpay.com/api/online/payment/v1/token", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vm2));
                    var data3 = Newtonsoft.Json.JsonConvert.DeserializeObject<torobpayment>(json3);
                    if (!data3.successful)
                    {
                        order.TorobStatus = "FAILED";
                        uow.OrderRepository.Update(order);
                        uow.Save();
                        return Json(new { Message = "خطا در ارسال اطلاعات به ترب", statusCode = 500 }, JsonRequestBehavior.AllowGet);
                    }

                    order.TorobPaymentToken = data3.response.paymentToken;
                    order.TorobStatus = "OK";
                    uow.OrderRepository.Update(order);
                    uow.Save();

                    return Json(new { paymentType = 7, gatewayUrl = data3.response.paymentPageUrl, statusCode = 200 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "torobpay", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { Message = "عدم اتصال به درگاه ترب: " + ex.Message, statusCode = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        // پورت مستقیم از بخش «//dgpay» توی CartController.OnlinePay — همون دو مرحله‌ی oauth/ticket
        private async Task<JsonResult> DgpayPayStart(Domain.Order order, Wallet wallet)
        {
            var model = wallet.BankAccount;
            if (model == null || !model.dgpay)
                return Json(new { Message = "عدم فعالسازی درگاه دیجی‌پی", statusCode = 500 }, JsonRequestBehavior.AllowGet);

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.mydigipay.com/digipay/api/oauth/token");
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(model.dgpay_client_id + ":" + model.dgpay_client_secret);
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(plainTextBytes));

                    var content = new System.Net.Http.MultipartFormDataContent();
                    content.Add(new System.Net.Http.StringContent(model.dgpay_username), "username");
                    content.Add(new System.Net.Http.StringContent(model.dgpay_password), "password");
                    content.Add(new System.Net.Http.StringContent("password"), "grant_type");
                    request.Content = content;

                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dgpay_token>(json);
                    if (data.access_token == null)
                        return Json(new { Message = "عدم اتصال به درگاه دیجی‌پی", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                    var request2 = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.mydigipay.com/digipay/api/tickets/business?type=11");
                    request2.Headers.Add("Agent", "WEB");
                    request2.Headers.Add("Digipay-Version", "2022-02-02");
                    request2.Headers.Add("Authorization", "Bearer " + data.access_token);

                    var vm2 = new { amount = wallet.Price * 10, cellNumber = order.User.PhoneNumber, callbackUrl = model.dgpay_callback, providerId = order.BankOrderId, basketDetailsDto = new basketDetailsDto() };
                    vm2.basketDetailsDto.basketId = order.BankOrderId.ToString();
                    vm2.basketDetailsDto.items = new List<paycartList>();
                    foreach (var item in order.OrderRows)
                    {
                        vm2.basketDetailsDto.items.Add(new paycartList
                        {
                            sellerId = "1",
                            supplierId = "1",
                            productCode = "TF-" + item.ProductPriceId.ToString(),
                            brand = item.Product.Brand.Name,
                            count = item.Quantity,
                            categoryId = item.Product.ProductCategories.First().Name,
                            productType = 1
                        });
                    }

                    request2.Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(vm2));
                    request2.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var response2 = await client.SendAsync(request2);
                    response2.EnsureSuccessStatusCode();
                    string json2 = await response2.Content.ReadAsStringAsync();
                    var data2 = Newtonsoft.Json.JsonConvert.DeserializeObject<dgpayticket>(json2);

                    if (data2.result.status != 0)
                        return Json(new { Message = "عدم اتصال به درگاه دیجی‌پی - حداقل خرید 20 هزار تومان - حداکثر 100 میلیون تومان", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                    order.DgpayPaymentToken = data2.ticket;
                    order.DgpayStatus = data2.result.status.ToString();
                    uow.OrderRepository.Update(order);
                    uow.Save();

                    return Json(new { paymentType = 8, gatewayUrl = data2.redirectUrl, statusCode = 200 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "dgpaypay", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { Message = "عدم اتصال به درگاه دیجی‌پی: " + ex.Message, statusCode = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        #region کال‌بک‌های جدید درگاه‌ها (جدا از کال‌بک‌های قدیمی روی CartController — این‌ها هیچ اثری
        // روی مسیر قدیمی ندارن؛ برای فعال‌سازی، CallbackUrl/torob_callback/dgpay_callback همون
        // BankAccount توی پنل ادمین باید به این URLهای جدید (apiv1/MellatCallback و ...) اشاره کنه)

        // بلوک مشترک «بعد از تایید نهایی موفق پرداخت» — دقیقاً همون منطق تکراری‌ای که توی نسخه‌ی
        // قدیمی عیناً توی هر ۴ تا Cart.*CallBack کپی شده بود: ثبت WalletAttribute (کد رهگیری/مرجع/
        // تاریخ)، ساخت یا آپدیت کیف پول دوم، افزودن وضعیت تایید_پرداخت، فعال‌سازی لاگ‌های کد تخفیف/
        // بن، و ارسال پیامک به ادمین/کاربر. تنها تفاوت عمدی با نسخه‌ی قدیمی: لینک‌های توی پیامک به
        // فرانت جدید (NextJsFrontendUrl) اشاره می‌کنن نه بک‌اند، و برای «شهر» همه‌جا از
        // order.OrderDeliveries.First().City استفاده شده (نسخه‌ی قدیمی بین بانک‌ها دراین‌مورد ناهماهنگ
        // بود؛ یکی از حالت‌ها این‌جا معیار قرار گرفت).
        private async Task FinalizeOrderPayment(Domain.Order order, Wallet wallet, BankAccount bankAccount, string trackingValue, string authorityValue, string bankTitle)
        {
            ForWhatType forWhat = ForWhatType.پرداخت_سفارش;
            var userId = order.UserId;

            wallet.State = true;
            var paymentRefId = uow.WalletAttributeRepository.GetQueryList().AsNoTracking().Where(x => x.DataType == 17).FirstOrDefault();
            var paymentAuthorityStatus = uow.WalletAttributeRepository.GetQueryList().AsNoTracking().Where(x => x.DataType == 16).FirstOrDefault();
            var dateAttr = uow.WalletAttributeRepository.GetQueryList().AsNoTracking().Where(x => x.DataType == 18).FirstOrDefault();

            foreach (var item in order.OrderDeliveries)
            {
                if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == paymentRefId.Id))
                    wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == paymentRefId.Id).First().Value = trackingValue;
                else
                    wallet.WalletAttributeWallets.Add(new WalletAttributeWallet { OrderDeliveryId = item.Id, WalletAttributeId = paymentRefId.Id, Value = trackingValue });

                if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == paymentAuthorityStatus.Id))
                    wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == paymentAuthorityStatus.Id).First().Value = authorityValue;
                else
                    wallet.WalletAttributeWallets.Add(new WalletAttributeWallet { OrderDeliveryId = item.Id, WalletAttributeId = paymentAuthorityStatus.Id, Value = authorityValue });

                if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == dateAttr.Id))
                    wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == dateAttr.Id).First().Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                else
                    wallet.WalletAttributeWallets.Add(new WalletAttributeWallet { OrderDeliveryId = item.Id, WalletAttributeId = dateAttr.Id, Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            uow.WalletRepository.Update(wallet);
            uow.Save();

            if (order.OrderWallets.Count() < 2)
            {
                Wallet oWallet = new Wallet
                {
                    ForWhat = uow.ForWhatRepository.GetQueryList().Where(x => x.ForWhatType == forWhat).First(),
                    UserId = userId,
                    InsertDate = DateTime.Now,
                    State = true,
                    Price = wallet.Price,
                    DepositOrWithdrawal = false,
                    PaymentType = order.OrderWallets.First().Wallet.PaymentType,
                    BankAccountId = bankAccount.Id,
                    LanguageId = 1
                };
                uow.WalletRepository.Insert(oWallet);
                uow.Save();
                order.OrderWallets.Add(new OrderWallet { Wallet = oWallet });
            }
            else
            {
                order.OrderWallets.Skip(0).First().Wallet.State = true;
                order.OrderWallets.Skip(0).First().Wallet.InsertDate = DateTime.Now;
                order.OrderWallets.Skip(0).First().Wallet.Price = wallet.Price;
                order.OrderWallets.Skip(0).First().Wallet.PaymentType = order.OrderWallets.First().Wallet.PaymentType;
                order.OrderWallets.Skip(0).First().Wallet.BankAccountId = bankAccount.Id;
            }
            uow.OrderRepository.Update(order);
            uow.Save();

            foreach (var item in order.OrderDeliveries)
            {
                order.OrderStates.Add(new OrderState
                {
                    OrderDeliveryId = item.Id,
                    LogDate = DateTime.Now,
                    state = OrderStatus.تایید_پرداخت
                });
            }

            var userCodeGiftLog = uow.UserCodeGiftLogRepository.Get(x => x, x => x.Order.BankOrderId == order.BankOrderId, null, "Order");
            foreach (var item in userCodeGiftLog) { item.state = true; uow.UserCodeGiftLogRepository.Update(item); }
            uow.Save();

            var generalCodeGiftLog = uow.GeneralCodeGiftLogRepository.Get(x => x, x => x.Order.BankOrderId == order.BankOrderId, null, "Order");
            foreach (var item in generalCodeGiftLog) { item.state = true; uow.GeneralCodeGiftLogRepository.Update(item); }
            uow.Save();

            var userBonLog = uow.UserBonLogRepository.Get(x => x, x => x.Order.BankOrderId == order.BankOrderId, null, "Order");
            foreach (var item in userBonLog) { item.state = true; uow.UserBonLogRepository.Update(item); }
            uow.Save();

            var userBon = uow.UserBonRepository.Get(x => x, x => x.Order.BankOrderId == order.BankOrderId && x.UserId == userId, null, "Order").FirstOrDefault();
            if (userBon != null) { userBon.state = true; uow.UserBonRepository.Update(userBon); uow.Save(); }

            order.IsActive = true;
            uow.OrderRepository.Update(order);
            uow.Save();

            string frontendUrl = System.Configuration.ConfigurationManager.AppSettings["NextJsFrontendUrl"] ?? "";
            var setting = GetSetting();
            SmsService sms = new SmsService();
            var user = uow.UserRepository.Get(x => x, x => x.Id == userId, null, "CityEntity.Province,UserAddresses").Single();
            IdentityMessage iPhonemessage = new IdentityMessage();

            try
            {
                bool isestalam = wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == 17);
                string cityName = user.CityId.HasValue ? user.CityEntity.Name : order.OrderDeliveries.First().City;
                iPhonemessage.Destination = setting.Mobile;
                iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_سفارش_جدید_مدیر, user.Id, order.CustomerOrderId, frontendUrl + "/profile/detail/" + order.CustomerOrderId, frontendUrl + "/profile/rate/" + order.CustomerOrderId, null, null, order.OrderDeliveries.First().ProductSendWay.Title, OrderStatus.تایید_پرداخت.EnumDisplayNameFor(), string.Format("{0:n0}", wallet.Price.ToString()), cityName, null, frontendUrl + "/profile/edit", (isestalam ? "استعلامی" : "عادی"), null, null, null, null, null, null, null, null, null, bankTitle);
                if (!string.IsNullOrEmpty(iPhonemessage.Body))
                    await sms.SendSMSAsync(iPhonemessage, "NewOrderAdmin", isestalam ? "استعلامی" : "عادی", null, null, string.Format("{0:n0}", wallet.Price.ToString()), cityName, true);
                order.AdminNotification = true;
                uow.OrderRepository.Update(order);
                uow.Save();
            }
            catch (Exception exp)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "callback-sms-admin", false, 500, exp.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }

            try
            {
                iPhonemessage.Destination = user.UserName;
                iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_سفارش_جدید_کاربر, user.Id, order.CustomerOrderId, frontendUrl + "/profile/detail/" + order.CustomerOrderId, frontendUrl + "/profile/rate/" + order.CustomerOrderId, null, null, null, OrderStatus.تایید_پرداخت.EnumDisplayNameFor(), string.Format("{0:n0}", wallet.Price.ToString()), (user.CityId.HasValue ? user.CityEntity.Name : order.OrderDeliveries.First().UserAddress.CityEntity.Name), null, frontendUrl + "/profile/edit");
                if (!string.IsNullOrEmpty(iPhonemessage.Body))
                    await sms.SendSMSAsync(iPhonemessage, "NewAddOrder", frontendUrl + "/profile/detail/" + order.CustomerOrderId, null, null, user.FirstName, order.CustomerOrderId.ToString(), true);
            }
            catch (Exception exp)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "callback-sms-user", false, 500, exp.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }

            var basketCookie = Request.Cookies["BasketProductPrice"];
            if (basketCookie != null)
            {
                basketCookie.Value = "";
                basketCookie.Expires = DateTime.Now.AddMonths(-6);
                Response.Cookies.Add(basketCookie);
            }
        }

        private const string CallbackOrderInclude = "OrderStates,OrderRows,OrderWallets,OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,OrderDeliveries.ProductSendWay,OrderDeliveries.UserAddress.CityEntity,OrderDeliveries.ProductSendWayWorkTime,OrderAttributeSelects.OrderAttribute";

        // معادل POST CartController.BankPaymentCallBack() — خودِ بانک ملت مستقیم به این URL هیت
        // می‌زنه (نه از طریق فرانت)، پس نه [JWTAuthorize] داره نه AuthToken لازم داره. بعد از
        // تایید/تسویه‌ی SOAP، مرورگر کاربر رو به صفحه‌ی نتیجه‌ی فرانت جدید ریدایرکت می‌کنه.
        [HttpPost]
        public async Task<ActionResult> MellatCallback()
        {
            string frontendUrl = System.Configuration.ConfigurationManager.AppSettings["NextJsFrontendUrl"] ?? "";
            string saleOrderId = Request.Params["SaleOrderId"];

            try
            {
                if (Request.Params["ResCode"] == null || Request.Params["ResCode"].ToString() != "0")
                    return Redirect(frontendUrl + "/cart/paymentresult/mellat/0?orderid=" + saleOrderId);

                try
                {
                    var orderId = Convert.ToInt64(saleOrderId);
                    var order = uow.OrderRepository.Get(x => x, x => x.BankOrderId == orderId, null, CallbackOrderInclude).Single();
                    // order.OrderWallets از قبل (توسط CallbackOrderInclude) eager-load شده و کاملاً
                    // در حافظه‌ست؛ ولی چون .FirstOrDefault(predicate) بعدی روی یه IQueryable صدا
                    // زده می‌شه، کل لامبدا (شاملِ order.OrderWallets.First().WalletId) به یه
                    // Expression Tree تبدیل و توسط EF سعی می‌شه به SQL ترجمه بشه - و چون
                    // order.OrderWallets یه Entity واقعیه (نه یه مقدار ساده)، EF با خطای "Unable to
                    // create a constant value of type Domain.OrderWallet" fail می‌کنه. رفعش: مقدارِ
                    // اسکالر (walletId) رو همین‌جا، قبل از ساختِ query، جدا محاسبه می‌کنیم.
                    var walletId = order.OrderWallets.First().WalletId;
                    var wallet = uow.WalletRepository.GetQueryList().Include(x => x.WalletAttributeWallets).Include("WalletAttributeWallets.WalletAttribute").FirstOrDefault(c => c.Id == walletId);
                    var bankAccount = uow.BankAccountRepository.GetQueryList().AsNoTracking().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 1);

                    var bpService = new ir.shaparak.bpm.PaymentGatewayImplService();
                    string result = bpService.bpVerifyRequest(Convert.ToInt64(bankAccount.BankAccountOnlineInfo.TerminalId), bankAccount.BankAccountOnlineInfo.UserName, bankAccount.BankAccountOnlineInfo.Password, orderId, orderId, Convert.ToInt64(Request.Params["saleReferenceId"]));
                    if (result != "0")
                        return Redirect(frontendUrl + "/cart/paymentresult/mellat/-2?orderid=" + saleOrderId);

                    result = bpService.bpSettleRequest(Convert.ToInt64(bankAccount.BankAccountOnlineInfo.TerminalId), bankAccount.BankAccountOnlineInfo.UserName, bankAccount.BankAccountOnlineInfo.Password, orderId, orderId, Convert.ToInt64(Request.Params["saleReferenceId"]));
                    if (result != "0")
                        return Redirect(frontendUrl + "/cart/paymentresult/mellat/-2?orderid=" + saleOrderId);

                    await FinalizeOrderPayment(order, wallet, bankAccount, Request.Params["SaleReferenceId"], saleOrderId, "ملت");

                    return Redirect(frontendUrl + "/cart/paymentresult/mellat/1?orderid=" + saleOrderId + "&traceno=" + Request.Params["SaleReferenceId"]);
                }
                catch (Exception ex)
                {
                    // ex.ToString() (نه فقط ex.Message) که StackTrace/InnerException هم توش باشه -
                    // برای دیباگِ بعدیِ راحت‌تر (دقیقاً همین چیزی بود که کمک کرد این باگ پیدا بشه).
                    Infrastructure.EventLog.Logger.Add(5, "api", "mellatcallback", false, 500, ex.ToString(), DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return Redirect(frontendUrl + "/cart/paymentresult/mellat/-1?orderid=" + saleOrderId);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "mellatcallback", false, 500, ex.ToString(), DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Redirect(frontendUrl + "/cart/paymentresult/mellat/-1?orderid=" + saleOrderId);
            }
        }

        // معادل POST CartController.SamanPaymentCallBack()
        [HttpPost]
        public async Task<ActionResult> SamanCallback()
        {
            string frontendUrl = System.Configuration.ConfigurationManager.AppSettings["NextJsFrontendUrl"] ?? "";
            string resNum = Request.Params["ResNum"];

            try
            {
                if (Request.Params["StateCode"] == null || Request.Params["StateCode"].ToString() != "0")
                    return Redirect(frontendUrl + "/cart/paymentresult/saman/0?orderid=" + resNum);

                try
                {
                    var orderId = Convert.ToInt64(resNum);
                    var order = uow.OrderRepository.Get(x => x, x => x.BankOrderId == orderId, null, CallbackOrderInclude).Single();
                    // order.OrderWallets از قبل (توسط CallbackOrderInclude) eager-load شده و کاملاً
                    // در حافظه‌ست؛ ولی چون .FirstOrDefault(predicate) بعدی روی یه IQueryable صدا
                    // زده می‌شه، کل لامبدا (شاملِ order.OrderWallets.First().WalletId) به یه
                    // Expression Tree تبدیل و توسط EF سعی می‌شه به SQL ترجمه بشه - و چون
                    // order.OrderWallets یه Entity واقعیه (نه یه مقدار ساده)، EF با خطای "Unable to
                    // create a constant value of type Domain.OrderWallet" fail می‌کنه. رفعش: مقدارِ
                    // اسکالر (walletId) رو همین‌جا، قبل از ساختِ query، جدا محاسبه می‌کنیم.
                    var walletId = order.OrderWallets.First().WalletId;
                    var wallet = uow.WalletRepository.GetQueryList().Include(x => x.WalletAttributeWallets).Include("WalletAttributeWallets.WalletAttribute").FirstOrDefault(c => c.Id == walletId);
                    var bankAccount = uow.BankAccountRepository.GetQueryList().AsNoTracking().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 8);

                    var paymentIFBinding = new TfShop.ir.shaparak.sep1.PaymentIFBinding();
                    double r = paymentIFBinding.verifyTransaction(Request.Params["RefNum"], bankAccount.BankAccountOnlineInfo.TerminalId);
                    if (r <= 0)
                        return Redirect(frontendUrl + "/cart/paymentresult/saman/-2?orderid=" + resNum);

                    await FinalizeOrderPayment(order, wallet, bankAccount, Request.Params["TRACENO"], resNum, "سامان");

                    return Redirect(frontendUrl + "/cart/paymentresult/saman/1?orderid=" + orderId + "&traceno=" + Request.Params["TRACENO"]);
                }
                catch (Exception ex)
                {
                    Infrastructure.EventLog.Logger.Add(5, "api", "samancallback", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return Redirect(frontendUrl + "/cart/paymentresult/saman/-1?orderid=" + resNum);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "samancallback", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Redirect(frontendUrl + "/cart/paymentresult/saman/-1?orderid=" + resNum);
            }
        }

        // معادل POST CartController.torobcallback() — verify سپس settle؛ اگه هرکدوم شکست بخوره
        // revert می‌زنه (دقیقاً مثل نسخه‌ی قدیمی)
        [HttpPost]
        public async Task<ActionResult> TorobCallback()
        {
            string frontendUrl = System.Configuration.ConfigurationManager.AppSettings["NextJsFrontendUrl"] ?? "";
            string transactionId = Request.Params["transactionId"];

            try
            {
                if (Request.Params["state"] == null || Request.Params["state"].ToString() != "OK")
                    return Redirect(frontendUrl + "/cart/paymentresult/torob/-1?orderid=" + transactionId);

                try
                {
                    var orderId = Convert.ToInt64(transactionId);
                    var order = uow.OrderRepository.Get(x => x, x => x.BankOrderId == orderId, null, CallbackOrderInclude).Single();
                    // order.OrderWallets از قبل (توسط CallbackOrderInclude) eager-load شده و کاملاً
                    // در حافظه‌ست؛ ولی چون .FirstOrDefault(predicate) بعدی روی یه IQueryable صدا
                    // زده می‌شه، کل لامبدا (شاملِ order.OrderWallets.First().WalletId) به یه
                    // Expression Tree تبدیل و توسط EF سعی می‌شه به SQL ترجمه بشه - و چون
                    // order.OrderWallets یه Entity واقعیه (نه یه مقدار ساده)، EF با خطای "Unable to
                    // create a constant value of type Domain.OrderWallet" fail می‌کنه. رفعش: مقدارِ
                    // اسکالر (walletId) رو همین‌جا، قبل از ساختِ query، جدا محاسبه می‌کنیم.
                    var walletId = order.OrderWallets.First().WalletId;
                    var wallet = uow.WalletRepository.GetQueryList().Include(x => x.WalletAttributeWallets).Include("WalletAttributeWallets.WalletAttribute").FirstOrDefault(c => c.Id == walletId);
                    var bankAccount = uow.BankAccountRepository.GetQueryList().AsNoTracking().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 1 && c.torob);

                    using (var client = new System.Net.WebClient())
                    {
                        client.Headers.Clear();
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(bankAccount.torob_client_id + ":" + bankAccount.torob_client_secret);
                        client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(plainTextBytes));
                        var vm = new { username = bankAccount.torob_username, password = bankAccount.torob_password };
                        client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                        string json = client.UploadString("https://cpg.torobpay.com/api/online/v1/oauth/token", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vm));
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<torob_token>(json);
                        if (data.access_token == null)
                            return Redirect(frontendUrl + "/cart/paymentresult/torob/-2?orderid=" + transactionId);

                        client.Headers.Clear();
                        client.Headers.Add("Authorization", "Bearer " + data.access_token);
                        client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                        var vmVerify = new { paymentToken = order.TorobPaymentToken };
                        string jsonVerify = client.UploadString("https://cpg.torobpay.com/api/online/payment/v1/verify", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vmVerify));
                        var dataVerify = Newtonsoft.Json.JsonConvert.DeserializeObject<torobpayment>(jsonVerify);

                        if (!dataVerify.successful)
                        {
                            client.Headers.Clear();
                            client.Headers.Add("Authorization", "Bearer " + data.access_token);
                            client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                            client.UploadString("https://cpg.torobpay.com/api/online/payment/v1/revert", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vmVerify));
                            return Redirect(frontendUrl + "/cart/paymentresult/torob/-2?orderid=" + transactionId);
                        }

                        client.Headers.Clear();
                        client.Headers.Add("Authorization", "Bearer " + data.access_token);
                        client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                        string jsonSettle = client.UploadString("https://cpg.torobpay.com/api/online/payment/v1/settle", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vmVerify));
                        var dataSettle = Newtonsoft.Json.JsonConvert.DeserializeObject<torobpayment>(jsonSettle);

                        if (!dataSettle.successful)
                        {
                            client.Headers.Clear();
                            client.Headers.Add("Authorization", "Bearer " + data.access_token);
                            client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json");
                            client.UploadString("https://cpg.torobpay.com/api/online/payment/v1/revert", "POST", Newtonsoft.Json.JsonConvert.SerializeObject(vmVerify));
                            return Redirect(frontendUrl + "/cart/paymentresult/torob/-2?orderid=" + transactionId);
                        }

                        await FinalizeOrderPayment(order, wallet, bankAccount, order.TorobPaymentToken, order.TorobPaymentToken, "ترب");

                        return Redirect(frontendUrl + "/cart/paymentresult/torob/1?orderid=" + transactionId + "&traceno=" + order.TorobPaymentToken);
                    }
                }
                catch (Exception ex)
                {
                    Infrastructure.EventLog.Logger.Add(5, "api", "torobcallback", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return Redirect(frontendUrl + "/cart/paymentresult/torob/-1?orderid=" + transactionId);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "torobcallback", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Redirect(frontendUrl + "/cart/paymentresult/torob/-1?orderid=" + transactionId);
            }
        }

        // معادل POST CartController.dgpaycallback() — verify؛ اگه شکست بخوره reverse می‌زنه
        [HttpPost]
        public async Task<ActionResult> DgpayCallback()
        {
            string frontendUrl = System.Configuration.ConfigurationManager.AppSettings["NextJsFrontendUrl"] ?? "";
            string providerId = Request.Params["providerId"];

            try
            {
                if (Request.Params["result"] == null || Request.Params["result"].ToString() != "SUCCESS")
                    return Redirect(frontendUrl + "/cart/paymentresult/dgpay/-1?orderid=" + providerId);

                try
                {
                    var orderId = Convert.ToInt64(providerId);
                    var order = uow.OrderRepository.Get(x => x, x => x.BankOrderId == orderId, null, CallbackOrderInclude).Single();
                    // order.OrderWallets از قبل (توسط CallbackOrderInclude) eager-load شده و کاملاً
                    // در حافظه‌ست؛ ولی چون .FirstOrDefault(predicate) بعدی روی یه IQueryable صدا
                    // زده می‌شه، کل لامبدا (شاملِ order.OrderWallets.First().WalletId) به یه
                    // Expression Tree تبدیل و توسط EF سعی می‌شه به SQL ترجمه بشه - و چون
                    // order.OrderWallets یه Entity واقعیه (نه یه مقدار ساده)، EF با خطای "Unable to
                    // create a constant value of type Domain.OrderWallet" fail می‌کنه. رفعش: مقدارِ
                    // اسکالر (walletId) رو همین‌جا، قبل از ساختِ query، جدا محاسبه می‌کنیم.
                    var walletId = order.OrderWallets.First().WalletId;
                    var wallet = uow.WalletRepository.GetQueryList().Include(x => x.WalletAttributeWallets).Include("WalletAttributeWallets.WalletAttribute").FirstOrDefault(c => c.Id == walletId);
                    var bankAccount = uow.BankAccountRepository.GetQueryList().AsNoTracking().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 1 && c.dgpay);

                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.mydigipay.com/digipay/api/oauth/token");
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(bankAccount.dgpay_client_id + ":" + bankAccount.dgpay_client_secret);
                        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(plainTextBytes));
                        var content = new System.Net.Http.MultipartFormDataContent();
                        content.Add(new System.Net.Http.StringContent(bankAccount.dgpay_username), "username");
                        content.Add(new System.Net.Http.StringContent(bankAccount.dgpay_password), "password");
                        content.Add(new System.Net.Http.StringContent("password"), "grant_type");
                        request.Content = content;

                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dgpay_token>(json);
                        if (data.access_token == null)
                            return Redirect(frontendUrl + "/cart/paymentresult/dgpay/-2?orderid=" + providerId);

                        var request2 = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.mydigipay.com/digipay/api/purchases/verify?type=5");
                        request2.Headers.Add("Authorization", "Bearer " + data.access_token);
                        var vm2 = new { trackingCode = Request.Params["trackingCode"], providerId = Request.Params["providerId"] };
                        request2.Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(vm2), System.Text.Encoding.UTF8, "application/json");
                        request2.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                        var response2 = await client.SendAsync(request2);
                        response2.EnsureSuccessStatusCode();
                        string json2 = await response2.Content.ReadAsStringAsync();
                        var data2 = Newtonsoft.Json.JsonConvert.DeserializeObject<dgpayticket>(json2);

                        if (data2.result.status != 0)
                        {
                            var request3 = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.mydigipay.com/digipay/api/reverse");
                            request3.Headers.Add("Authorization", "Bearer " + data.access_token);
                            request3.Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(vm2), System.Text.Encoding.UTF8, "application/json");
                            request3.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                            await client.SendAsync(request3);
                            return Redirect(frontendUrl + "/cart/paymentresult/dgpay/-2?orderid=" + providerId);
                        }

                        order.Dgpayamount = Convert.ToInt64(Request.Params["amount"]);
                        order.DgpaytrackingCode = Request.Params["trackingCode"];
                        order.Dgpayresult = Request.Params["result"];
                        order.Dgpaytype = Convert.ToInt32(Request.Params["type"]);
                        short gt = Convert.ToInt16(data2.paymentGateway);
                        order.DgpaypaymentGateway = gt == 0 ? "IPG" : gt == 3 ? "WALLET" : "CPG(اعتباری)";
                        order.DgpayJson = json2;

                        await FinalizeOrderPayment(order, wallet, bankAccount, Request.Params["trackingCode"], providerId, "دیجی پی");

                        return Redirect(frontendUrl + "/cart/paymentresult/dgpay/1?orderid=" + providerId + "&traceno=" + Request.Params["trackingCode"]);
                    }
                }
                catch (Exception ex)
                {
                    Infrastructure.EventLog.Logger.Add(5, "api", "dgpaycallback", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return Redirect(frontendUrl + "/cart/paymentresult/dgpay/-1?orderid=" + providerId);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "dgpaycallback", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Redirect(frontendUrl + "/cart/paymentresult/dgpay/-1?orderid=" + providerId);
            }
        }

        // معادل GET CartController.*CallBack(id, orderid/transactionId, TRACENO) — یه اکشن مشترک برای
        // هر ۴ درگاه، چون منطق‌شون یکسانه: به‌جای اعتماد به پارامتر id توی URL (که با نسخه‌ی قدیمی
        // فرق داره)، خودِ وضعیت واقعیِ OrderStates رو چک می‌کنه (مطمئن‌تره، قابل دستکاری با ادیت URL
        // نیست). این اکشن هر بار که صفحه‌ی نتیجه توی فرانت باز بشه صدا زده می‌شه: CallbackSeen رو
        // true می‌کنه و اطلاعات لازم برای نمایش پیام موفقیت/شکست رو برمی‌گردونه.
        [HttpPost]
        [JWTAuthorize]
        public JsonResult CallbackResult(long orderid, string traceno)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { url = "/account/login", statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                var order = uow.OrderRepository.Get(x => x, x => x.BankOrderId == orderid && x.UserId == userid, null,
                    "OrderStates,OrderDeliveries.ProductSendWay,OrderRows,OrderWallets.Wallet.BankAccount,OrderAttributeSelects.OrderAttribute").FirstOrDefault();
                if (order == null)
                    return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

                bool paid = order.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت);

                order.CallbackSeen = true;
                if (!paid)
                    order.CallbackFailedBank = true;
                uow.OrderRepository.Update(order);
                uow.Save();

                // «مشخصات سفارش» - عیناً همون منطقی که توی Views/Cart/BankPaymentCallBack.cshtml قدیمی
                // بود (هزینه ارسال از OrderAttributeSelect با DataType==14، ارسال‌رایگان/پس‌کرایه از
                // DataType==29)، چون فرانتِ جدید دیگه به این Viewها دسترسی نداره و باید همینجا محاسبه بشه.
                var walletPrice = order.OrderWallets.First().Wallet.Price;
                var productsPrice = order.OrderRows.Sum(x => x.Price * x.Quantity);

                int shippingPrice = 0;
                if (order.OrderAttributeSelects.Any(x => x.OrderAttribute.DataType == 14))
                {
                    foreach (var item in order.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 14))
                        shippingPrice += Convert.ToInt32(item.Value);
                }
                string shippingText;
                if (shippingPrice > 0)
                {
                    shippingText = string.Format("{0:n0}", shippingPrice) + " تومان";
                }
                else
                {
                    var offFreeSend = order.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 29).FirstOrDefault();
                    if (offFreeSend != null)
                        shippingText = offFreeSend.Value == "1" ? "ارسال رایگان" : "پس کرایه";
                    else
                        shippingText = "پس کرایه";
                }

                var firstDelivery = order.OrderDeliveries.FirstOrDefault();

                if (paid)
                {
                    return Json(new
                    {
                        status = 1,
                        customerOrderId = order.CustomerOrderId,
                        traceno = traceno,
                        isFreeSend = firstDelivery?.ProductSendWay?.FreeOff ?? false,
                        walletPrice = walletPrice,
                        productsPrice = productsPrice,
                        shippingText = shippingText,
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

                double minutesLeft = Math.Max(0, Math.Ceiling((order.ExpireDate - DateTime.Now).TotalMinutes));
                return Json(new
                {
                    status = 0,
                    minutesLeft = minutesLeft,
                    customerOrderId = order.CustomerOrderId,
                    walletPrice = walletPrice,
                    productsPrice = productsPrice,
                    shippingText = shippingText,
                    bankOrderId = order.BankOrderId,
                    bankId = order.OrderWallets.First().Wallet.BankAccount?.BankId,
                    deliveryId = firstDelivery?.Id,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "callbackresult", false, 500, ex.ToString(), DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new { Message = "خطایی رخ داد.", statusCode = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        // معادل JSON اکشن قدیمی CartController.CardPay (POST) — کاربر بعد از انتقال کارت‌به‌کارت،
        // عکس رسید پرداخت رو آپلود می‌کنه. سفارش باید قبلاً با AddOrder(paymentType=3) ساخته شده باشه.
        // برخلاف نسخه‌ی قدیمی، پیامک‌ها/UserActivity به عهده‌ی پنل ادمین (روی همون سفارش) گذاشته می‌شه؛
        // این اکشن فقط عکس رسید رو ثبت می‌کنه تا وضعیت سفارش برای بررسی ادمین آماده بشه.
        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> CardPay(long BankOrderId, HttpPostedFileBase[] UploadedImages)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userid = Authentication.ValidateToken(Token);
            if (userid == null)
                return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

            try
            {
                DateTime nowdateTime = DateTime.Now;
                var order = uow.OrderRepository.Get(x => x, x => x.BankOrderId == BankOrderId && x.UserId == userid
                    && x.CancelingSms == false && x.IsOld == false && x.IsExpire == false
                    && x.OrderStates.Any(s => s.state == OrderStatus.تایید_سفارش)
                    && !x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت)
                    && !x.OrderStates.Any(s => s.state == OrderStatus.لغو_شده)
                    && !x.OrderStates.Any(s => s.state == OrderStatus.درخواست_لغو),
                    null, "OrderWallets.Wallet").FirstOrDefault();

                if (order == null)
                    return Json(new { statusCode = 404 }, JsonRequestBehavior.AllowGet);

                if (order.OrderWallets.First().Wallet.PaymentType != 3)
                    return Json(new { Message = "این سفارش با روش کارت‌به‌کارت ثبت نشده.", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                if (nowdateTime >= order.ExpireDate)
                    return Json(new { Message = "مهلت پرداخت این سفارش به پایان رسیده است.", statusCode = 4051 }, JsonRequestBehavior.AllowGet);

                if (UploadedImages == null || !UploadedImages.Any(x => x != null))
                    return Json(new { Message = "عکس رسید پرداختی را وارد نمایید.", statusCode = 4052 }, JsonRequestBehavior.AllowGet);

                string messages;
                List<attachment> newAttachmentList;
                var result = UploadMultipleFile(UploadedImages, UploadedImages.Select(x => x.FileName).ToArray(), "on", "3", "on", "on", 1, 1437, "1", false, "", out messages, out newAttachmentList, userid);

                if (result == null || newAttachmentList == null || !newAttachmentList.Any())
                    return Json(new { Message = "در آپلود تصویر خطایی رخ داد.", statusCode = 500 }, JsonRequestBehavior.AllowGet);

                order.AdminPayAttach = newAttachmentList.First().Id;
                uow.OrderRepository.Update(order);
                await uow.SaveAsync();

                return Json(new { statusCode = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "cardpay", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetCart(List<Basket> BasketItems, int? m)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userid = Token != null ? Authentication.ValidateToken(Token) : null;


                var step1 = GetBasket(BasketItems, 1, userid);
                if (step1 == null)
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                else if (step1.NotvalidBox == true)
                    return Json(new
                    {
                        statusCode = 501
                    }, JsonRequestBehavior.AllowGet);




                var setting = GetSetting();
                bool MergOrder = false;
                List<SuspendOrder> SuspendOrders = new List<SuspendOrder>();
                string hours = "";
                if (userid != null)
                {

                    DateTime nowdateTime = DateTime.Now;
                    if (!setting.SiteClosed)
                    {
                        var suspendOrders = uow.OrderRepository.Get(x => new Domain.ViewModel.SuspendOrder() { ExpireDate = x.ExpireDate, BankId = x.OrderWallets.First().Wallet.BankAccount.BankId, bankOrderid = x.BankOrderId, customerOrderid = x.CustomerOrderId, deliverId = x.OrderDeliveries.First().Id, price = x.OrderWallets.First().Wallet.Price }, x => x.OrderWallets.Any(s => s.Wallet.PaymentType != 4 && s.Wallet.PaymentType != 5 && s.Wallet.PaymentType != 3) && x.UserId == userid && x.IsOld == false && x.IsExpire == false && (x.ExpireDate != null && x.ExpireDate > nowdateTime) && !x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23)) && !x.OrderStates.Any(s => s.state >= Domain.OrderStatus.تایید_پرداخت), null, "OrderDeliveries,OrderStates,OrderRows,OrderWallets.Wallet.BankAccount");
                        SuspendOrders = suspendOrders.ToList();
                        if (suspendOrders.Any())
                            hours = System.Math.Ceiling((suspendOrders.First().ExpireDate - DateTime.Now).TotalMinutes).ToString();
                    }

                    #region check Current Order
                    // آیا کاربر سفارش بازی دارد؟
                    if (uow.OrderRepository.Any(x => x.Id, x => x.UserId == userid && ((!x.OrderStates.Any(a => a.state > OrderStatus.تایید_سفارش) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_سفارش) && x.OrderWallets.Any(s => s.Wallet.PaymentType == 4 || s.Wallet.PaymentType == 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.تایید_پرداخت) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت) && x.OrderWallets.Any(s => s.Wallet.PaymentType != 4 && s.Wallet.PaymentType != 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.پردازش_انبار) && x.OrderStates.Any(a => a.state == OrderStatus.پردازش_انبار)))))
                    {
                        var currentOrder = uow.OrderRepository.Get(x => x, x => x.UserId == userid && ((!x.OrderStates.Any(a => a.state > OrderStatus.تایید_سفارش) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_سفارش) && x.OrderWallets.Any(s => s.Wallet.PaymentType == 4 || s.Wallet.PaymentType == 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.تایید_پرداخت) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت) && x.OrderWallets.Any(s => s.Wallet.PaymentType != 4 && s.Wallet.PaymentType != 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.پردازش_انبار) && x.OrderStates.Any(a => a.state == OrderStatus.پردازش_انبار))), null, "OrderDeliveries.ProductSendWay.ProductSendWayBoxes.SendwayBox,OrderRows.ProductPrice.Product").FirstOrDefault();
                        //آیا میتوان سبد خرید جاری را با این سفارش ارسال کرد؟
                        var stepp2 = GetBasketStep2(BasketItems, userid);
                        // اگر سبد خرید یک مرسوله ای باشد
                        if (stepp2.basketStep1.ProductBoxes.Select(x => x.SendwayBox.ProductPackageType).Distinct().Count() == 1)
                        {
                            var currentBox = stepp2.basketStep1.ProductBoxes.Select(x => x.SendwayBox).OrderByDescending(x => x.Lenght * x.Width * x.Height).First();
                            //آیا این باکس برای این روش ارسال می شود و ماهیت ارسال ها یکی ست؟
                            if (currentOrder.OrderDeliveries.First().ProductSendWay.ProductSendWayBoxes.Any(a => a.SendwayBox.Id == currentBox.Id) && Convert.ToInt32(currentOrder.OrderDeliveries.First().ProductPackageType) == currentBox.ProductPackageType)
                            {
                                ////آیا حجم و وزن جا می شود؟
                                //int boxMass = currentBox.Lenght * currentBox.Width * currentBox.Height;
                                //int CurrentOrderMass = currentOrder.OrderRows.Max(x => x.ProductPrice.Product.Lenght * x.ProductPrice.Product.Width * x.ProductPrice.Product.Height);
                                //int CurrentOrderWieght = currentOrder.OrderRows.Sum(x => x.ProductPrice.Product.ProductWeight);
                                //List<int> ids = stepp2.basketStep1.ProductBoxes.SelectMany(a => a.ProductPriceIdList).ToList();
                                //int OrderMass = uow.ProductPriceRepository.Max(x => x.Product.Lenght * x.Product.Width * x.Product.Height, x => ids.Contains(x.Id));
                                //int OrderWieght = uow.ProductPriceRepository.Sum(x => x.Product.ProductWeight, x => ids.Contains(x.Id));
                                //if ((boxMass - CurrentOrderMass) >= OrderMass && (currentBox.ProductWeight - CurrentOrderWieght) >= OrderWieght)
                                //{
                                MergOrder = true;
                                //}
                            }
                        }
                    }

                    #endregion
                }

                string message = "";
                switch (m)
                {
                    case 1: message = "متاسفانه دیگر امکان انتخاب این بازه زمانی تحویل وجود ندارد."; break;
                    case 2: message = "برخی از کالاهای شما ناموجود شدند. پس از بررسی، اقدام به ادامه فرآیند خرید نمایید."; break;
                    case 3: message = "برخی از کالاهای شما تغییر قیمت داشته اند. پس از بررسی و در صورت تمایل اقدام به ادامه فرآیند خرید نمایید."; break;
                    case 4: message = "کد تخفیف وارد شده معتبر نمی باشد !"; break;
                    case 5: message = "شما شرایط لازم برای استفاده از ارسال رایگان را ندارید. از روش های دیگر استفاده نمایید."; break;
                    default: break;
                }

                return Json(new
                {
                    MergOrder = MergOrder,
                    SuspendOrders = SuspendOrders,
                    hours = hours,
                    message = message,
                    capacity = string.Format("{0:n0} لیتر", step1.ProductBasketItems.Sum(x => x.l * x.w * x.h * x.UserQuantity) / 1000),
                    box = step1.ProductBoxes.Select(x => x.SendwayBox.Title).ToList(),
                    weight = string.Format("{0:n0} گرم", step1.ProductBasketItems.Sum(x => x.we * x.UserQuantity)),
                    BasketCount = BasketItems.Count,
                    data = step1,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getcart", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [JWTAuthorize]
        public ActionResult MergeOrder(List<Basket> BasketItems)
        {
            try
            {
                if (BasketItems == null)
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);


                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var userid = Authentication.ValidateToken(Token);
                if (userid == null)
                    return Json(new
                    {
                        url = "/cart",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                if (BasketItems.Any())
                {

                    BasketStep1V2 basketStep1 = new BasketStep1V2();
                    //محصولاتِ در سبد
                    basketStep1.ProductBasketItems = uow.ProductPriceRepository.ProductBasketItemV2(BasketItems);
                    foreach (var item in basketStep1.ProductBasketItems)
                    {
                        BasketItems.Where(x => x.ProductPriceId == item.Id).First().Price = item.rawprice;
                        if (BasketItems.Where(x => x.ProductPriceId == item.Id).First().Quantity == 0)
                            BasketItems.Remove(BasketItems.Where(x => x.ProductPriceId == item.Id).First());
                    }

                    // آیا کاربر سفارش بازی دارد؟
                    if (uow.OrderRepository.Any(x => x.Id, x => x.UserId == userid && ((!x.OrderStates.Any(a => a.state > OrderStatus.تایید_سفارش) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_سفارش) && x.OrderWallets.Any(s => s.Wallet.PaymentType == 4 || s.Wallet.PaymentType == 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.تایید_پرداخت) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت) && x.OrderWallets.Any(s => s.Wallet.PaymentType != 4 && s.Wallet.PaymentType != 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.پردازش_انبار) && x.OrderStates.Any(a => a.state == OrderStatus.پردازش_انبار)))))
                    {
                        List<orderMergeVM> orders = new List<orderMergeVM>();
                        // این محاسبه فقط به BasketItems/userid وابسته‌ست، نه به currentOrder — قبلاً داخل
                        // foreach بود و برای هر سفارش دوباره از صفر (با کوئری‌های سنگین) محاسبه می‌شد؛ چون
                        // نتیجه‌ش برای همه‌ی تکرارها یکسانه، بیرون از حلقه آورده شد تا فقط یه‌بار اجرا بشه
                        // (یکی از دلایل اصلی کند بودن این صفحه).
                        var stepp2 = GetBasketStep2(BasketItems, userid);
                        foreach (var currentOrder in uow.OrderRepository.Get(x => x, x => x.UserId == userid && ((!x.OrderStates.Any(a => a.state > OrderStatus.تایید_سفارش) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_سفارش) && x.OrderWallets.Any(s => s.Wallet.PaymentType == 4 || s.Wallet.PaymentType == 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.تایید_پرداخت) && x.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت) && x.OrderWallets.Any(s => s.Wallet.PaymentType != 4 && s.Wallet.PaymentType != 5)) || (!x.OrderStates.Any(a => a.state > OrderStatus.پردازش_انبار) && x.OrderStates.Any(a => a.state == OrderStatus.پردازش_انبار))), null, "OrderDeliveries.ProductSendWay.ProductSendWayBoxes.SendwayBox,OrderRows.ProductPrice.Product,OrderDeliveries.UserAddress,OrderDeliveries.ProductSendWayWorkTime,OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.UserAddress.CityEntity.Province,User.CityEntity.Province,OrderWallets.Wallet,OrderStates"))
                        {
                            // اگر سبد خرید یک مرسوله ای باشد
                            if (stepp2.basketStep1.ProductBoxes.Select(x => x.SendwayBox.ProductPackageType).Distinct().Count() == 1)
                            {
                                var currentBox = stepp2.basketStep1.ProductBoxes.Select(x => x.SendwayBox).OrderByDescending(x => x.Lenght * x.Width * x.Height).First();

                                // قبلاً همه‌ی این‌ها با .First()/.Last() بدون چک null بودن گرفته می‌شدن؛ اگه یه
                                // سفارش هنوز OrderDelivery یا OrderWallet یا OrderState ثبت‌شده نداشت (یا یکی از
                                // محصولاتش هیچ عکسی نداشت)، کل اکشن exception می‌خورد و می‌رفت توی catch بیرونی
                                // (statusCode=500 بدون data/basketStep1) — یعنی نه فقط این سفارش، بلکه کل لیست
                                // سفارش‌های قابل‌ادغام از دست می‌رفت. الان چک می‌شه و این سفارش خاص فقط skip
                                // می‌شه، نه کل درخواست.
                                var currentDelivery = currentOrder.OrderDeliveries.FirstOrDefault();
                                var currentWallet = currentOrder.OrderWallets.FirstOrDefault();
                                var currentState = currentOrder.OrderStates.OrderByDescending(s => s.Id).FirstOrDefault();
                                if (currentDelivery?.ProductSendWay == null || currentWallet == null || currentState == null)
                                    continue;

                                //آیا این باکس برای این روش ارسال می شود و ماهیت ارسال ها یکی ست؟
                                if (currentDelivery.ProductSendWay.ProductSendWayBoxes.Any(a => a.SendwayBox.Id == currentBox.Id) && currentDelivery.ProductPackageType == (Domain.ProductPackageType)currentBox.ProductPackageType)
                                {
                                    ////آیا حجم و وزن جا می شود؟
                                    //int boxMass = currentBox.Lenght * currentBox.Width * currentBox.Height;
                                    //int CurrentOrderMass = currentOrder.OrderRows.Max(x => x.ProductPrice.Product.Lenght * x.ProductPrice.Product.Width * x.ProductPrice.Product.Height);
                                    //int CurrentOrderWieght = currentOrder.OrderRows.Sum(x => x.ProductPrice.Product.ProductWeight);
                                    //List<int> ids = stepp2.basketStep1.ProductBoxes.SelectMany(a => a.ProductPriceIdList).ToList();
                                    //int OrderMass = uow.ProductPriceRepository.Max(x => x.Product.Lenght * x.Product.Width * x.Product.Height, x => ids.Contains(x.Id));
                                    //int OrderWieght = uow.ProductPriceRepository.Sum(x => x.Product.ProductWeight, x => ids.Contains(x.Id));
                                    //if ((boxMass - CurrentOrderMass) >= OrderMass && (currentBox.ProductWeight - CurrentOrderWieght) >= OrderWieght)
                                    //{
                                    orders.Add(new orderMergeVM()
                                    {
                                        CustomerOrderId = currentOrder.CustomerOrderId,
                                        Id = currentOrder.Id.ToString(),
                                        InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(currentOrder.InsertDate),
                                        Price = currentWallet.Wallet.Price,
                                        OrderDeliveryId = currentDelivery.Id,
                                        State = currentState.state.EnumDisplayNameFor(),
                                        OrderRows = currentOrder.OrderRows.Select(x => new OrderRowsVM()
                                        {
                                            Id = x.Id,
                                            OrderId = x.OrderId.ToString(),
                                            MainImage = x.ProductPrice.ProductImages.Any(a => a.IsMain)
                                                ? x.ProductPrice.ProductImages.Where(a => a.IsMain).Select(a => a.Image.FileName).FirstOrDefault()
                                                : x.ProductPrice.ProductImages.Select(a => a.Image.FileName).FirstOrDefault(),
                                            Title = x.ProductPrice.Product.Title
                                        })
                                    });
                                    //}
                                }
                            }
                        }

                        return Json(new
                        {
                            basketStep1 = basketStep1,
                            data = orders,
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else return Json(new
                    {
                        url = "/cart",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "mergeorder", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }



        }

        [HttpPost]
        public JsonResult GetBasket(List<Basket> BasketItems)
        {
            if (BasketItems == null)
                return Json(new
                {
                    msg = "سبد شما خالی ست !",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);

            try
            {
                Domain.ViewModels.BasketShort products = null;
                if (BasketItems != null)
                {
                    if (BasketItems.Any())
                        products = uow.ProductPriceRepository.ProductBasketItemShort(BasketItems, null);
                }
                return Json(new
                {
                    BasketCount = BasketItems.Count,
                    data = products,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getbasket", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion

        #region product

        private IEnumerable<ProductItemVM> productlist(int? catId, int? brandid, List<int> pids, int? tagid, bool hasoffer, bool hasamazing, int sort, bool sorttype, bool state, int skip, int take)
        {
            try
            {
                var dt = DateTime.Now;
                var products = context.Products.AsQueryable().Where(x => x.IsActive);
                if (catId.HasValue)
                {
                    List<int> CatIds = context.Database.SqlQuery<int>("exec GetSubCats @CatId", new SqlParameter("@CatId", catId.Value)).ToList();
                    products = products.Where(x => x.ProductCategories.Any(s => CatIds.Contains(s.Id)));
                }
                if (brandid.HasValue)
                    products = products.Where(x => x.BrandId == brandid);
                if (tagid.HasValue)
                    products = products.Where(x => x.Tags.Any(s => s.Id == tagid));
                if (pids != null)
                    if (pids.Any())
                        products = products.Where(x => pids.Contains(x.Id));
                if (hasoffer)
                    products = products.Where(x => x.ProductPrices.Any(p => p.ProductOffers.Any(s => s.Offer.CodeTypeValueCode == 2 && s.Quantity > 0 && s.Value > 0 && s.ProductPrice.ProductStateId < 3 && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= dt) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= dt) || s.Offer.StartDate == null))));
                if (hasamazing)
                    products = products.Where(x => x.ProductPrices.Any(p => p.ProductOffers.Any(s => s.Offer.CodeTypeValueCode == 1 && s.Quantity > 0 && s.Value > 0 && s.ProductPrice.ProductStateId < 3 && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= dt) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= dt) || s.Offer.StartDate == null))));
                if (state)
                    products = products.Where(x => x.ProductPrices.Any(s => s.IsDefault && (s.ProductStateId < 3 || s.ProductStateId == 6)));

                if (sorttype == false)
                {
                    switch (sort)
                    {
                        case 0:
                            products = products.OrderByDescending(x => x.Id); break;
                        case 1:
                            products = products.OrderByDescending(x => x.DisplaySort); break;
                        case 2:
                            products = products.OrderByDescending(x => x.InsertDate); break;
                        case 3:
                            products = products.OrderByDescending(x => x.UpdateDate); break;
                        case 4:
                            products = products.OrderByDescending(x => Guid.NewGuid()); break;
                        case 5:
                            products = products.OrderByDescending(x => x.SellCount); break;
                        case 6:
                            products = products.OrderByDescending(x => x.FavCount); break;
                        default:
                            products.OrderByDescending(x => x.Id); break;
                    }
                }
                else
                {
                    switch (sort)
                    {
                        case 0:
                            products = products.OrderBy(x => x.Id); break;
                        case 1:
                            products = products.OrderBy(x => x.DisplaySort); break;
                        case 2:
                            products = products.OrderBy(x => x.InsertDate); break;
                        case 3:
                            products = products.OrderBy(x => x.UpdateDate); break;
                        case 4:
                            products = products.OrderBy(x => Guid.NewGuid()); break;
                        case 5:
                            products = products.OrderBy(x => x.SellCount); break;
                        case 6:
                            products = products.OrderBy(x => x.FavCount); break;
                        default:
                            products = products.OrderBy(x => x.Id); break;
                    }
                }
                return products.Include(x => x.ProductPrices).Include("ProductPrices.ProductState").Include("ProductPrices.ProductAttributeSelectModel").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductIcon").AsNoTracking().Skip(() => skip).Take(() => take).ToList().Select(x => new ProductItemVM()
                {
                    Id = x.Id,
                    PrId = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id : 0,
                    shortTitle = x.ProductPrices.Count > 1 ? (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectModelId.HasValue ? " مدل " + x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectModel.Value : "") : x.Title,
                    title = x.Title,
                    pageAdress = string.Format("/tfp/{0}/{1}", x.Id, CommonFunctions.NormalizeAddress(x.PageAddress.ToLower())),
                    colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)).ToList() : null,
                    cover = GetMainImageFileName(x.Id),
                    productStateId = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId : null,
                    productStateTitle = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title : "",
                    icon = x.ProductIconId.HasValue ? x.ProductIcon.Title : "",
                    price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                    finalPrice = x.ProductPrices.Any(a => a.IsDefault) ? cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.FirstOrDefault().Id, x.BrandId, cprice) : 0,
                    offValue = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0 : 0,
                    offType = cofftype,
                    hasoff = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false : false,
                    rateAvg = x.RateAvg,
                    rateCount = x.CountAvg,
                    priceCount = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectSizeId.HasValue || s.ProductAttributeSelectGarantyId.HasValue || s.ProductAttributeSelectWeightId.HasValue).Count() : 0
                });
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "productlist", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetProductBreadCrumbs(int id)
        {

            try
            {
                var pr = context.Products.AsQueryable().Include("ProductCategories").AsNoTracking().Where(x => x.Id == id).Select(x => new { ProductCategory = x.ProductCategories.FirstOrDefault() }).SingleOrDefault();
                if (pr == null)
                {
                    return Json(new
                    {
                        msg = "no product exist!",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var tfs2 = new { id = pr.ProductCategory.Id, Name = pr.ProductCategory.Name, pageaddress = string.Format("/tfs/{0}/{1}", pr.ProductCategory.Id, CommonFunctions.NormalizeAddress(pr.ProductCategory.PageAddress.ToLower())) };
                    if (pr.ProductCategory.ParentCat.ParentCat != null)
                    {
                        var tfs = new { id = pr.ProductCategory.ParentCat.Id, Name = pr.ProductCategory.ParentCat.Name, pageaddress = string.Format("/tfs/{0}/{1}", pr.ProductCategory.ParentCat.Id, CommonFunctions.NormalizeAddress(pr.ProductCategory.ParentCat.PageAddress.ToLower())) };
                        var tfc = new { id = pr.ProductCategory.ParentCat.ParentCat.Id, Name = pr.ProductCategory.ParentCat.ParentCat.Name, pageaddress = string.Format("/tfc/{0}/{1}", pr.ProductCategory.ParentCat.ParentCat.Id, CommonFunctions.NormalizeAddress(pr.ProductCategory.ParentCat.ParentCat.PageAddress.ToLower())) };
                        return Json(new
                        {
                            tfs2 = tfs2,
                            tfs = tfs,
                            tfc = tfc,
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var tfc = new { id = pr.ProductCategory.ParentCat.Id, Name = pr.ProductCategory.ParentCat.Name, pageaddress = string.Format("/tfc/{0}/{1}", pr.ProductCategory.ParentCat.Id, CommonFunctions.NormalizeAddress(pr.ProductCategory.ParentCat.PageAddress.ToLower())) };
                        return Json(new
                        {
                            tfs2 = tfs2,
                            tfc = tfc,
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductbreadcrumbs", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpGet]
        public JsonResult GetProductList(int? catId, int? brandid, int? tagid, bool hasoffer, bool hasamazing, int sort, bool sorttype, bool state, int skip, int take)
        {
            try
            {
                return Json(new
                {
                    data = productlist(catId, brandid, null, tagid, hasoffer, hasamazing, sort, sorttype, state, skip, take),
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductlist", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public long GetPrice(int ProductId, long price, int? taxid)
        {

            ctaxtvalue = 0;
            cprice = price;
            cprice = Convert.ToInt64(Math.Ceiling(cprice * 0.001) * 1000);
            return cprice;

        }

        public IEnumerable<string> GetColor(IEnumerable<string> ColorIds)
        {
            if (ColorIds != null)
            {
                return context.ProductAttributeItemColors.Where(x => ColorIds.Contains(x.Id.ToString())).Select(x => x.Value);
            }
            else
                return null;
        }
        public string GetMainImageFileName(int pId)
        {

            var sqlQuery = @" 
                SELECT top(1) FIRST_VALUE( attachments.FileName) OVER ( ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc)  FROM ProductPrices AS pp
                       LEFT OUTER   JOIN ProductImages ON pp.Id = ProductImages.ProductPriceId
                        LEFT OUTER JOIN attachments ON ProductImages.AttachementId = attachments.Id

                        WHERE 
                         pp.ProductId =  " + pId + @" AND  ProductImages.IsImage = 1
                        ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc";
            var queryResult = context.Database.SqlQuery<string>(sqlQuery).AsQueryable();
            return queryResult.FirstOrDefault();

        }

        public long GetOff(int ProductPriceId, int CatId, int? BrandId, long price)
        {
            offerId = null;
            chasoff = false;
            var productoffer = context.ProductOffers.Include("Offer").Include("ProductPrice").Where(s => s.Quantity > 0 && s.Value > 0 && s.ProductPrice.ProductStateId < 3 && s.ProductPriceId == ProductPriceId && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= DateTime.Now) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= DateTime.Now) || s.Offer.StartDate == null)).FirstOrDefault();
            if (productoffer != null)
            {
                offexpiredate = productoffer.Offer.ExpireDate.HasValue ? productoffer.Offer.ExpireDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + productoffer.Offer.ExpireDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "";
                ProductOfferType = productoffer.Offer.CodeTypeValueCode == 2 ? false : true;
                offtitle = productoffer.Offer.Title;
                cofftype = productoffer.CodeType;
                offerQuantity = productoffer.Quantity;
                offerMaxQuantity = productoffer.MaxBasketCount;
                chasoff = true;
                offerId = productoffer.Id;
                return ComputeOffVaule(productoffer.CodeType, productoffer.Value, price);

            }
            else
            {
                chasoff = false;
                cofftype = 3;
                coffvalue = 0;
                coffvaluefinal = 0;
                return 0;
            }
        }

        public long ComputeOffVaule(short codeType, int value, long price)
        {
            if (codeType == 2)
            {
                coffvaluefinal = Convert.ToInt64(price * (value * 0.01));
                coffvaluefinal = Convert.ToInt64(Math.Ceiling(coffvaluefinal * 0.001) * 1000);
                coffvalue = value;
                return coffvaluefinal;
            }
            else if (codeType == 1)
            {
                coffvaluefinal = value;
                coffvaluefinal = Convert.ToInt64(Math.Ceiling(coffvaluefinal * 0.001) * 1000);
                coffvalue = value;
                return coffvaluefinal;
            }
            else
            {
                coffvalue = 0;
                coffvaluefinal = 0;
                return 0;
            }
        }

        #region Favorate LetmeKnow

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> AddFavorate(int id)
        {

            //UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            //userActivitiesObj.SaveUserActivity(null, id);
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var uid = Authentication.ValidateToken(Token);
                if (uid != null)
                {

                    if (!uow.ProductFavorateRepository.Any(x => x.Id, x => x.ProductId == id && x.UserId == uid))
                    {
                        await uow.ProductFavorateRepository.addFavorate(id, uid);
                        await uow.ProductRepository.addProductFavorate(id);
                    }
                    return Json(new
                    {
                        statusCode = 200,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 403,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addfavorate", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد " + ex.Message,
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        [JWTAuthorize]
        public JsonResult GetLetmeKnow(int id)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var uid = Authentication.ValidateToken(Token);
                if (uid != null)
                {
                    string userid = uid;

                    var mobile = uow.UserRepository.Get(x => x.PhoneNumber, x => x.Id == userid).SingleOrDefault();
                    var state = uow.ProductPriceRepository.Get(x => x.ProductStateId, x => x.Id == id).SingleOrDefault();
                    var letmeknow = uow.ProductLetmeknowRepository.Get(x => new { lid = x.Id, x.NotificationType, x.Available, x.AmazingOffer }, x => x.ProductPriceId == id && x.UserId == userid, null, "User").FirstOrDefault();
                    if (letmeknow != null)
                    {
                        return Json(new
                        {
                            data = new { letmeknow.AmazingOffer, letmeknow.Available, letmeknow.lid, letmeknow.NotificationType, mobile, state },
                            statusCode = 200,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            data = new { mobile, state },
                            statusCode = 200,
                        }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 403,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getlemeknow", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد " + ex.Message,
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> AddLetmeKnow(int id, int productPriceId, bool? amazing, bool? available, short? notificationType)
        {
            //UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            //userActivitiesObj.SaveUserActivity(null, id);

            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            var uid = Authentication.ValidateToken(Token);
            try
            {
                if (uid != null)
                {

                    await uow.ProductLetmeknowRepository.addLetmeKnow(id, productPriceId, uid, amazing.Value, available.Value, notificationType.Value);
                    return Json(new
                    {
                        statusCode = 200,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 403,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addletmeknow", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد " + ex.Message,
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);

            }
        }

        #endregion
        #endregion

        #region product category

        [HttpGet]
        public JsonResult GetProductCategory(int? id, string title, bool mobile)
        {
            if (!id.HasValue)
            {
                return Json(new
                {
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }

            if (!uow.ProductCategoryRepository.Any(x => x.Id, x => x.Id == id.Value))
            {
                return Json(new
                {
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }
            var date = DateTime.Now.Date;
            try
            {
                var pr = uow.ProductCategoryRepository.GetQueryList().Include("attachment").Include("ChildCategory.attachment").AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, x.ParrentId, x.Title, x.Name, x.Abstract, PageAddress = x.PageAddress.ToLower(), x.Sort, ChildCategory = x.ChildCategory.Select(s => new { s.Sort, s.hidden, s.Cover, attachment = s.attachment != null ? s.attachment.FileName : "", s.Id, s.Title, s.Name, s.Abstract, PageAddress = s.PageAddress.ToLower(), ChildCategory = s.ChildCategory.Select(s2 => new { s2.Sort, s2.hidden, s2.Cover, attachment = s2.attachment != null ? s2.attachment.FileName : "", s2.Id, s2.Title, s2.Name, s2.Abstract, PageAddress = s2.PageAddress.ToLower() }) }) }).SingleOrDefault();
                // title = title.Replace("-", " ").ToLower();
                if (pr.ParrentId != null)
                {
                    return Json(new
                    {
                        url = string.Format(string.Format("/tfs/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower()))),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (pr == null)
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrEmpty(title))
                {
                    return Json(new
                    {
                        url = string.Format("/tfc/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }

                string incomingTitle = Server.UrlDecode(title ?? "").ToLower();
                string normalizedDbTitle = CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower());
                if (incomingTitle != normalizedDbTitle)
                {
                    return Json(new
                    {
                        url = string.Format(string.Format("/tfc/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower()))),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<int> CatIds = uow.ProductCategoryRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", pr.Id)).ToList();
                    return Json(new
                    {
                        data = pr,
                        Breadcrumb = uow.ProductCategoryRepository.GerProductBreadcrumb(pr.Id),
                        TopAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 1 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 1 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                        RightAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 2 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 2 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                        BotomAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 3 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 3 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                        LeftAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 4 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 4 && x.TypeId == 4 && x.LinkId == pr.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                        Sliders = uow.SliderRepository.GetQueryList().Include("SliderImages.attachment").AsNoTracking().Where(x => x.IsActive && x.TypeId == 4 && x.LinkId == pr.Id).Select(x => new { x.Id, x.DisplaySort, SliderImages = x.SliderImages.Select(s => new { s.Id, image = s.attachment.FileName, title = s.Title, link = s.Link, s.ExpireDate, s.color, s.SliderTimerWidth, s.SliderTimerPosition, s.DisplaySort }) }),
                        ProductCategories = pr.ChildCategory.Where(x => x.hidden == false).Select(x => new { x.Id, x.Title, x.Name, x.Abstract, PageAddress = x.PageAddress.ToLower(), Cover = x.attachment, x.ChildCategory, x.Sort }).OrderBy(x => x.Sort),
                        NewProductItems = uow.ProductRepository.ProductItemList(x => x.ProductCategories.Any(s => s.hidden == false && CatIds.Contains(s.Id)) && x.IsActive && x.LanguageId == 1 && x.ProductPrices.Any(z => z.IsDefault) && x.state == 4 && (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 6), x => x.OrderByDescending(o => o.Id), 0, 15).ToList(),
                        PopularProductItems = uow.ProductRepository.ProductItemList(x => x.ProductCategories.Any(s => s.hidden == false && CatIds.Contains(s.Id)) && x.IsActive && x.LanguageId == 1 && x.ProductPrices.Any(z => z.IsDefault) && x.state == 4 && (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 6), x => x.OrderByDescending(o => o.FavCount), 0, 15).ToList(),
                        MostOrderProductItems = uow.ProductRepository.ProductItemList(x => x.ProductCategories.Any(s => s.hidden == false && CatIds.Contains(s.Id)) && x.IsActive && x.LanguageId == 1 && x.ProductPrices.Any(z => z.IsDefault) && x.state == 4 && (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 6), x => x.OrderByDescending(o => o.SellCount), 0, 15).ToList(),
                        ChildProductCategories = pr.ChildCategory.Where(x => x.hidden == false).Select(x => new { x.Sort, x.Id, title = x.Title, x.Name, link = "/tfs/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()), image = x.attachment }).OrderBy(x => x.Sort),
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductcategory", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion

        #region product search

        #region Compare

        public ActionResult compare(int? id1, int? id2, int? id3, int? id4)
        {
            //  UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            // userActivitiesObj.SaveUserActivity(null, id1);


            try
            {
                if (id1.HasValue)
                {
                    if (uow.ProductRepository.Any(x => x.Id, x => x.Id == id1.Value && x.IsActive && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4))
                    {
                        var cat = uow.ProductRepository.Get(x => new { x.ProductCategories.First().Id, x.ProductCategories.First().Name }, x => x.Id == id1.Value).Single();

                        var allCatIds = context.Database.SqlQuery<int>("exec GetParentCats @CatId", new SqlParameter("@CatId", cat.Id)).ToList();
                        return Json(new
                        {
                            cat = cat.Name,
                            catId = cat.Id,
                            groups = uow.ProductAttributeGroupProductCategorysRepository
.Get(x => x,
     x => allCatIds.Contains(x.ProductCategoryId),
     null,
     "ProductAttributeGroup.ProductAttributeGroupSelects.ProductAttribute.ProductAttributeItems")

.GroupBy(x => x.ProductAttributeGroupId)
.Select(g => g.First())

.Select(x => new
{
    ProductAttributeGroupSelects =
        x.ProductAttributeGroup.ProductAttributeGroupSelects
        .Select(a => new
        {
            grouptitle = x.ProductAttributeGroup.Title,
            attname = a.ProductAttribute.Title != "" ? a.ProductAttribute.Title : a.ProductAttribute.Name,
            attid = a.ProductAttribute.Id,
            items = a.ProductAttribute.ProductAttributeItems.Select(b => new { b.Id, b.AttributeId, b.Value }),
            a.DisplayGroupOrder,
            a.DisplayOrder,
            a.ProductAttribute.Unit,
            a.ProductAttribute.DataType
        })
        .OrderBy(a => a.DisplayGroupOrder)
        .ThenBy(a => a.DisplayOrder)
}),

                            data = uow.ProductRepository.ProductItemListComparev1(x => x.Id == id1.Value || x.Id == id2.Value || x.Id == id3.Value || x.Id == id4.Value),
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            url = "/",
                            statusCode = 404
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "compare", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    url = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult RemovePr(int id, List<int> ids)
        {
            try
            {
                string link = "https://www.tfshops.com/Compare/";
                foreach (var item in ids)
                {
                    if (item != id)
                        link += item + "/";
                }
                return Json(new
                {
                    link = link,
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "removepr", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetProducts(int id, List<int> ids, int? brandId, string keyword)
        {
            CompareCat compareCat = new CompareCat();

            try
            {
                if (keyword != null)
                    keyword = keyword.Trim();
                ViewBag.brandId = brandId;
                ViewBag.keyword = keyword;
                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", id)).ToList();

                compareCat.brands = uow.BrandRepository.Get(x => x, x => x.Products.Any(s => s.IsActive && s.state == 4 && s.ProductPrices.Any(b => b.IsDefault && b.IsActive) && s.ProductCategories.Any(a => CatIds.Contains(a.Id))));

                var ProductPrices = uow.ProductPriceRepository.GetQueryList().AsNoTracking();

                if (!String.IsNullOrEmpty(keyword))
                {
                    var color = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Color.Contains(keyword)).FirstOrDefault();
                    string colorId = "0";
                    if (color != null)
                        colorId = color.Id.ToString();
                    var Garanty = uow.ProductAttributeItemRepository.Get(x => x, x => x.Value.Contains(keyword)).FirstOrDefault();
                    string GarantyId = "0";
                    if (Garanty != null)
                        GarantyId = Garanty.Id.ToString();
                    ProductPrices = from s in ProductPrices
                                    where
                                    (s.Product.LatinName != null && s.Product.LatinName.ToLower().Contains(keyword)) ||
                                    (s.Product.Name != null && s.Product.Name.ToLower().Contains(keyword)) ||
                                    (s.Product.Code != null && s.Product.Code.ToLower().Contains(keyword)) ||
                                    (s.code != null && s.code.ToLower().Contains(keyword)) ||
                                    (s.ProductAttributeSelectModelId != null && s.ProductAttributeSelectModel.Value.ToLower().Contains(keyword)) ||
                                    (s.ProductAttributeSelectSizeId != null && s.ProductAttributeSelectSize.Value.ToLower().Contains(keyword)) ||
                                    (s.ProductAttributeSelectWeightId != null && s.ProductAttributeSelectweight.Value.ToLower().Contains(keyword)) ||
                                    (s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor.Value.ToLower() == colorId) ||
                                    (s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId)
                                    select s;
                }

                if (brandId.HasValue)
                    ProductPrices = ProductPrices.Where(s => s.Product.BrandId == brandId.Value);

                List<int> pids = ProductPrices.Where(s => s.Product.ProductCategories.Any(x => CatIds.Contains(x.Id))).Select(x => x.ProductId).ToList();

                compareCat.productItems = uow.ProductRepository.ProductItemList(x => pids.Contains(x.Id) && !ids.Contains(x.Id));
                return Json(new
                {
                    AddedCompare = compareCat,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproducts", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult AddPr(int id, List<int> ids)
        {
            try
            {
                string link = "https://www.tfshops.com/Compare/";
                foreach (var item in ids)
                {
                    link += item + "/";
                }
                link += id + "/";
                return Json(new
                {
                    link = link,
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addpr", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        [HttpGet]
        public JsonResult GetProductSearchBreadCrumbs(int id)
        {

            try
            {
                var prcat = context.ProductCategories.Find(id);
                if (prcat == null)
                {
                    return Json(new
                    {
                        msg = "no product cat exist!",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var tfs2 = new { id = prcat.Id, Name = prcat.Name, pageaddress = string.Format("/tfs/{0}/{1}", prcat.Id, CommonFunctions.NormalizeAddress(prcat.PageAddress.ToLower())) };
                    if (prcat.ParrentId == null)
                    {
                        return Json(new
                        {
                            tfs = tfs2,
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (prcat.ParentCat.ParentCat != null)
                        {
                            var tfs = new { id = prcat.ParentCat.Id, Name = prcat.ParentCat.Name, pageaddress = string.Format("/tfs/{0}/{1}", prcat.ParentCat.Id, CommonFunctions.NormalizeAddress(prcat.ParentCat.PageAddress.ToLower())) };
                            var tfc = new { id = prcat.ParentCat.ParentCat.Id, Name = prcat.ParentCat.ParentCat.Name, pageaddress = string.Format("/tfc/{0}/{1}", prcat.ParentCat.ParentCat.Id, CommonFunctions.NormalizeAddress(prcat.ParentCat.ParentCat.PageAddress.ToLower())) };
                            return Json(new
                            {
                                tfs2 = tfs2,
                                tfs = tfs,
                                tfc = tfc,
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var tfc = new { id = prcat.ParentCat.Id, Name = prcat.ParentCat.Name, pageaddress = string.Format("/tfc/{0}/{1}", prcat.ParentCat.Id, CommonFunctions.NormalizeAddress(prcat.ParentCat.PageAddress.ToLower())) };
                            return Json(new
                            {
                                tfs2 = tfs2,
                                tfc = tfc,
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductsearchbreadcrumbs", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetProductSearchh(int id)
        {
            try
            {
                var pr = context.ProductCategories.AsQueryable().AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, Title = x.Title2, x.Name, x.Abstract, x.Data }).SingleOrDefault();
                if (pr == null)
                {
                    return Json(new
                    {
                        msg = "no product cat exist!",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        data = pr,
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductsearchh", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpGet]
        public ActionResult GetProductSearch(int? id, string title, string q, int? sort, int? page, int? perpage, bool mobile)
        {
            if (!id.HasValue)
            {
                return Json(new
                {
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }

            if (!uow.ProductCategoryRepository.Any(x => x.Id, x => x.Id == id.Value))
            {
                return Json(new
                {
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }
            if (uow.ProductCategoryRepository.Any(x => x.Id, x => x.Id == id.Value && x.ParrentId == null))
            {
                return Json(new
                {
                    url = string.Format("/tfc/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(uow.ProductCategoryRepository.GetByID(id.Value).PageAddress.ToLower())),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (uow.RedirectUrlRepository.Any(x => x.Id, x => x.SourceTypeId == 5 && x.SourceLinkId == id.Value))
                {
                    return Json(new
                    {
                        url = uow.RedirectUrlRepository.Get(x => x.DestinationUrl, x => x.SourceTypeId == 5 && x.SourceLinkId == id.Value).First(),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }

                #region اعتبار سنجی اولیه
                var url = Request.RawUrl.Split('?');
                var totlaFilters = getUrlParameter(Server.UrlDecode(Request.RawUrl));
                totlaFilters.pagenum = page == null ? 1 : page.Value;
                // totlaFilters.prevpage = prevpage == null ? 1 : prevpage.Value;
                totlaFilters.perpage = perpage == null ? 20 : perpage.Value;
                totlaFilters.UrlPathname = Request.Url.AbsolutePath;

                totlaFilters.sortby = sort == null ? 4 : sort.Value;
                totlaFilters.SearchStr = q;


                var prcat = uow.ProductCategoryRepository.GetQueryList().AsNoTracking().Include("productCategoryFAQs").Include("ChildCategory").Include("ParentCat").Select(c => new
                {
                    Id = c.Id,
                    parentCat = c.ParentCat,
                    Name = c.Name,
                    PageAddress2 = c.PageAddress2.ToLower(),
                    Descr2 = c.Descr2,
                    Title2 = c.Title2,
                    attachmentFileName = c.attachment.FileName,
                    CoverHasValue = c.Cover.HasValue,
                    Data = c.Data,
                    productCategoryFAQs = c.productCategoryFAQs,
                    childcats = c.ChildCategory.Where(x => x.hidden == false)
                }).FirstOrDefault(x => x.Id == id.Value);


                if (string.IsNullOrEmpty(title))
                {
                    return Json(new
                    {
                        url = string.Format("/tfs/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(prcat.PageAddress2.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }

                string incomingTitle = Server.UrlDecode(title ?? "").ToLower();
                string normalizedDbTitle = CommonFunctions.NormalizeAddress(prcat.PageAddress2.ToLower());
                //string normalizedIncomingTitle = CommonFunctions.NormalizeAddress(incomingTitle);

                var ChildCats = prcat.childcats.Select(x => new { x.Sort, x.Id, title = x.Title, x.Name, link = CommonFunctions.NormalizeAddress(x.PageAddress2.ToLower()), image = x.Cover.HasValue ? x.attachment.FileName : "default-thumbnail.jpg" }).ToList();
                var productGoogleFaq = prcat.productCategoryFAQs.Select(x => new ProductFAQGoogleList
                {
                    @type = "Question",
                    name = x.Question,
                    acceptedAnswer = new acceptedAnswer { type = "Answer", text = x.Answer }

                });
                var productCategoryFAQs = prcat.productCategoryFAQs.AsEnumerable();

                if (prcat == null)
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (normalizedDbTitle != incomingTitle)
                {
                    return Json(new
                    {
                        url = string.Format("/tfs/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(prcat.PageAddress2.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (!String.IsNullOrEmpty(Request.QueryString.ToString().Split('&').ToString()))
                {
                    var qs = Request.QueryString.ToString().Split('&');
                    if (qs.Where(x => x.StartsWith("page")).Count() > 1 || qs.Where(x => x.StartsWith("sort")).Count() > 1)
                    {
                        return Json(new
                        {
                            url = string.Format("/tfs/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(prcat.PageAddress2.ToLower())),
                            statusCode = 301
                        }, JsonRequestBehavior.AllowGet);
                    }
                }


                #endregion


                //  لیست تمام دسته بندی هایی  که سرچ در انها انجام می شود
                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", prcat.Id)).ToList();

                if (!uow.ProductRepository.Any(x => x.Id, x => x.ProductCategories.Any(s => s.hidden == false && CatIds.Contains(s.Id))))
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }

                /// لیستی از تمام اتریبویت  های قابل سرچ  
                var productCategoryAttribute = uow.ProductCategoryAttributeRepository.GetQueryList().AsNoTracking().Include(c => c.ProductAttribute).Where(c => c.ProductCategoryId == prcat.Id).Select(c => c.ProductAttribute);
                var baseQuery1 = productCategoryAttribute;
                //آیدی اتریبیوت های قابل سرچ
                List<int> attIds = baseQuery1.Select(x => x.Id).ToList();
                //آیدی اتریبیوت های مدل،سایز،رنگ،گارانتی
                attIds.AddRange(uow.ProductAttributeGroupSelectRepository.Get(x => x.AttributeId, x => x.ProductAttributeGroup.Primary).ToList());
                //لیست مقادیر خصوصیت گروه فعلی
                var baseQuery = uow.ProductAttributeSelectRepository.GetQueryList().Include(c => c.Product).Where(x => attIds.Contains(x.ProductAttributeGroupSelect.AttributeId) && x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id)));
                //فیلتر دسته بندی محصول

                var ProductAttributeSelects = baseQuery;
                var count = ProductAttributeSelects.Count();

                var grantiIds = ProductAttributeSelects.Where(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15).Select(c => c.Value).Distinct().ToList();
                var colorIds = ProductAttributeSelects.Where(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12).Select(c => c.Value).Distinct().ToList();

                // ویو بگ های سرچ کناری 
                var PACQuerry = baseQuery.Include(c => c.ProductAttributeGroupSelect.ProductAttribute);

                List<FilterShowViewModel> filterShowViewModels = new List<FilterShowViewModel>();
                #region لیست تمام گارانتی 
                List<FilterShowViewModel> GarantyList = new List<FilterShowViewModel>();
                var garantyList1 = uow.ProductAttributeRepository.Get(x => new { Id = x.Id, Name = x.Name, productattributeItems = x.ProductAttributeItems }, x => x.DataType == 15, null, "productattributeItems").Select(c => new FilterShowViewModel
                {
                    type = "attribute",
                    attrId = c.Id,
                    title = c.Name,
                    items = c.productattributeItems.Where(s => grantiIds.Contains(s.Id.ToString())).Select(s => new FilterShowItems
                    {
                        id = s.Id,
                        name = s.Value,
                        checkedd = totlaFilters.AttributeViewModels != null && totlaFilters.AttributeViewModels.Any(a => a.Key1 == c.Id && int.Parse(a.Value) == s.Id)
                    }).ToList()
                }).ToList();
                if (uow.ProductCategoryAttributeRepository.Any(x => x.Id, x => x.ProductCategoryId == prcat.Id && x.ProductAttribute.DataType == 15))
                {
                    var garantyList = new List<FilterShowViewModel>();
                    garantyList1.ForEach(c =>
                    {
                        if (!garantyList.Any(s => s.attrId == c.attrId))
                            garantyList.Add(c);
                    });
                    GarantyList = garantyList;
                    filterShowViewModels.AddRange(GarantyList);
                }
                #endregion

                #region رنگ
                List<FilterShowViewModel> ColorList = new List<FilterShowViewModel>();
                if (uow.ProductCategoryAttributeRepository.Any(x => x.Id, x => x.ProductCategoryId == prcat.Id && x.ProductAttribute.DataType == 12))
                {

                    // لیست تمام رنگ ها 
                    var colorList1 = uow.ProductAttributeRepository.Get(x => new { Id = x.Id, Name = x.Name, ProductAttributeItemColors = x.ProductAttributeItemColors }, x => x.DataType == 12, null, "ProductAttributeItemColors").Select(c => new FilterShowViewModel
                    {
                        type = "color",
                        attrId = c.Id,
                        title = c.Name,
                        items = c.ProductAttributeItemColors.Where(s => colorIds.Contains(s.Id.ToString())).Select(s => new FilterShowItems
                        {
                            id = s.Id,
                            name = s.Color,
                            ColorCode = s.Value
                        }).ToList()
                    }).ToList();
                    var colorList = new List<FilterShowViewModel>();
                    colorList1.ForEach(c =>
                    {
                        if (!colorList.Any(s => s.attrId == c.attrId))
                            colorList.Add(c);
                    });
                    colorList.ForEach(x =>
                    {
                        if (x?.items != null)
                            x.items.ForEach(c =>
                            {
                                if (totlaFilters.AttributeViewModels != null && totlaFilters.AttributeViewModels.Any(s => s.Key1 == x.attrId && int.Parse(s.Value) == c.id))
                                    c.checkedd = true;
                            });
                    });
                    ColorList = colorList;
                    filterShowViewModels.AddRange(ColorList);


                }
                #endregion

                #region سایز
                List<FilterShowViewModel> SizeList = new List<FilterShowViewModel>();
                if (uow.ProductCategoryAttributeRepository.Any(x => x.Id, x => x.ProductCategoryId == prcat.Id && x.ProductAttribute.DataType == 13))
                {

                    // لیست تمام سایز ها 
                    var sizeList1 = uow.ProductAttributeRepository.Get(x => new { Id = x.Id, Name = x.Name }, x => x.DataType == 13).Select(c => new FilterShowViewModel
                    {
                        type = "size",
                        attrId = c.Id,
                        title = c.Name,
                        items = PACQuerry.Where(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13).Select(s => s.Value).Distinct().Select(s => new FilterShowItems
                        {
                            id = 1,
                            name = s
                        }).ToList()
                    }).ToList();
                    var sizeList = new List<FilterShowViewModel>();
                    sizeList1.ForEach(c =>
                    {
                        if (!sizeList.Any(s => s.attrId == c.attrId))
                            sizeList.Add(c);
                    });
                    sizeList.ForEach(x =>
                    {
                        if (x?.items != null)
                            x.items.ForEach(c =>
                            {
                                if (totlaFilters.AttributeViewModels != null && totlaFilters.AttributeViewModels.Any(s => s.Value == c.name))
                                    c.checkedd = true;
                            });
                    });
                    SizeList = sizeList;
                    filterShowViewModels.AddRange(SizeList);
                }
                #endregion

                #region لیست تمام برندها
                var bands = uow.ProductRepository.GetQueryList().AsNoTracking().Where(c => c.ProductCategories.Any(s => s.hidden == false && CatIds.Contains(s.Id)) && c.BrandId.HasValue)
                    .Select(s => new FilterShowItems
                    {
                        id = s.BrandId.Value,
                        name = s.Brand.PersianName,
                        enname = s.Brand.Name
                        //PersianName = s.Brand.PersianName
                    }).Distinct().ToList();
                if (bands != null)
                    bands.ForEach(c =>
                    {
                        if (totlaFilters.BrandViewModels != null && totlaFilters.BrandViewModels.Any(s => s.Value == c.id))
                            c.checkedd = true;

                    });
                var BrandList = new FilterShowViewModel() { title = "برند", type = "brand", items = bands };
                filterShowViewModels.Add(BrandList);
                #endregion

                #region اتریبیوت های منطقی
                var boolList1 = baseQuery1.Where(c => c.DataType == 5).Select(c => new FilterShowItems
                {
                    id = c.Id,
                    name = c.Name,

                }).ToList();

                var boolList = new List<FilterShowItems>();
                boolList1.ForEach(c =>
                {
                    if (!boolList.Any(s => s.id == c.id))
                        boolList.Add(c);
                });

                boolList.ForEach(x =>
                {
                    if (totlaFilters.LogicViewModels != null && totlaFilters.LogicViewModels.Any(s => s.Key == x.id))
                        x.checkedd = true;

                });
                filterShowViewModels.Add(new FilterShowViewModel() { title = "ویژگی‌ها", type = "bool", items = boolList });
                #endregion

                #region بقیه لیست ها

                List<FilterShowViewModel> othetList = new List<FilterShowViewModel>();
                var othetList1 = baseQuery1.Where(c => c.DataType == 8).Select(c => new FilterShowViewModel
                {
                    type = "attribute",
                    attrId = c.Id,
                    title = !String.IsNullOrEmpty(c.Title) ? c.Title : c.Name,
                    items = c.ProductAttributeItems.Select(s => new FilterShowItems
                    {
                        id = s.Id,
                        name = s.Value,
                    }).ToList()
                }).ToList();
                othetList1.ForEach(c =>
                {
                    if (!othetList.Any(s => s.attrId == c.attrId))
                        othetList.Add(c);
                });

                othetList.ForEach(x =>
                {
                    if (x?.items != null)
                        x.items.ForEach(c =>
                        {
                            if (totlaFilters.DbListViewModels != null && totlaFilters.DbListViewModels.Any(s => s.Key1 == x.attrId && int.Parse(s.Value) == c.id))
                                c.checkedd = true;
                        });
                });

                filterShowViewModels.AddRange(othetList);
                #endregion

                #region قیمت
                GetLongRange PriceRange = uow.ProductRepository.GetPriceRaneg(prcat.Id);
                filterShowViewModels.Add(new FilterShowViewModel() { type = "range", title = "محدوده قیمت مورد نظر", min = PriceRange.R1, max = PriceRange.R2 });
                #endregion

                List<Domain.ViewModel.SearchResultSort> SearchResultSorts = new List<Domain.ViewModel.SearchResultSort>();
                List<ProductItem> sortedProductItems = new List<ProductItem>();

                List<int> catIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", id)).ToList();
                IQueryable<ProductSearchResult> productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters, CatIds, null, count > 0 ? false : true);
                var totalCount = productSearchResults.Count();
                // pageSetting

                ViewBag.TotalCount = totalCount;
                int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
                if (skip < 0)
                    skip = 0;
                if (totlaFilters.perpage == 0)
                {
                    totlaFilters.perpage = 1;
                }
                var paggingModel = new PagingViewModel
                {
                    Count = totalCount,
                    PrevPage = totlaFilters.prevpage,
                    CurentPage = totlaFilters.pagenum,
                    PerPage = totlaFilters.perpage,
                    RawUrl = Request.RawUrl
                };

                productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
                ViewBag.Id = id.Value;
                SearchResultSorts = productSearchResults.ToList().Select(c => new Domain.ViewModel.SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
                List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
                var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id), null, 0, 0, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84").ToList();
                foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
                {
                    sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
                }

                var Products = sortedProductItems;

                var date = DateTime.Now.Date;

                return Json(new
                {
                    parentCat = prcat.parentCat != null ? "/tfs/" + prcat.parentCat.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcat.parentCat.PageAddress2.ToLower()) : null,
                    filters = filterShowViewModels,
                    q = q,
                    ChildCats = ChildCats,
                    ProductGoogleFaq = productGoogleFaq,
                    FAQs = productCategoryFAQs,
                    pagging = paggingModel,
                    products = Products,
                    data = new { Name = prcat.Name, Title = prcat.Title2, attachmentFileName = prcat.attachmentFileName, Data = prcat.Data },
                    TopAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 1 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 1 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    RightAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 2 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 2 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    BotomAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 3 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 3 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    LeftAdverestings = mobile ? uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 4 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Include("attachment2").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.Cover2.HasValue ? x.attachment2.FileName : x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList() : uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 4 && x.TypeId == 6 && x.LinkId == prcat.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").Select(x => new Domain.ViewModels.adverestingShow { AdverestingSizeId = x.AdverestingSizeId, attachmentFileName = x.attachment.FileName, Id = x.Id, Link = x.Link, Position = x.Position, Title = x.Title, TypeLink = x.TypeLink }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductsearch", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        private getUrlParameter getUrlParameter(string rawUrl)
        {
            var model = new getUrlParameter();
            var urlParams = rawUrl.Split('?');
            if (urlParams.Count() > 0)
            {
                if (urlParams.Count() == 2)
                {
                    var dataUrl = urlParams[1];
                    var dataUrlArray = dataUrl.Split('&');
                    int id = getUrl(dataUrlArray, "id");
                    List<AttributeViewModel> attributes = getAttributes(dataUrlArray);
                    model.AttributeViewModels = attributes;
                    List<LogicViewModel> logicList = getLogicList(dataUrlArray);
                    model.LogicViewModels = logicList;
                    var dbList = getDbList(dataUrlArray);
                    model.DbListViewModels = dbList;
                    List<BrandViewModel> brands = getBrands(dataUrlArray);
                    model.BrandViewModels = brands;
                    List<SellerConditionViewModel> sellers = getSellers(dataUrlArray);
                    model.SellerConditionViewModels = sellers;
                    bool onlyExit = idOnlyExit(dataUrlArray);
                    model.IsExit = onlyExit;
                    bool onlyInStore = isOnlyInStore(dataUrlArray);
                    model.OnlyInStore = onlyInStore;
                    PriceRange priceRange = getPriceRange(dataUrlArray);
                    model.PriceRange = priceRange;

                }
            }
            return model;
        }
        private List<LogicViewModel> getLogicList(string[] dataUrlArray)
        {
            var list = new List<LogicViewModel>();
            foreach (var item in dataUrlArray)
            {
                var dataSplit = item.Split('=');
                if (dataSplit.Count() == 2)
                {
                    if (dataSplit[0].ToLower().Contains("check"))
                    {
                        var data = new LogicViewModel();
                        data.Key = getcheckkty(dataSplit[0]);
                        data.Value = "true";
                        list.Add(data);
                    }
                }
            }
            return list;
        }

        private int getcheckkty(string v)
        {
            v = v.Replace("check[", "").Replace("][", ",").Replace("[", "").Replace("]", "");


            return int.Parse(v);
        }

        private PriceRange getPriceRange(string[] dataUrlArray)
        {
            long price0 = 0;
            long price1 = 0;
            foreach (var item in dataUrlArray)
            {
                if (item.ToLower().Contains("price[0]="))
                {
                    var reu = item.Replace("price[0]=", "");
                    price0 = long.Parse(reu);
                }
                if (item.ToLower().Contains("price[1]="))
                {
                    var reu = item.Replace("price[1]=", "");
                    price1 = long.Parse(reu);
                }
            }
            if (price0 != 0 && price1 != 0)
            {
                return new PriceRange
                {
                    StartPrice = price0,
                    EndPrice = price1
                };
            }
            return null;
        }

        private bool isOnlyInStore(string[] dataUrlArray)
        {
            foreach (var item in dataUrlArray)
            {
                if (item.ToLower().Contains("nstore=1"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool idOnlyExit(string[] dataUrlArray)
        {
            //has_selling_stock=1
            foreach (var item in dataUrlArray)
            {
                if (item.ToLower().Contains("has_selling_stock=1"))
                {
                    return true;
                }
            }
            return false;
        }

        private List<SellerConditionViewModel> getSellers(string[] dataUrlArray)
        {
            var list = new List<SellerConditionViewModel>();
            foreach (var item in dataUrlArray)
            {
                var dataSplit = item.Split('=');
                if (dataSplit.Count() == 2)
                {
                    if (dataSplit[0].ToLower().Contains("seller_condition"))
                    {
                        var data = new SellerConditionViewModel();
                        var value = int.Parse(dataSplit[1]);
                        data.Value = value;
                        data = getSeller(data, dataSplit[0]);
                        list.Add(data);
                    }
                }
            }
            return list;
        }

        private SellerConditionViewModel getSeller(SellerConditionViewModel data, string v)
        {
            v = v.Replace("seller_condition", "");
            var reu = v.Replace("][", ",").Replace("[", "").Replace("]", "").Split(',');
            if (reu.Count() > 0)
            {
                data.Key = int.Parse(reu[0]);
            }

            return data;
        }

        private List<BrandViewModel> getBrands(string[] dataUrlArray)
        {
            var list = new List<BrandViewModel>();
            foreach (var item in dataUrlArray)
            {
                var dataSplit = item.Split('=');
                if (dataSplit.Count() == 2)
                {
                    if (dataSplit[0].ToLower().Contains("brand"))
                    {
                        var data = new BrandViewModel();
                        var value = int.Parse(dataSplit[1]);
                        data.Value = value;
                        data = getBrand(data, dataSplit[0]);
                        list.Add(data);
                    }
                }
            }
            return list;
        }

        private BrandViewModel getBrand(BrandViewModel data, string v)
        {
            v = v.Replace("brand", "");
            var reu = v.Replace("][", ",").Replace("[", "").Replace("]", "").Split(',');
            if (reu.Count() > 0)
            {
                data.Key = int.Parse(reu[0]);
            }

            return data;
        }

        private List<AttributeViewModel> getAttributes(string[] dataUrlArray)
        {
            var list = new List<AttributeViewModel>();
            foreach (var item in dataUrlArray)
            {
                var dataSplit = item.Split('=');
                if (dataSplit.Count() == 2)
                {
                    if (dataSplit[0].ToLower().Contains("attribute"))
                    {
                        var data = new AttributeViewModel();
                        data.Value = dataSplit[1];
                        AttributeViewModel keys = getKeys(data, dataSplit[0]);
                        list.Add(keys);
                    }
                }
            }
            return list;
        }
        private List<AttributeViewModel> getDbList(string[] dataUrlArray)
        {
            var list = new List<AttributeViewModel>();
            foreach (var item in dataUrlArray)
            {
                var dataSplit = item.Split('=');
                if (dataSplit.Count() == 2)
                {
                    if (dataSplit[0].ToLower().Contains("dblist"))
                    {
                        var data = new AttributeViewModel();
                        data.Value = dataSplit[1];
                        AttributeViewModel keys = getKeys(data, dataSplit[0]);
                        list.Add(keys);
                    }
                }
            }
            return list;
        }

        private AttributeViewModel getKeys(AttributeViewModel data, string v)
        {
            v = v.Replace("attribute", "").Replace("dblist", "");
            var reu = v.Replace("][", ",").Replace("[", "").Replace("]", "").Split(',');
            if (reu.Count() > 0)
            {
                if (reu.Count() == 1)
                    data.Key1 = int.Parse(reu[0]);
                else
                {
                    data.Key1 = int.Parse(reu[0]);
                    data.Key2 = int.Parse(reu[1]);
                }
            }

            return data;
        }

        private int getUrl(string[] dataUrlArray, string param)
        {

            foreach (var item in dataUrlArray)
            {
                var dataSplit = item.Split('=');
                if (dataSplit.Count() == 2)
                {
                    if (dataSplit[0].ToLower() == param)
                    {
                        return int.Parse(dataSplit[1]);
                    }
                }
            }
            return (int)0;
        }
        #endregion

        #region tag

        [HttpGet]
        public JsonResult GetTag(int? id, string title, int? page, int? perpage, int? sort, int? brand, string keyword, string has_selling_stock, string min_price, string max_price)
        {
            if (!id.HasValue)
            {
                return Json(new
                {
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }

            if (uow.RedirectUrlRepository.Any(x => x.Id, x => x.SourceTypeId == 3 && x.SourceLinkId == id.Value))
            {
                return Json(new
                {
                    url = uow.RedirectUrlRepository.Get(x => x.DestinationUrl, x => x.SourceTypeId == 6 && x.SourceLinkId == id.Value).First(),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var tag = context.Tags.Include(x => x.TagFAQs).AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, Title = x.TagName, x.Data, x.Today, x.TagFAQs }).SingleOrDefault();
                //title = title.Replace("-", " ").ToLower();
                // چنین تگ محصولی وجود دارد؟
                if (tag == null)
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrEmpty(title))
                {
                    return Json(new
                    {
                        url = string.Format("/ptag/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(tag.Title.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (!uow.ProductRepository.Any(s => s.Tags.Any(a => a.TagName == title)))
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }

                string incomingTitle = Server.UrlDecode(title ?? "").ToLower();
                string normalizedDbTitle = CommonFunctions.NormalizeAddress(tag.Title.ToLower());
                if (incomingTitle != normalizedDbTitle)
                {
                    return Json(new
                    {
                        url = string.Format("/ptag/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(tag.Title.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }

                else if (!String.IsNullOrEmpty(Request.QueryString.ToString().Split('&').ToString()))
                {
                    var qs = Request.QueryString.ToString().Split('&');
                    if (qs.Where(x => x.StartsWith("page")).Count() > 1 || qs.Where(x => x.StartsWith("sort")).Count() > 1 || (!Request.RawUrl.Contains("?") && Request.RawUrl.Contains("&")))
                    {
                        return Json(new
                        {
                            url = string.Format("/ptag/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(tag.Title.ToLower())),
                            statusCode = 301
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                string todayDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(DateTime.Now.Date);
                string metaTitle = (!String.IsNullOrEmpty(tag.Title) ? tag.Title : tag.Title) + (tag.Today ? " " + todayDate : "");

                ProductTagViewModel productTagViewModel = new ProductTagViewModel()
                {
                    Name = tag.Title + (tag.Today ? " " + todayDate : ""),
                    Id = tag.Id,
                    Data = tag.Data,
                    TagFAQs = tag.TagFAQs
                };
                var productGoogleFaq = productTagViewModel.TagFAQs.Select(x => new ProductFAQGoogleList
                {
                    @type = "Question",
                    name = x.Question,
                    acceptedAnswer = new acceptedAnswer { type = "Answer", text = x.Answer }

                });

                #region Products



                var totlaFilters = new getUrlParameter();
                totlaFilters.pagenum = page == null ? 1 : page.Value;
                totlaFilters.perpage = perpage == null ? 15 : perpage.Value;
                totlaFilters.sortby = sort.HasValue ? sort.Value : 4;

                totlaFilters.productTagId = id.Value;

                var brands = uow.ProductRepository.GetQueryList().AsNoTracking().Where(c => c.Tags.Any(s => s.Id == id))
                 .Select(s => new BrandShowViewModel
                 {
                     Id = s.BrandId.Value,
                     PersianName = s.Brand.PersianName
                 }).Distinct().ToList();

                if (brand.HasValue)
                {
                    totlaFilters.BrandViewModels = new List<BrandViewModel>();
                    totlaFilters.BrandViewModels.Add(new BrandViewModel()
                    {
                        Key = brand.Value,
                        Value = brand.Value
                    });
                }

                ViewBag.search = keyword;
                if (!String.IsNullOrEmpty(keyword))
                    totlaFilters.SearchStr = keyword;
                if (!String.IsNullOrEmpty(has_selling_stock))
                    totlaFilters.IsExit = has_selling_stock == "1" ? true : false;
                ViewBag.has_selling_stock = has_selling_stock;


                GetLongRange PriceRange = uow.ProductRepository.GetPriceRanegGetPriceTag(id.Value);


                if (!String.IsNullOrEmpty(min_price) && !String.IsNullOrEmpty(max_price))
                {
                    totlaFilters.PriceRange = new PriceRange();
                    totlaFilters.PriceRange = new PriceRange() { StartPrice = Convert.ToInt64(min_price), EndPrice = Convert.ToInt64(max_price) };
                    ViewBag.CurenTPrice = totlaFilters.PriceRange;
                }

                List<SearchResultSort> SearchResultSorts = new List<SearchResultSort>();
                List<ProductItem> sortedProductItems = new List<ProductItem>();
                var productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters, null);
                // pageSetting
                var totalCount = productSearchResults.Count();
                ViewBag.TotalCount = totalCount;
                int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
                if (skip < 0)
                    skip = 0;
                if (totlaFilters.perpage == 0)
                {
                    totlaFilters.perpage = 1;
                }
                var paggingModel = new PagingViewModel
                {
                    Count = totalCount,
                    CurentPage = totlaFilters.pagenum,
                    PerPage = totlaFilters.perpage,
                    RawUrl = Request.RawUrl
                };

                productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
                SearchResultSorts = productSearchResults.ToList().Select(c => new SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
                List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
                var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id)).ToList();
                foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
                {
                    sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
                }
                productTagViewModel.Products = sortedProductItems;
                #endregion

                var cat = uow.ProductCategoryRepository.GetQueryList().AsNoTracking().Where(x => x.hidden == false && x.Products.Any(s => productids.Contains(s.Id))).Select(x => new { x.Id, PageAddress2 = x.PageAddress2.ToLower(), x.Name }).ToList();


                return Json(new
                {
                    productGoogleFaq = productGoogleFaq,
                    TagFAQs = productTagViewModel.TagFAQs,
                    cat = cat,
                    pagging = paggingModel,
                    PriceRange = PriceRange,
                    brands = brands,
                    products = productTagViewModel.Products,
                    title = metaTitle,
                    data = tag,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "gettag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult LoadMoreTag(int? id, int? page, int? sort, int? brand, string keyword, string has_selling_stock, string min_price, string max_price)
        {

            try
            {

                var tag = uow.TagRepository.Get(x => x, x => x.Id == id.Value).FirstOrDefault();
                var setting = GetSetting();

                ProductTagViewModel productTagViewModel = new ProductTagViewModel()
                {
                    Name = tag.TagName,
                    Id = tag.Id,
                    Data = tag.Data
                };

                #region Products

                var totlaFilters = new getUrlParameter();
                totlaFilters.pagenum = page == null ? 1 : page.Value;
                totlaFilters.perpage = 15;
                totlaFilters.sortby = sort.HasValue ? sort.Value : 1;
                totlaFilters.productTagId = id.Value;

                if (brand.HasValue)
                {
                    if (brand > 0)
                    {
                        totlaFilters.BrandViewModels = new List<BrandViewModel>();
                        totlaFilters.BrandViewModels.Add(new BrandViewModel()
                        {
                            Key = brand.Value,
                            Value = brand.Value
                        });
                    }
                }

                GetLongRange PriceRange = uow.ProductRepository.GetPriceRanegGetPriceTag(id.Value);

                if (!String.IsNullOrEmpty(min_price) && !String.IsNullOrEmpty(max_price))
                {
                    totlaFilters.PriceRange = new PriceRange();
                    totlaFilters.PriceRange = new PriceRange() { StartPrice = Convert.ToInt64(min_price), EndPrice = Convert.ToInt64(max_price) };
                }

                if (!String.IsNullOrEmpty(has_selling_stock))
                    totlaFilters.IsExit = has_selling_stock == "1" ? true : false;

                if (!String.IsNullOrEmpty(keyword))
                    totlaFilters.SearchStr = keyword;
                List<SearchResultSort> SearchResultSorts = new List<SearchResultSort>();
                List<ProductItem> sortedProductItems = new List<ProductItem>();
                var productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters, null);
                // pageSetting
                var totalCount = productSearchResults.Count();
                int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
                if (skip < 0)
                    skip = 0;
                if (totlaFilters.perpage == 0)
                {
                    totlaFilters.perpage = 1;
                }
                var paggingModel = new PagingViewModel
                {
                    Count = totalCount,
                    CurentPage = totlaFilters.pagenum,
                    PerPage = totlaFilters.perpage,
                    RawUrl = Request.RawUrl
                };

                productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
                SearchResultSorts = productSearchResults.ToList().Select(c => new SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
                List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
                var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id)).ToList();
                foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
                {
                    sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
                }
                productTagViewModel.Products = sortedProductItems;
                #endregion


                return Json(new
                {
                    PriceRange = PriceRange,
                    remain = totalCount,
                    NewdatRow = sortedProductItems,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "loadmoretag", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region brand


        [HttpGet]
        public JsonResult GetBrand(int? id, int? CatId, string title, int? page, int? perpage, int? sort, string keyword, string has_selling_stock, string min_price, string max_price)
        {
            if (!id.HasValue)
            {
                return Json(new
                {
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }


            if (uow.RedirectUrlRepository.Any(x => x.Id, x => x.SourceTypeId == 6 && x.SourceLinkId == id.Value))
            {
                return Json(new
                {
                    url = uow.RedirectUrlRepository.Get(x => x.DestinationUrl, x => x.SourceTypeId == 6 && x.SourceLinkId == id.Value).First(),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);
            }


            // title = title.Replace("-", " ").ToLower();
            try
            {
                var brand = context.Brands.Include(x => x.BrandFAQs).AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, x.TitleH1, x.MeteDescription, x.PersianName, x.AttachementId, x.attachment.FileName, x.Name, Title = x.Title, x.Data, x.BrandFAQs }).SingleOrDefault();
                if (brand == null)
                {
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }

                string incomingTitle = Server.UrlDecode(title ?? "").ToLower();
                string normalizedDbTitle = CommonFunctions.NormalizeAddress(brand.Name.ToLower());
                if (incomingTitle != normalizedDbTitle)
                {
                    return Json(new
                    {
                        url = string.Format("/tfb/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(brand.Name.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrEmpty(title))
                {
                    return Json(new
                    {
                        url = string.Format("/tfb/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(brand.Name.ToLower())),
                        statusCode = 301
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (!String.IsNullOrEmpty(Request.QueryString.ToString().Split('&').ToString()))
                {

                    var qs = Request.QueryString.ToString().Split('&');
                    if (qs.Where(x => x.StartsWith("page")).Count() > 1 || qs.Where(x => x.StartsWith("sort")).Count() > 1 || (!Request.RawUrl.Contains("?") && Request.RawUrl.Contains("&")))
                    {
                        return Json(new
                        {
                            url = string.Format("/tfb/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(brand.Name.ToLower())),
                            statusCode = 301
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                string prc = null;
                if (CatId.HasValue)
                    prc = uow.ProductCategoryRepository.Get(x => x.Name, x => x.Id == CatId.Value).Single();

                string metaTitle = brand.Title;
                string metaDescription = brand.MeteDescription;
                if (CatId.HasValue && !page.HasValue)
                {
                    metaTitle = " محصولات " + brand.Name + " در گروه " + prc;
                    metaDescription = metaDescription + " گروه " + prc;
                }
                else if (CatId.HasValue && page.HasValue)
                {
                    metaTitle = " محصولات " + brand.Name + " در گروه " + prc + " " + "صفحه " + page;
                    metaDescription = metaDescription + " گروه " + prc + " " + "صفحه " + page;
                }
                else if (!CatId.HasValue && page.HasValue)
                {
                    metaTitle = metaTitle + " " + " صفحه " + page;
                    metaDescription = metaDescription + " صفحه " + page;
                }
                BrandVM brandVM = new BrandVM()
                {
                    RawName = brand.Name,
                    Name = CatId.HasValue ? " محصولات " + brand.Name + " در گروه " + prc : brand.Name,
                    PersianName = CatId.HasValue ? " محصولات " + brand.PersianName + " در گروه " + prc : brand.PersianName,
                    Title = CatId.HasValue ? brand.Title + " ، " + prc : brand.Title,
                    Data = brand.Data,
                    Description = CatId.HasValue ? brand.MeteDescription + " ، " + prc : brand.MeteDescription,
                    Id = brand.Id,
                    Cover = brand.AttachementId.HasValue ? brand.FileName : "",
                    //Breadcrumb = uow.ProductRepository.GerProductBreadcrumbBrand(brand.Id, brand.Title, brand.Name, CatId),
                    BrandFAQs = brand.BrandFAQs,
                    TitleH1 = brand.TitleH1

                };
                var productGoogleFaq = brandVM.BrandFAQs.Select(x => new ProductFAQGoogleList
                {
                    @type = "Question",
                    name = x.Question,
                    acceptedAnswer = new acceptedAnswer { type = "Answer", text = x.Answer }

                });

                #region Products



                var totlaFilters = new getUrlParameter();
                totlaFilters.pagenum = page == null ? 1 : page.Value;
                totlaFilters.perpage = perpage == null ? 15 : perpage.Value;
                totlaFilters.sortby = sort.HasValue ? sort.Value : 4;

                totlaFilters.BrandViewModels = new List<BrandViewModel>();
                totlaFilters.BrandViewModels.Add(new BrandViewModel() { Key = id.Value, Value = id.Value });


                ViewBag.search = keyword;
                if (!String.IsNullOrEmpty(keyword))
                    totlaFilters.SearchStr = keyword;
                if (!String.IsNullOrEmpty(has_selling_stock))
                    totlaFilters.IsExit = has_selling_stock == "1" ? true : false;
                ViewBag.has_selling_stock = has_selling_stock;


                GetLongRange PriceRange = uow.ProductRepository.GetPriceRanegGetPriceBrand(id.Value);



                if (!String.IsNullOrEmpty(min_price) && !String.IsNullOrEmpty(max_price))
                {
                    totlaFilters.PriceRange = new PriceRange();
                    totlaFilters.PriceRange = new PriceRange() { StartPrice = Convert.ToInt64(min_price), EndPrice = Convert.ToInt64(max_price) };
                    ViewBag.CurenTPrice = totlaFilters.PriceRange;
                }


                List<int> CatIds = new List<int>();
                if (CatId.HasValue)
                    CatIds = uow.ProductCategoryRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", CatId)).ToList();

                List<SearchResultSort> SearchResultSorts = new List<SearchResultSort>();
                List<ProductItem> sortedProductItems = new List<ProductItem>();
                var productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters, CatId.HasValue ? CatIds : null);
                // pageSetting
                var totalCount = productSearchResults.Count();
                ViewBag.TotalCount = totalCount;
                int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
                if (skip < 0)
                    skip = 0;
                if (totlaFilters.perpage == 0)
                {
                    totlaFilters.perpage = 1;
                }
                var paggingModel = new PagingViewModel
                {
                    Count = totalCount,
                    CurentPage = totlaFilters.pagenum,
                    PerPage = totlaFilters.perpage,
                    RawUrl = Request.RawUrl
                };

                productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
                SearchResultSorts = productSearchResults.ToList().Select(c => new SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
                List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
                var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id)).ToList();
                foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
                {
                    sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
                }
                brandVM.Products = sortedProductItems;
                #endregion

                var cat = uow.ProductCategoryRepository.GetQueryList().AsNoTracking().Where(x => x.hidden == false && x.Products.Any(s => s.BrandId == id) && x.IsActive && x.LanguageId == 1).Select(x => new { x.Id, PageAddress2 = x.PageAddress2.ToLower(), x.Name }).ToList();


                return Json(new
                {
                    productGoogleFaq = productGoogleFaq,
                    TagFAQs = brandVM.BrandFAQs,
                    cat = cat,
                    pagging = paggingModel,
                    PriceRange = PriceRange,
                    products = brandVM.Products,
                    title = metaTitle,
                    data = brand,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "brand", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    msg = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult LoadMoreBrand(int? id, int? CatId, int? page, int? sort, string keyword, string has_selling_stock, string min_price, string max_price)
        {

            try
            {
                var brand = context.Brands.Include(x => x.BrandFAQs).AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, x.TitleH1, x.MeteDescription, x.PersianName, x.AttachementId, x.attachment.FileName, x.Name, Title = x.Title, x.Data, x.BrandFAQs }).SingleOrDefault();
                var setting = GetSetting();

                string prc = null;
                if (CatId.HasValue)
                    prc = uow.ProductCategoryRepository.Get(x => x.Name, x => x.Id == CatId.Value).Single();
                BrandVM brandVM = new BrandVM()
                {
                    RawName = brand.Name,
                    Name = CatId.HasValue ? " محصولات " + brand.Name + " در گروه " + prc : brand.Name,
                    PersianName = CatId.HasValue ? " محصولات " + brand.PersianName + " در گروه " + prc : brand.PersianName,
                    Title = CatId.HasValue ? brand.Title + " ، " + prc : brand.Title,
                    Data = brand.Data,
                    Description = CatId.HasValue ? brand.MeteDescription + " ، " + prc : brand.MeteDescription,
                    Id = brand.Id,
                    Cover = brand.AttachementId.HasValue ? brand.FileName : "",
                    //Breadcrumb = uow.ProductRepository.GerProductBreadcrumbBrand(brand.Id, brand.Title, brand.Name, CatId),
                    BrandFAQs = brand.BrandFAQs,
                    TitleH1 = brand.TitleH1

                };

                #region Products

                var totlaFilters = new getUrlParameter();
                totlaFilters.pagenum = page == null ? 1 : page.Value;
                totlaFilters.perpage = 15;
                totlaFilters.sortby = sort.HasValue ? sort.Value : 1;


                totlaFilters.BrandViewModels = new List<BrandViewModel>();
                totlaFilters.BrandViewModels.Add(new BrandViewModel() { Key = id.Value, Value = id.Value });



                GetLongRange PriceRange = uow.ProductRepository.GetPriceRanegGetPriceBrand(id.Value);



                if (!String.IsNullOrEmpty(min_price) && !String.IsNullOrEmpty(max_price))
                {
                    totlaFilters.PriceRange = new PriceRange();
                    totlaFilters.PriceRange = new PriceRange() { StartPrice = Convert.ToInt64(min_price), EndPrice = Convert.ToInt64(max_price) };
                    ViewBag.CurenTPrice = totlaFilters.PriceRange;
                }


                List<int> CatIds = new List<int>();
                if (CatId.HasValue)
                    CatIds = uow.ProductCategoryRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", CatId)).ToList();


                if (!String.IsNullOrEmpty(has_selling_stock))
                    totlaFilters.IsExit = has_selling_stock == "1" ? true : false;

                if (!String.IsNullOrEmpty(keyword))
                    totlaFilters.SearchStr = keyword;
                List<SearchResultSort> SearchResultSorts = new List<SearchResultSort>();
                List<ProductItem> sortedProductItems = new List<ProductItem>();
                var productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters, CatId.HasValue ? CatIds : null);
                // pageSetting
                var totalCount = productSearchResults.Count();
                int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
                if (skip < 0)
                    skip = 0;
                if (totlaFilters.perpage == 0)
                {
                    totlaFilters.perpage = 1;
                }
                var paggingModel = new PagingViewModel
                {
                    Count = totalCount,
                    CurentPage = totlaFilters.pagenum,
                    PerPage = totlaFilters.perpage,
                    RawUrl = Request.RawUrl
                };

                productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
                SearchResultSorts = productSearchResults.ToList().Select(c => new SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
                List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
                var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id)).ToList();
                foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
                {
                    sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
                }
                brandVM.Products = sortedProductItems;
                #endregion


                return Json(new
                {
                    PriceRange = PriceRange,
                    remain = totalCount,
                    NewdatRow = sortedProductItems,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "loadmorebrand", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Ads
        public JsonResult Ads(int? Id)
        {
            try
            {
                if (Id.HasValue)
                {

                    var ads = uow.AdverestingRepository.GetByID(Id);
                    if (ads != null)
                    {
                        if (ads.IsActive && ((ads.ExpireDate != null && ads.ExpireDate >= DateTime.Now) || ads.ExpireDate == null) && ((ads.StartDate != null && ads.StartDate <= DateTime.Now) || ads.StartDate == null))
                        {
                            ////Log 
                            //AdverestingLog adLog = new AdverestingLog()
                            //{
                            //    AdId = Id.Value,
                            //    ClientIP = Request.UserHostAddress,
                            //    Browser = Request.Browser.Browser,
                            //    UserAgent = GetUserPlatform(Request),
                            //    InsertDate = DateTime.Now
                            //};
                            //UnitOfWork.AdverestingLogRepository.Insert(adLog);
                            //UnitOfWork.Save();

                            ads.visits++;
                            uow.AdverestingRepository.Update(ads);
                            uow.Save();
                            //Redirect
                            return Json(new
                            {
                                url = ads.Link,
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);

                        }
                        else
                            return Json(new
                            {
                                url = "/",
                                statusCode = 404
                            }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new
                        {
                            url = "/",
                            statusCode = 404
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "addfavorate", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. ",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region search

        [CorrectArabianLetter(new string[] { "q" })]
        [Infrastructure.Filter.UserActivitiesFilter]
        public JsonResult Search(string q, int? page, int? perpage, int? sort)
        {
            try
            {
                if (String.IsNullOrEmpty(q))
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                string regex = @"^[a-zA-Z0-9_\u0600-\u06FF\s]+$";

                if (q.Contains("%") || !Regex.Match(q, regex).Success || q.Contains("www") || q.Length > 30)
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                if (!String.IsNullOrEmpty(q))
                    if (q.Length > 20)
                        q = q.Substring(0, 20);


                List<CoreLib.ViewModel.SearchParserString> keywords = CoreLib.Infrastructure.CommonFunctions.ParseStringsmall(q);
                List<string> where1List = keywords.Select(x => " products.BrandId in (select id from Brands where Name LIKE N'%" + x.val + "%' OR  Title LIKE N'%" + x.val + "%' OR  PersianName LIKE N'%" + x.val + "%') OR products.Title LiKE N'%" + x.val + "%' OR products.Name LiKE N'%" + x.val + "%' OR Products.LatinName LiKE N'%" + x.val + "%' OR ProductAttributeSelects.ProductId in (select productid from productprices where  ProductPrices.ProductAttributeSelectModelId IN(select id from ProductAttributeSelects where Value LIKE N'%" + x.val + "%') ) OR ProductAttributeSelects.ProductId in (select productid from productprices where  ProductPrices.ProductAttributeSelectSizeId IN(select id from ProductAttributeSelects where Value LIKE N'%" + x.val + "%')) ").ToList();
                var where1 = string.Join(" Or ", where1List);

                #region Products
                var totlaFilters = new getUrlParameter();
                totlaFilters.pagenum = page == null ? 1 : page.Value;
                totlaFilters.perpage = perpage == null ? 15 : perpage.Value;
                totlaFilters.sortby = sort.HasValue ? sort.Value : 4;
                //if (sort.HasValue)
                //    ViewBag.sort = sort.Value;
                totlaFilters.SearchStr = q;

                List<SearchResultSort> SearchResultSorts = new List<SearchResultSort>();
                List<ProductItem> sortedProductItems = new List<ProductItem>();
                var productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters, null, true, null, where1);
                // pageSetting
                var totalCount = productSearchResults.Count();
                int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
                if (skip < 0)
                    skip = 0;
                if (totlaFilters.perpage == 0)
                {
                    totlaFilters.perpage = 1;
                }
                var paggingModel = new PagingViewModel
                {
                    Count = totalCount,
                    CurentPage = totlaFilters.pagenum,
                    PerPage = totlaFilters.perpage,
                    RawUrl = Request.RawUrl
                };

                productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
                SearchResultSorts = productSearchResults.ToList().Select(c => new SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
                List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
                var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id)).ToList();
                foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
                {
                    sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
                }
                #endregion
                return Json(new
                {
                    //sort=sort.Value,
                    q = q,
                    TotalCount = totalCount,
                    pagging = paggingModel,
                    products = sortedProductItems,
                    title = "جستجوی " + q,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "search", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. ",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region SuperDeal
        public ActionResult SuperDeal(int? id)
        {
            try
            {

                var superdealPage = uow.ContentRepository.Get(x => x, x => x.IsSuperDeal).Single();
                if (superdealPage == null)
                    return Json(new
                    {
                        url = "/",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);


                SuperDealViewModel superDealViewModel = new SuperDealViewModel() { DefaultAmazingOffers = new List<AmazingOffers>(), currentAmazingOffers = new List<AmazingOffers>(), soonAmazingOffers = new List<AmazingOffers>(), noTimerAmazingOffers = new List<AmazingOffers>() };
                var date = DateTime.Now;
                var ProductOffers = uow.ProductOfferRepository.GetQueryList().Include("ProductPrice").AsNoTracking().Where(x => x.Offer.IsPublicOffer == true);
                var ids2 = ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).OrderBy(x => Guid.NewGuid()).Take(30).Select(x => x.ProductPrice.ProductId).ToList();
                int c2 = ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).Count();
                superDealViewModel.DefaultAmazingOffers.Add(new AmazingOffers
                {
                    Title = "پیشنهاد ویژه",
                    finish = c2 <= 30,
                    ProductItems = uow.ProductRepository.ProductItemList(x => ids2.Contains(x.Id), x => x.OrderByDescending(s => s.Id), 0, 30)
                });

                foreach (var item in uow.OfferRepository.GetQueryList().AsNoTracking().Where(x => x.ShowinSuerdeal && x.CodeTypeValueCode == 1 && x.IsActive && x.LanguageId == 1 && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null) && x.ProductOffers.Any()).OrderBy(x => x.Sort).Include("ProductOffers.ProductPrice.Product").Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 10).ToList())
                {
                    List<int> ids = new List<int>();
                    ids.AddRange(item.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).OrderBy(x => Guid.NewGuid()).Take(30).Select(x => x.ProductPrice.ProductId).ToList());
                    int c = item.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).Count();
                    superDealViewModel.currentAmazingOffers.Add(new AmazingOffers() { finish = c <= 30, Id = item.Id, Title = item.Title, Cover = item.Cover.HasValue ? item.attachment.FileName : "", EndDate = item.ExpireDate.HasValue ? item.ExpireDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + item.ExpireDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : null, ProductItems = uow.ProductRepository.ProductItemList(x => ids.Contains(x.Id), x => x.OrderByDescending(s => s.Id), 0, 30) });

                }

                foreach (var item in uow.OfferRepository.GetQueryList().AsNoTracking().Where(x => x.ShowinSuerdeal && x.IsActive && x.CodeTypeValueCode == 1 && x.LanguageId == 1 && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate > date)) && x.ProductOffers.Any()).OrderBy(x => x.Sort).Include("ProductOffers.ProductPrice.Product").Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 10).ToList())
                {
                    List<int> ids = new List<int>();
                    ids.AddRange(item.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).OrderBy(x => Guid.NewGuid()).Take(30).Select(x => x.ProductPrice.ProductId).ToList());
                    int c = item.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).Count();
                    superDealViewModel.soonAmazingOffers.Add(new AmazingOffers() { finish = c <= 30, Id = item.Id, Title = item.Title, Cover = item.Cover.HasValue ? item.attachment.FileName : "", EndDate = item.StartDate.HasValue ? item.StartDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + item.StartDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : null, ProductItems = uow.ProductRepository.ProductItemList(x => ids.Contains(x.Id), x => x.OrderByDescending(s => s.Id), 0, 30) });
                }
                foreach (var item in uow.OfferRepository.GetQueryList().AsNoTracking().Where(x => x.ShowinSuerdeal && x.IsActive && x.CodeTypeValueCode == 2 && x.LanguageId == 1 && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null) && x.ProductOffers.Any()).OrderBy(x => x.Sort).Include("ProductOffers.ProductPrice.Product").Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 10).ToList())
                {
                    List<int> ids = new List<int>();
                    ids.AddRange(item.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).OrderBy(x => Guid.NewGuid()).Take(30).Select(x => x.ProductPrice.ProductId).ToList());
                    int c = item.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).Count();
                    superDealViewModel.noTimerAmazingOffers.Add(new AmazingOffers() { finish = c <= 30, Id = item.Id, Title = item.Title, Cover = item.Cover.HasValue ? item.attachment.FileName : "", EndDate = item.ExpireDate.HasValue ? item.ExpireDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + item.ExpireDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : null, ProductItems = uow.ProductRepository.ProductItemList(x => ids.Contains(x.Id), x => x.OrderByDescending(s => s.Id), 0, 30) });
                }
                superDealViewModel.Name = superdealPage.Title;
                superDealViewModel.Abstract = superdealPage.Abstract;
                superDealViewModel.Data = superdealPage.Data;

                return Json(new
                {
                    data = superDealViewModel,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "superdeal", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. ",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult LoadMoreSuperDeal(int id, int pagenumber, int[] pids)
        {

            try
            {
                ViewBag.w = 233;
                ViewBag.h = 233;
                ViewBag.size = "SM";

                var setting = GetSetting();
                if (id == 0)
                {
                    List<ProductItem> products = new List<ProductItem>();
                    var ProductOffers = uow.ProductOfferRepository.GetQueryList().Include("ProductPrice").AsNoTracking().Where(x => x.Offer.IsPublicOffer == true && !pids.Contains(x.ProductPriceId.Value));
                    var ids2 = ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault && !pids.Contains(x.ProductPriceId.Value)).OrderBy(x => Guid.NewGuid()).Select(x => x.ProductPrice.ProductId).ToList();
                    int c2 = ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).Count();
                    products.AddRange(uow.ProductRepository.ProductItemList(x => ids2.Contains(x.Id), x => x.OrderByDescending(s => s.Id), (pagenumber - 1) * 30, 30));

                    return Json(new
                    {
                        finish = ((pagenumber - 1) * 30) + 30 >= c2 ? true : false,
                        NewRow = products,
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var offer = uow.OfferRepository.GetQueryList().AsNoTracking().Where(x => x.Id == id).Include("ProductOffers.ProductPrice.Product").Include("attachment").Single();

                    List<int> ids = new List<int>();
                    List<ProductItem> products = new List<ProductItem>();
                    ids.AddRange(offer.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault && !pids.Contains(x.ProductPriceId.Value)).OrderBy(x => Guid.NewGuid()).Select(x => x.ProductPrice.ProductId).ToList());
                    int c = offer.ProductOffers.Where(x => x.Quantity > 0 && x.ProductPrice.Quantity > 0 && x.Value > 0 && x.ProductPrice.IsDefault).Count();
                    products.AddRange(uow.ProductRepository.ProductItemList(x => ids.Contains(x.Id), x => x.OrderByDescending(s => s.Id), (pagenumber - 1) * 30, 30));

                    return Json(new
                    {
                        finish = ((pagenumber - 1) * 30) + 30 >= c ? true : false,
                        NewRow = products,
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "loadmoresuperdeal", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Product
        public async Task<JsonResult> getProduct(int? id, string name)
        {


            #region اعتبار سنجی اولیه

            //نام و ای دی محصول خالی است
            if (!id.HasValue)
                return Json(new
                {
                    data = "",
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);

            if (uow.RedirectUrlRepository.Any(x => x.Id, x => x.SourceTypeId == 7 && x.SourceLinkId == id.Value))
                return Json(new
                {
                    data = "",
                    url = uow.RedirectUrlRepository.Get(x => x.DestinationUrl, x => x.SourceTypeId == 7 && x.SourceLinkId == id.Value).First(),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);

            //  if (!String.IsNullOrEmpty(name))
            //    name = name.Replace("-", " ").ToLower();

            // چنین محصولی وجود دارد؟
            var pr = uow.ProductRepository.GetQueryList().Include("ProductPrices").Where(x => x.Id == id.Value && x.IsActive).SingleOrDefault();
            if (pr == null)
            {
                return Json(new
                {
                    data = "",
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            }
            if (String.IsNullOrEmpty(name))
            {
                return Json(new
                {
                    data = "",
                    url = string.Format("/tfp/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower())),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);

            }

            string incomingTitle = Server.UrlDecode(name ?? "").ToLower();
            string normalizedDbTitle = CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower());
            if (incomingTitle != normalizedDbTitle && !String.IsNullOrEmpty(name))
            {
                // name = name.Replace("-", " ").ToLower();
                return Json(new
                {
                    data = "",
                    url = string.Format("/tfp/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(pr.PageAddress.ToLower())),
                    statusCode = 301
                }, JsonRequestBehavior.AllowGet);

            }

            #endregion

            //تنوع پیش فرض
            var currentProductprice = pr.ProductPrices.Where(x => x.IsDefault).SingleOrDefault();
            if (currentProductprice == null)
                return Json(new
                {
                    data = "",
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);
            //محصول
            var product = uow.ProductPriceRepository.ProductDetailItemv2(pr.ProductPrices.Where(x => x.IsDefault).Single().Id);
            if (product == null)
                return Json(new
                {
                    data = "",
                    url = "/",
                    statusCode = 404
                }, JsonRequestBehavior.AllowGet);

            //محصولات مرتبط
            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", product.ProductCategory.id)).ToList();

            var prup = uow.ProductRepository.GetByID(id);
            prup.Visits++;
            uow.ProductRepository.Update(prup);
            await uow.SaveAsync();

            return Json(new
            {
                RelatedList = uow.ProductRepository.ProductItemList(x => x.IsActive && x.LanguageId == 1 && x.ProductPrices.Any(p => p.IsDefault && p.ProductStateId < 3) && x.ProductCategories.Any(p => CatIds.Contains(p.Id)) && x.state == 4 && x.Id != id.Value, x => x.OrderBy(o => o.ProductPrices.FirstOrDefault().ProductStateId).ThenByDescending(o => o.OrderRows.Count), 0, 10).ToList(),
                productFaqs = uow.ProductQuestionRepository.GetProductFAQv2(id.Value).OrderByDescending(x => x.Id).ToPagedList(1, 10),
                productComments = uow.ProductCommentRepository.GetProductCommentsv2(id.Value).OrderByDescending(x => x.Id).ToPagedList(1, 10),
                productGoogleFaq = uow.ProductQuestionRepository.GetProductGoogleFAQ(id.Value),
                productGoogleReview = uow.ProductRepository.GetProductReviewGoogleList(id.Value),
                ProductGoogleVideos = uow.ProductRepository.GetProductVideoGoogleList(id.Value),
                data = product,
                url = "",
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductPrice(string selector, int productId, string model, string size, int? color, int? garanty, string weight)
        {

            //UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            //  userActivitiesObj.SaveUserActivity(null, productId);

            try
            {
                var currentProductprice = uow.ProductPriceRepository.GetProductPrice(selector, productId, model, size, color, garanty, weight);
                var product = uow.ProductPriceRepository.ProductDetailItemv2(currentProductprice.Id);

                return Json(new
                {

                    productGoogleFaq = uow.ProductQuestionRepository.GetProductGoogleFAQ(productId),
                    productGoogleReview = uow.ProductRepository.GetProductReviewGoogleList(productId),
                    ProductGoogleVideos = uow.ProductRepository.GetProductVideoGoogleList(productId),
                    data = product,
                    url = "",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getproductprice", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetComments(int productid, int? page, int? sort)
        {
            try
            {
                // UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
                // userActivitiesObj.SaveUserActivity(null, productid);


                if (sort == 1) // به ترتیب جدیدترینها
                {
                    return Json(new
                    {
                        statusCode = 200,
                        NewRow = uow.ProductCommentRepository.GetProductCommentsv2(productid).OrderByDescending(x => x.Id).ToPagedList(page.Value, 10)
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (sort == 2)// به ترتیب خریدارن
                {
                    return Json(new
                    {
                        statusCode = 200,
                        NewRow = uow.ProductCommentRepository.GetProductCommentsv2(productid).OrderByDescending(x => x.IsBuy).ToPagedList(page.Value, 10)
                    }, JsonRequestBehavior.AllowGet);
                }
                else //به ترتیب مفیدها
                {
                    return Json(new
                    {
                        statusCode = 200,
                        NewRow = uow.ProductCommentRepository.GetProductCommentsv2(productid).OrderByDescending(x => x.Useful).ToPagedList(page.Value, 10)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getcomments", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult Usefulcomment(int id)
        {
            try
            {
                bool result = false;
                HttpCookie CommentCookie = HttpContext.Request.Cookies["PCommentRate"];
                if (CommentCookie == null)
                    CommentCookie = new HttpCookie("PCommentRate");
                if (CommentCookie[id.ToString()] != id.ToString())
                {

                    result = uow.ProductCommentRepository.SetUsefulcomment(id);
                    uow.Save();
                    CommentCookie[id.ToString()] = id.ToString();
                    CommentCookie.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Response.Cookies.Add(CommentCookie);
                }
                return Json(new
                {
                    statusCode = result ? 200 : 500,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "usefulcomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult Unusefulcomment(int id)
        {
            try
            {
                bool result = false;
                HttpCookie CommentCookie = HttpContext.Request.Cookies["PCommentRate"];
                if (CommentCookie == null)
                    CommentCookie = new HttpCookie("PCommentRate");
                if (CommentCookie[id.ToString()] != id.ToString())
                {

                    result = uow.ProductCommentRepository.SetUnusefulcomment(id);
                    uow.Save();
                    CommentCookie[id.ToString()] = id.ToString();
                    CommentCookie.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Response.Cookies.Add(CommentCookie);
                }
                return Json(new
                {
                    statusCode = result ? 200 : 500,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "unusefulcomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<JsonResult> AddProductComment(int ProductId, string Text, string Title, int? Satisfaction, string[] advantages, string[] disadvantages, string[] rankvalue, string[] rankid, HttpPostedFileBase[] UploadedImages, int? customerOrderId, int? OrderRowId, bool IsTemp, string fakeName, string fakeFamily)
        {
            bool fakeComment = !String.IsNullOrEmpty(fakeName) || String.IsNullOrEmpty(fakeFamily);
            // UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            // userActivitiesObj.SaveUserActivity(null, ocomment.ProductId);
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var userid = Authentication.ValidateToken(Token);
                if (userid != null)
                {

                    var ecomment = uow.ProductCommentRepository.Get(x => x, x => x.ProductId == ProductId && x.UserId == userid && x.IsTemp).FirstOrDefault();
                    if (ecomment != null)
                    {
                        //ecomment.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ocomment.Title);
                        ecomment.Text = Text;
                        if (Satisfaction.HasValue)
                            ecomment.Satisfaction = (ProductCommentSatisfaction)Satisfaction;

                        if (advantages != null)
                        {
                            ecomment.ProductCommentAdvantages = new List<ProductCommentAdvantage>();
                            foreach (var item in advantages)
                            {
                                if (!String.IsNullOrEmpty(item))
                                    ecomment.ProductCommentAdvantages.Add(new ProductCommentAdvantage() { Title = item });
                            }
                        }
                        if (disadvantages != null)
                        {
                            ecomment.ProductCommentDisAdvantages = new List<ProductCommentDisAdvantage>();
                            foreach (var item in disadvantages)
                            {
                                if (!String.IsNullOrEmpty(item))
                                    ecomment.ProductCommentDisAdvantages.Add(new ProductCommentDisAdvantage() { Title = item });
                            }
                        }
                        if (rankvalue != null && rankid != null)
                        {
                            ecomment.ProductRankSelectValues = new List<ProductRankSelectValue>();
                            int i = 0;
                            foreach (var item in rankvalue)
                            {
                                int value = int.Parse(item);
                                int id = int.Parse(rankid[i]);
                                ecomment.ProductRankSelectValues.Add(new ProductRankSelectValue() { IsPrimary = false, Value = value, UserId = userid, ProductRankSelectId = uow.ProductRankSelectRepository.Get(x => x.Id, x => x.ProductId == ProductId && x.ProductRankGroupSelect.RankId == id).First() });
                                i++;
                            }
                        }

                        uow.ProductCommentRepository.Update(ecomment);
                        uow.Save();

                        if (uow.OrderRepository.Any(x => x.Id, x => x.UserId == userid && x.OrderRows.Any(s => s.ProductId == ProductId) && x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true) && x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت) && x.compeleteRate == false))
                        {
                            var order = uow.OrderRepository.Get(x => x, x => x.UserId == userid && x.OrderRows.Any(s => s.ProductId == ProductId) && x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true) && x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت) && x.compeleteRate == false).SingleOrDefault();
                            if (order != null)
                            {
                                order.compeleteRate = true;
                                uow.OrderRepository.Update(order);
                                uow.Save();
                            }
                        }
                    }
                    else
                    {
                        ProductComment e2comment = new ProductComment() { InsertDate = DateTime.Now, Text = Text, ProductId = ProductId, OrderRowId = OrderRowId, UserId = userid, Title = Title, IsBuy = fakeComment || uow.OrderRowRepository.Any(x => x.Id, x => x.Order.UserId == userid && x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.ProductId == ProductId), IsTemp = IsTemp, fakeComment = fakeComment, fakeName = fakeName, fakeFamily = fakeFamily };

                        if (Satisfaction.HasValue)
                            e2comment.Satisfaction = (ProductCommentSatisfaction)Satisfaction;

                        if (advantages != null)
                        {
                            e2comment.ProductCommentAdvantages = new List<ProductCommentAdvantage>();
                            foreach (var item in advantages)
                            {
                                if (!String.IsNullOrEmpty(item))
                                    e2comment.ProductCommentAdvantages.Add(new ProductCommentAdvantage() { Title = uow.ProductAdvantageTitleRepository.GetByID(int.Parse(item)).Title });
                            }
                        }
                        if (disadvantages != null)
                        {
                            e2comment.ProductCommentDisAdvantages = new List<ProductCommentDisAdvantage>();
                            foreach (var item in disadvantages)
                            {
                                if (!String.IsNullOrEmpty(item))
                                    e2comment.ProductCommentDisAdvantages.Add(new ProductCommentDisAdvantage() { Title = uow.ProductDisAdvantageTitleRepository.GetByID(int.Parse(item)).Title });
                            }
                        }
                        if (rankvalue != null && rankid != null)
                        {
                            e2comment.ProductRankSelectValues = new List<ProductRankSelectValue>();
                            int i = 0;
                            foreach (var item in rankvalue)
                            {
                                int value = int.Parse(item);
                                int id = int.Parse(rankid[i]);
                                e2comment.ProductRankSelectValues.Add(new ProductRankSelectValue() { IsPrimary = false, Value = value, UserId = userid, ProductRankSelectId = uow.ProductRankSelectRepository.Get(x => x.Id, x => x.ProductId == e2comment.ProductId && x.ProductRankGroupSelect.RankId == id).First() });
                                i++;
                            }
                        }
                        if (UploadedImages != null)
                        {
                            if (UploadedImages.Any(x => x != null))
                            {
                                string messages = "";
                                List<attachment> newAttachmentList = new List<attachment>();
                                var result = UploadMultipleFile(UploadedImages, (!String.IsNullOrEmpty(Title) ? new string[] { Title } : UploadedImages.Select(x => x.FileName.Substring(0, x.FileName.LastIndexOf("."))).ToArray()), "on", "3", "on", "on", 1, 1437, "1", false, "", out messages, out newAttachmentList, userid);
                                if (result != null)
                                {
                                    e2comment.attachments = new List<attachment>();
                                    foreach (var item in newAttachmentList)
                                    {
                                        e2comment.attachments.Add(item);

                                    }
                                }
                            }
                        }

                        uow.ProductCommentRepository.Insert(e2comment);
                        uow.Save();

                        if (uow.OrderRepository.Any(x => x.Id, x => x.UserId == userid && x.OrderRows.Any(s => s.ProductId == e2comment.ProductId) && x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true) && x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت) && x.compeleteRate == false))
                        {
                            var order = uow.OrderRepository.Get(x => x, x => x.UserId == userid && x.OrderRows.Any(s => s.ProductId == e2comment.ProductId) && x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true) && x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت) && x.compeleteRate == false).SingleOrDefault();
                            if (order != null)
                            {
                                order.compeleteRate = true;
                                uow.OrderRepository.Update(order);
                                uow.Save();
                            }
                        }
                    }


                    var pr = uow.ProductRepository.GetByID(ProductId);
                    pr.RateAvg = Convert.ToInt32(uow.ProductRankSelectValueRepository.GetQueryList().Where(x => x.ProductRankSelect.ProductId == ProductId).Average(x => x.Value));
                    pr.CountAvg = Convert.ToInt32(uow.ProductRankSelectValueRepository.GetQueryList().Where(x => x.ProductRankSelect.ProductId == ProductId).GroupBy(x => x.UserId).Count());
                    uow.ProductRepository.Update(pr);
                    uow.Save();

                    var content = uow.ProductRepository.GetByID(ProductId);
                    ////Send Mail To Related Admin
                    //#region Create Html Body
                    //string EmailBodyHtml = "";

                    //var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment").SingleOrDefault();
                    //string contentUrl = "http://" + HttpContext.Request.Url.Host + "/TFP/" + ocomment.ProductId + "/" + CommonFunctions.NormalizeAddress(content.PageAddress);
                    //CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(osetting.attachmentFileName, "ثبت نظر محصول جدید در سایت", "مدیر گرامی نظر محصول جدیدی توسط ، " + User.Identity.Name + "، در سایت ثبت شد . شما میتوانید با مراجعه به پنل مدیریت آن را دیده و تایید نمایید .لینک صفحه مربوط به نظر : <br/> <a href='" + contentUrl + "'>" + content.Title + "</a><br/> نظر : <br/> <p>" + ocomment.Text + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                    //EmailBodyHtml = TfShop.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                    //#endregion

                    //#region SendMail
                    //var emails = uow.AdministratorPermissionRepository.Get(x => x, x => x.ModuleId == 4 && x.NotificationEmail == true).Select(x => x.User.Email).Distinct();

                    //EmailService es = new EmailService();
                    //await es.SendMultiDestinationAsync(" ثبت نظر محصول  جدید در سایت ", EmailBodyHtml, emails.ToList());

                    //#endregion

                    if (customerOrderId.HasValue)
                    {
                        return Json(new
                        {
                            url = "/profile/rate?id=" + customerOrderId + "&step=2",
                            Message = "ثبت شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {

                        return Json(new
                        {
                            id = ProductId,
                            Message = "نظر شما ثبت شد و پس از تایید در سایت نمایش داده می شود.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {

                    return Json(new
                    {
                        statusCode = 403
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "addproductcomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);

            }
        }

        protected List<EditorFile> UploadMultipleFile(HttpPostedFileBase[] uploadedFiles, string[] Title, string UseWaterMark, string WaterMarkType, string HasMultiSize, string UseCompression, Int16 compressionLevel, int? FolderId, string LanguageId, bool PopUpAttachements, string controllerName, out string messages, out List<attachment> newAttachmentList, string userid, bool fromComputer = false, bool uwebp = false, int? ProductId = null, int? ProductCommentId = null)
        {
            List<EditorFile> EditorFiles = new List<EditorFile>();
            messages = "";
            string HTMLString = "";
            newAttachmentList = new List<attachment>();
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();

                int success = uploadedFiles.Count();
                int i = 1;
                foreach (var File in uploadedFiles)
                {
                    if (File != null && File.ContentLength > 0)//Check File Is Selected ?
                    {
                        #region Get Extention
                        string extention = File.FileName.Substring(File.FileName.LastIndexOf("."));
                        var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                        #endregion
                        if (oFiletype != null)//Check Extension is Valid?
                        {
                            #region Get File From Uploader
                            byte[] FileByteArray = new byte[File.ContentLength];
                            File.InputStream.Read(FileByteArray, 0, File.ContentLength);
                            #endregion

                            #region Save To Database And Load Retated Object
                            attachment newAttchment = new attachment();
                            if (fromComputer)
                                newAttchment.UseCount = 1;
                            else
                                newAttchment.UseCount = 0;
                            newAttchment.InsertDate = DateTime.Now;
                            newAttchment.IsActive = true;
                            newAttchment.DisplaySort = 1;
                            newAttchment.LanguageId = Convert.ToInt16(LanguageId);
                            //newAttchment.FileType = uploadedFile.ContentType.ToString();
                            //newAttchment.FileContent = FileByteArray;
                            newAttchment.FileName = File.FileName;
                            newAttchment.FileTypeId = oFiletype.Id;
                            newAttchment.Capacity = File.ContentLength / 1024;
                            newAttchment.UserId = userid;
                            if (Title.Length > 1)
                            {
                                if (!string.IsNullOrEmpty(Title[(i - 1)]))
                                    newAttchment.Title = Title[(i - 1)];
                                else
                                    newAttchment.Title = " بی نام";
                            }
                            else
                            {
                                newAttchment.Title = Title[0];
                            }
                            if (FolderId.HasValue)
                                newAttchment.FolderId = FolderId.Value;
                            if (UseWaterMark == "on")
                                newAttchment.HasWatermark = true;
                            else
                                newAttchment.HasWatermark = false;
                            if (HasMultiSize == "on")
                                newAttchment.HasMultiSize = true;
                            else
                                newAttchment.HasMultiSize = false;
                            uow.AttachmentRepository.Insert(newAttchment);
                            uow.Save();
                            newAttachmentList.Add(newAttchment);

                            attachment upAttachement = uow.AttachmentRepository.GetByID(newAttchment.Id);
                            string fileName = "LG_" + newAttchment.Id.ToString() + uow.FiletypeRepository.GetByID(newAttchment.FileTypeId).FileTypeName;

                            if (controllerName != "filemanager")
                                upAttachement.FileName = controllerName + "/" + fileName;
                            else
                                upAttachement.FileName = fileName;
                            uow.Save();


                            //uow.Entry(newAttchment).Reference(c => c.FileType).Load();
                            //uow.Entry(newAttchment).Reference(c => c.Folder).Load();
                            #endregion

                            #region Upload File To Folder
                            string targetFolder = "";
                            if (controllerName != "filemanager")
                                //targetFolder = Server.MapPath("~/Content/UploadFiles/" + controllerName);
                            targetFolder = @"C:\inetpub\wwwroot\tfshops\public\UploadFiles\" + controllerName;
                            else
                                //targetFolder = Server.MapPath("~/Content/UploadFiles");
                            targetFolder = @"C:\inetpub\wwwroot\tfshops\public\UploadFiles\";
                            string targetPath = Path.Combine(targetFolder, fileName);
                            File.SaveAs(targetPath);
                            #endregion

                            #region Watermark Compression MultiSize
                            string UploadPath = @"C:\inetpub\wwwroot\tfshops\public\UploadFiles\" + (controllerName != "filemanager" ? controllerName + @"\" : "");
                            //string UploadPath = "~/Content/UploadFiles/" + (controllerName != "filemanager" ? controllerName + "/" : "");
                            if (CoreLib.Infrastructure.Image.ImageClass.IsFileAnImage(UploadPath + fileName))
                            {
                                string result = "";
                                int langId = int.Parse(LanguageId);
                                Setting oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == langId, null, "Waterattachment,attachment").SingleOrDefault();
                                if (newAttchment.HasWatermark)
                                {
                                    if (newAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                }
                                else
                                {
                                    if (newAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }

                                }

                                Infrastructure.EventLog.Logger.Add(5, "FileManager", "UploadMultipleFile", true, 500, result, DateTime.Now, userid);
                            }
                            #endregion


                            //FileInfo fi = new FileInfo(Server.MapPath("~/Content/UploadFiles/" + newAttchment.FileName));
                            FileInfo fi = new FileInfo(@"C:\inetpub\wwwroot\tfshops\public\UploadFiles\" + newAttchment.FileName);
                            newAttchment.Capacity = Convert.ToInt32(fi.Length / 1024);
                            uow.AttachmentRepository.Update(newAttchment);
                            uow.Save();

                            if (uwebp)
                            {
                                #region convertToWebP
                                var oattachment = newAttchment;
                                if (oattachment.FileType.FileTypeName == ".bmp" || oattachment.FileType.FileTypeName == ".jpg" || oattachment.FileType.FileTypeName == ".png" || oattachment.FileType.FileTypeName == ".jpeg" || oattachment.FileType.FileTypeName == ".tif")
                                {
                                    SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
                                    {
                                        Quality = 30
                                    });

                                    IImageFormat format;
                                    SixLabors.ImageSharp.Formats.Webp.WebpEncoder encoder = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder();

                                    string filename = @"C:\inetpub\wwwroot\tfshops\public\UploadFiles\" + oattachment.FileName;
                                    //string filename = Server.MapPath("~/Content/UploadFiles/" + oattachment.FileName);

                                    if (System.IO.File.Exists(filename))
                                    {
                                        using (var image = SixLabors.ImageSharp.Image.Load(filename, out format))
                                        {
                                            image.SaveAsWebp(filename.Substring(0, filename.LastIndexOf(".")) + ".webp", encoder);
                                        }
                                        if (oattachment.HasMultiSize)
                                        {

                                            if (System.IO.File.Exists(filename.Replace("LG_", "MD_")))
                                            {
                                                using (var image = SixLabors.ImageSharp.Image.Load(filename.Replace("LG_", "MD_"), out format))
                                                {
                                                    image.SaveAsWebp(filename.Substring(0, filename.LastIndexOf(".")).Replace("LG_", "MD_") + ".webp", encoder);
                                                }
                                            }
                                            if (System.IO.File.Exists(filename.Replace("LG_", "SM_")))
                                            {
                                                using (var image = SixLabors.ImageSharp.Image.Load(filename.Replace("LG_", "SM_"), out format))
                                                {
                                                    image.SaveAsWebp(filename.Substring(0, filename.LastIndexOf(".")).Replace("LG_", "SM_") + ".webp", encoder);
                                                }
                                            }
                                            if (System.IO.File.Exists(filename.Replace("LG_", "XS_")))
                                            {
                                                using (var image = SixLabors.ImageSharp.Image.Load(filename.Replace("LG_", "XS_"), out format))
                                                {
                                                    image.SaveAsWebp(filename.Substring(0, filename.LastIndexOf(".")).Replace("LG_", "XS_") + ".webp", encoder);
                                                }
                                            }
                                        }
                                    }

                                    oattachment.FileName = oattachment.FileName.Replace(oattachment.FileType.FileTypeName, ".webp");
                                    uow.AttachmentRepository.Update(oattachment);
                                    uow.Save();


                                    System.IO.File.Delete(filename);
                                    if (oattachment.HasMultiSize)
                                    {
                                        System.IO.File.Delete(filename.Replace("LG_", "MD_"));
                                        System.IO.File.Delete(filename.Replace("LG_", "SM_"));
                                        System.IO.File.Delete(filename.Replace("LG_", "XS_"));
                                    }

                                }


                                #endregion
                            }
                            var setting = uow.SettingRepository.Get(x => x.StaticContentDomain).First();
                            messages += i.ToString() + "- " + "فایلِ " + File.FileName + " با موفقیت ذخیره شد. ";
                            EditorFiles.Add(new EditorFile() { FileName = setting + "/UploadFiles/" + controllerName + "/" + fileName, Title = upAttachement.Title, fileType = extention });
                        }
                        else
                        {
                            success--;
                            messages += i.ToString() + "- " + " پسوندِ فایلِ  " + File.FileName + " مجاز نیست. برای کسب اطلاعات بیشتر به مدیریت پسوند ها مراجعه کنید، ";
                        }
                    }
                    else
                    {
                        success--;
                        messages += i.ToString() + "- " + "  فایل خراب است،  ";
                    }
                    i++;



                }//for each

                if (success > 0)
                {


                    //if (PopUpAttachements)
                    //    HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulkSimple", newAttachmentList, this.ControllerContext);
                    //else
                    //    HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulk", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, userid);
                    #endregion

                }

                return EditorFiles;
            }
            catch (Exception x)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "UploadMultipleFile", true, 500, x.Message + x.InnerException != null ? x.InnerException.Message : "", DateTime.Now, userid);
                return null;
            }
        }

        [JWTAuthorize]
        public virtual JsonResult UplodMultipleFileFromComputer(HttpPostedFileBase[] uploadedFiles, int ProductId, int? ProductCommentId)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            var userid = Authentication.ValidateToken(Token);

            try
            {
                int? folderId = null, ufolderId = null;

                var user = uow.UserRepository.GetByID(User.Identity.GetUserId());
                string ufolder = "فایل های کاربران";
                var uufolder = uow.FolderRepository.Get(x => x, x => x.FolderName == ufolder).FirstOrDefault();
                if (uufolder != null)
                    ufolderId = uufolder.Id;
                else
                {

                    Folder newfolder = new Folder();
                    newfolder.FolderName = ufolder;
                    newfolder.LanguageId = uow.SettingRepository.Get(x => x).First().LanguageId;
                    uow.FolderRepository.Insert(newfolder);
                    uow.Save();
                    ufolderId = newfolder.Id;
                }

                string cfoldername = user.FirstName + " " + user.LastName;
                var folder = uow.FolderRepository.Get(x => x, x => x.FolderName == cfoldername).FirstOrDefault();
                if (folder != null)
                    folderId = folder.Id;
                else
                {
                    Folder newfolder = new Folder();
                    newfolder.FolderName = cfoldername;
                    newfolder.FolderID = ufolderId;
                    newfolder.LanguageId = uow.SettingRepository.Get(x => x).First().LanguageId;
                    uow.FolderRepository.Insert(newfolder);
                    uow.Save();
                    folderId = newfolder.Id;
                }

                string messages = "";
                List<attachment> newAttachmentList = new List<attachment>();
                var setting = uow.SettingRepository.Get(x => x).First();
                List<EditorFile> result = new List<EditorFile>();
                if (ProductCommentId.HasValue)
                    result = UploadMultipleFile(uploadedFiles, uploadedFiles.Select(x => x.FileName.Substring(0, x.FileName.LastIndexOf("."))).ToArray(), "on", setting.WaterMarkPosition.ToString(), "off", "on", 9, folderId, setting.LanguageId.Value.ToString(), true, "filemanager", out messages, out newAttachmentList, userid, true, false, ProductId, ProductCommentId);
                else

                    result = UploadMultipleFile(uploadedFiles, uploadedFiles.Select(x => x.FileName.Substring(0, x.FileName.LastIndexOf("."))).ToArray(), "on", setting.WaterMarkPosition.ToString(), "off", "off", 9, folderId, setting.LanguageId.Value.ToString(), true, "filemanager", out messages, out newAttachmentList, userid, true, false, ProductId, ProductCommentId);

                if (result.Any())
                {


                    var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    string HTMLString = "";
                    HTMLString = CaptureHelper.RenderViewToString("_AttachmentJanebi", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        id = ProductCommentId,
                        statusCode = 200,
                        successCounter = 1,
                        status = messages,
                        NewRow = HTMLString
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 400,
                        successCounter = 0,
                        status = messages
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "UplodMultiple", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                #region Unexpected Error
                return Json(new
                {
                    statusCode = 400,
                    successCounter = 0,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);
                #endregion
            }


        }


        [HttpPost]
        public JsonResult GetQuestions(int productid, int? page, int? sort)
        {
            try
            {
                // UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
                //  userActivitiesObj.SaveUserActivity(null, productid);
                string userid = "";
                if (Request.Headers.AllKeys.Any(x => x == "AuthToken"))
                {
                    string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                    if (!String.IsNullOrEmpty(Token))
                        userid = Authentication.ValidateToken(Token);
                }

                if (sort == 1) // به ترتیب جدیدترینها
                {
                    return Json(new
                    {

                        statusCode = 200,
                        NewRow = uow.ProductQuestionRepository.GetProductFAQv2(productid).OrderByDescending(x => x.Id).ToPagedList(page.Value, 10)
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (sort == 2)// بیشترین پاسخ به پرسش
                {
                    return Json(new
                    {
                        statusCode = 200,
                        NewRow = uow.ProductQuestionRepository.GetProductFAQv2(productid).OrderByDescending(x => x.ChildComment.Count()).ToPagedList(page.Value, 10)
                    }, JsonRequestBehavior.AllowGet);
                }
                else //پرسش های شما
                {
                    if (userid != "")
                    {
                        return Json(new
                        {
                            statusCode = 200,
                            NewRow = uow.ProductQuestionRepository.GetProductFAQv2(productid).Where(x => x.UserId == userid).OrderByDescending(x => x.Id).ToPagedList(page.Value, 10)
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 200,
                            NewRow = uow.ProductQuestionRepository.GetProductFAQv2(productid).OrderByDescending(x => x.Id).ToPagedList(page.Value, 10)
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getquestions", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult UsefulQuestion(int id)
        {
            try
            {
                bool result = false;
                HttpCookie CommentCookie = HttpContext.Request.Cookies["PFAQRate"];
                if (CommentCookie == null)
                    CommentCookie = new HttpCookie("PFAQRate");
                if (CommentCookie[id.ToString()] != id.ToString())
                {

                    result = uow.ProductQuestionRepository.SetUsefulQuestion(id);
                    uow.Save();
                    CommentCookie[id.ToString()] = id.ToString();
                    CommentCookie.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Response.Cookies.Add(CommentCookie);
                }
                return Json(new
                {
                    statusCode = result ? 200 : 500,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "usefulquestion", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UnusefulQuestion(int id)
        {
            try
            {
                bool result = false;
                HttpCookie CommentCookie = HttpContext.Request.Cookies["PFAQRate"];
                if (CommentCookie == null)
                    CommentCookie = new HttpCookie("PFAQRate");
                if (CommentCookie[id.ToString()] != id.ToString())
                {

                    result = uow.ProductQuestionRepository.SetUnusefulQuestion(id);
                    uow.Save();
                    CommentCookie[id.ToString()] = id.ToString();
                    CommentCookie.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Response.Cookies.Add(CommentCookie);
                }
                return Json(new
                {
                    statusCode = result ? 200 : 500,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "unusefulquestion", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    statusCode = 500,
                    NewRow = "",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [JWTAuthorize]
        public ActionResult GetAddComment(int? id)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                var userid = Authentication.ValidateToken(Token);
                if (userid == null)
                {
                    return Json(new
                    {
                        url = "/account/login?returnUrl=/tfp/" + id.Value,
                        statusCode = 403
                    }, JsonRequestBehavior.AllowGet);
                }


                IdentityManager im = new IdentityManager();
                bool commenter = im.IsInRole(userid, "Comment");


                if (uow.ProductCommentRepository.Any(x => x.Id, x => (x.ProductId == id.Value && x.UserId == userid && x.IsTemp == false && !commenter)))
                    return Json(new
                    {

                        message = "نظر شما درباره این محصول ثبت شده است. از  پروفایل کاربری می‌توانید پیگیری کنید",
                        statusCode = 403
                    }, JsonRequestBehavior.AllowGet);
                if (!id.HasValue)
                    return Json(new
                    {
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                if (uow.ProductRepository.GetByID(id.Value) == null)
                    return Json(new
                    {
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);


                var ProductRanks = uow.ProductRankRepository.Get(x => x, x => x.ProductRankGroupSelects.Any(s => s.ProductRankSelects.Any(a => a.ProductId == id.Value)));

                ProductAddCommentv2 productAddComment = new ProductAddCommentv2()
                {
                    ProductId = id.Value,
                    ProductAdvantageTitless = uow.ProductAdvantageTitleRepository.Get(x => x, x => x.IsActive, x => x.OrderBy(s => s.DisplayOrder)).Select(x => new ProductAdvantageTitle { Id = x.Id, Title = x.Title, DisplayOrder = x.DisplayOrder }),
                    ProductDisAdvantageTitles = uow.ProductDisAdvantageTitleRepository.Get(x => x, x => x.IsActive, x => x.OrderBy(s => s.DisplayOrder)).Select(x => new ProductDisAdvantageTitle { Id = x.Id, Title = x.Title, DisplayOrder = x.DisplayOrder }),
                    ProductComment = uow.ProductCommentRepository.Get(x => x, x => x.ProductId == id.Value && x.UserId == userid && x.IsTemp).Select(x => new ProductCommentvm() { ProductId = x.ProductId, Text = x.Text, IsTemp = x.IsTemp, attachments = x.attachments.Select(s => new attachmentvm() { id = s.Id, filename = s.FileName }) }).FirstOrDefault(),
                    productRanks = ProductRanks.Select(x => new ProductRankvm() { id = x.Id, name = x.Name }),
                    productItem = uow.ProductRepository.ProductItemv2(x => x.Id == id.Value, x => x.OrderBy(s => s.Id), 0, 1),
                    IsBuy = uow.OrderRowRepository.Any(x => x.Id, x => x.Order.UserId == userid && x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.ProductId == id.Value),
                    commenter = commenter
                };
                productAddComment.AllowAddComment = (productAddComment.ProductComment != null && uow.ProductCommentRepository.Any(x => x.Id, x => x.ProductId == id.Value && x.UserId == userid && x.IsTemp == true)) || (productAddComment.ProductComment == null && !uow.ProductCommentRepository.Any(x => x.Id, x => x.ProductId == id.Value && x.UserId == userid)) || commenter;

                return Json(new
                {
                    data = productAddComment,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "getaddcomment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);


            }
        }

        [HttpPost]
        [JWTAuthorize]
        public ActionResult AddQuestion(string Message, int ProductId, string FakeUserFullName, int? ProductQuestionId)
        {
            //UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            //userActivitiesObj.SaveUserActivity(null, ProductId);

            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            var userid = Authentication.ValidateToken(Token);
            try
            {

                if (userid != null)
                {
                    ProductQuestion prq = new ProductQuestion()
                    {
                        InsertDate = DateTime.Now,
                        IsActive = false,
                        Like = 0,
                        Message = Message,
                        ParrentId = ProductQuestionId,
                        ProductId = ProductId,
                        UnLike = 0,
                        UserId = userid,
                        Visited = false,
                        FakeUserFullName = FakeUserFullName
                    };
                    uow.ProductQuestionRepository.Insert(prq);
                    uow.Save();

                    var content = uow.ProductRepository.GetByID(prq.ProductId);

                    //Send Mail To Related Admin
                    #region Create Html Body
                    //string EmailBodyHtml = "";

                    //var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment").SingleOrDefault();
                    //string contentUrl = "http://" + HttpContext.Request.Url.Host + "/TFP/" + prq.ProductId + "/" + CommonFunctions.NormalizeAddress(content.PageAddress);
                    //CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(osetting.attachmentFileName, "ثبت پرسش محصول جدید در سایت", "مدیر گرامی پرسش محصول جدیدی توسط ، " + User.Identity.Name + "، در سایت ثبت شد . شما میتوانید با مراجعه به پنل مدیریت آن را دیده و تایید نمایید .لینک صفحه مربوط به نظر : <br/> <a href='" + contentUrl + "'>" + content.Title + "</a><br/> نظر : <br/> <p>" + Messag + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                    //EmailBodyHtml = TfShop.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                    //#endregion

                    //#region SendMail
                    //var emails = uow.AdministratorPermissionRepository.Get(x => x, x => x.ModuleId == 4 && x.NotificationEmail == true).Select(x => x.User.Email).Distinct();

                    //EmailService es = new EmailService();
                    //await es.SendMultiDestinationAsync(" ثبت نظر محصول  جدید در سایت ", EmailBodyHtml, emails.ToList());

                    #endregion


                    return Json(new
                    {
                        Message = "ثبت شد.",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 403
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "addquestion", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Profile

        #region Index

        [JWTAuthorize]
        public async Task<JsonResult> getprofile(ManageMessageId? message, string PhoneNumberConfirmation, int? Error, string msg)
        {

            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
                DateTime dt = DateTime.Now;
                var user = uow.UserRepository.Get(x => x, x => x.Id == userId, null, "UserMessage2s,Avatarattachment,ProductLetmeknows,ProductFavorates,Wallets").SingleOrDefault();
                var model = new IndexViewModelV2
                {
                    IsInNewsLetter = uow.NewsLetterEmailRepository.Any(x => x, x => x.Email == user.UserName && x.IsVerified),
                    HasPassword = HasPassword(userId),
                    PhoneNumber = user.PhoneNumber,
                    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                    EmailConfirmed = await UserManager.IsEmailConfirmedAsync(userId),
                    PhoneNumberConfirmed = await UserManager.IsPhoneNumberConfirmedAsync(userId),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    About = user.About,
                    Gender = user.Gender,
                    Avatar = user.Avatar,
                    BirthDate = user.BirthDate,
                    CardNumber = user.CardNumber,
                    City = user.City,
                    State = user.State,
                    LandlinePhone = user.LandlinePhone,
                    NationalCode = user.NationalCode,
                    Address = user.Address,
                    Email = user.Email,
                    Avatarattachment = user.Avatar.HasValue ? user.Avatarattachment.FileName : "",
                    MessageCout = user.UserMessage2s.Where(x => x.state == false).Count(),
                    BonCount = uow.UserBonRepository.Count(x => x.state == false && x.ExpireDate > dt),
                    CodeCount = uow.UserCodeGiftRepository.Count(x => ((x.ExpireDate != null && x.ExpireDate > dt) || x.ExpireDate == null) && ((x.Offer.ExpireDate != null && x.Offer.ExpireDate > dt) || x.Offer.ExpireDate == null) && ((x.Offer.StartDate != null && x.Offer.StartDate <= dt) || x.Offer.StartDate == null) && x.IsActive && x.Offer.IsActive && x.Offer.IsDeleted == false && x.Offer.state == true && x.ExpireDate.Value >= dt && x.IsActive && x.UserId == userId),
                    NoticeCount = user.ProductLetmeknows.Where(x => x.Notofied == false || x.NotofiedEmail == false || x.NotofiedSms == false).Count(),
                    FavCount = user.ProductFavorates.Count(),
                    wallet = user.Wallets.Where(x => x.State && x.DepositOrWithdrawal == true).Sum(x => x.Price) - user.Wallets.Where(x => x.State && x.DepositOrWithdrawal == false).Sum(x => x.Price)
                };


                return Json(new
                {
                    data = model,
                    Error = Error,
                    Message = msg,
                    StatusMessage =
                    message == ManageMessageId.ChangePasswordSuccess ? "رمز عبور شما تغییر یافت."
                    : message == ManageMessageId.SetPasswordSuccess ? "رمز عبور شما تنظیم شد."
                    : message == ManageMessageId.SetTwoFactorSuccess ? "احراز هویت دو مرحله ای شما تنظیم شد."
                    : message == ManageMessageId.Error ? "خطایی اتفاق افتاد."
                    : message == ManageMessageId.AddPhoneSuccess ? "شماره تلفن شما اضافه شد."
                    : message == ManageMessageId.RemovePhoneSuccess ? "شماره تماس شما حذف شد."
                    : message == ManageMessageId.SendConfirmEmail ? "ایمیلِ تاییدِ ایمیل برای شما ارسال شد."
                    : message == ManageMessageId.SendConfirmPhone ? "کد فعال سازی به تلفن همراه شما ارسال شد."
                    : "",
                    EducationMessage = !String.IsNullOrEmpty(PhoneNumberConfirmation) ? "برای استفاده از امکانات وب سایت باید تلفن همراه خود را تایید نمایید. لطفا با کلیک روی تایید تلفن همراه، شماره موبایل خود را تایید نمایید." : "",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }

        }
        private bool HasPassword(string userid)
        {
            var user = UserManager.FindById(userid);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> ConfirmEmail()
        {

            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            ManageMessageId? message;
            var user = UserManager.FindById(userId);
            if (!String.IsNullOrEmpty(user.Email))
            {
                try
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("VerifyEmail", "Profile", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    #region Create Html Body
                    string EmailBodyHtml = "";


                    var setting = GetSetting();
                    CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(setting.attachmentFileName, "تایید ایمیلِ حساب کاربری", "لطفا با کلیک روی لینک مقابل ایمیل خود را تایید نمایید : <a href='" + callbackUrl + "'> لینک تایید ایمیل</a>", setting.WebSiteName, HttpContext.Request.Url.Host, setting.WebSiteTitle);
                    EmailBodyHtml = TfShop.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);

                    #endregion

                    EmailService es = new EmailService();
                    IdentityMessage imessage = new IdentityMessage();
                    imessage.Body = EmailBodyHtml;
                    imessage.Destination = user.Email;
                    imessage.Subject = " تایید ایمیل حساب کاربری شما در " + setting.WebSiteName;
                    await es.SendAsync(imessage);

                    message = ManageMessageId.SendConfirmEmail;
                    return Json(new
                    {
                        Message = message,
                        url = "/profile",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    Infrastructure.EventLog.Logger.Add(5, "api", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return Json(new
                    {
                        Message = "خطایی رخ داد.",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                Infrastructure.EventLog.Logger.Add(5, "api", "profile", false, 500, "خطایی رخ داد", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            {
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> PhoneConfirm()
        {

            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            // ManageMessageId? message;
            var user = UserManager.FindById(userId);

            // PhoneConfirm خارج از دامنه‌ی این تغییر (ورود/ثبت‌نام/فراموشی‌رمز توی login-flow.tsx)
            // بود - عمداً 6 رقمی و 10 دقیقه‌ای نگه داشته شد چون مشخص نبود کدوم UI/صفحه‌ای ازش
            // استفاده می‌کنه (شاید هنوز پورت نشده به Next.js).
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            Session["Code"] = r;
            try
            {
                user.ActiveTempCode = r;
                user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(10);
                await UserManager.UpdateAsync(user);

                SmsService sms = new SmsService();
                IdentityMessage iPhonemessage = new IdentityMessage();
                iPhonemessage.Destination = user.UserName;
                iPhonemessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.تایید_تلفن_همراه_پروفایل, user.Id, null, null, null, null, r, null, null, null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Edit", null);
                if (!String.IsNullOrEmpty(iPhonemessage.Body))
                    await sms.SendSMSAsync(iPhonemessage, null, null, null, null, null, null, true);

                return Json(new
                {
                    url = "/profile/verifyphone",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "phoneconfirm", false, 500, "خطایی رخ داد", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [JWTAuthorize]
        public virtual async Task<ActionResult> VerifyEmail(string code)
        {
            try
            {
                if (code == null)

                    if (code == null)
                        return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {

                    NewsLetterEmail oNewsLetterEmail = new NewsLetterEmail();
                    IdentityManager im = new IdentityManager();
                    oNewsLetterEmail.Email = im.GetUser(userId).Email;
                    oNewsLetterEmail.InsertDate = DateTime.Now;
                    oNewsLetterEmail.LanguageId = 1;
                    oNewsLetterEmail.IsVerified = true;
                    uow.NewsLetterEmailRepository.Insert(oNewsLetterEmail);
                    uow.Save();
                }

                return Json(new
                {
                    Message = result.Succeeded ? "تایید شد" : "خطایی رخ داد",
                    statusCode = result.Succeeded ? 200 : 500,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "VerifyEmail", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        [JWTAuthorize]
        public JsonResult profileVerifyPhone(string code)
        {

            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {

                if (code == null)
                {
                    return Json(new
                    {
                        Message = "کد فعال سازی را وارد نمایید.",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }
                if (Session["Code"].ToString() != code)
                {
                    return Json(new
                    {
                        Message = "کد وارد شده صحیح نیست.",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }

                var user = uow.UserRepository.GetByID(userId);
                user.PhoneNumberConfirmed = true;
                user.Disable = false;
                uow.Save();

                return Json(new
                {
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "profileverifyphone", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);


                var token = await UserManager.GeneratePasswordResetTokenAsync(userId);

                var result = await UserManager.ResetPasswordAsync(userId, token, model.NewPassword);

                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }

                    return Json(new
                    {
                        Message = "با موفقیت انجام شد",
                        statusCode = 200,
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "changepassword", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        public async Task<JsonResult> Edit(string r, int? Update, string returnurl, int? m)
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                if (Update.HasValue)
                {
                    if (Update.Value == 0)
                    {
                        return Json(new
                        {
                            Message = " لطفا پروفایل کاربری خود را تکمیل نمایید ( وارد کردن نام و نام خانوادگی اجباری است). ",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                var user = uow.UserRepository.Get(x => x, x => x.Id == userId, null, "CityEntity,Avatarattachment").First();
                var step1 = new EditProfileViewModelStep1V2
                {
                    Id = userId,
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    About = user.About,
                    Gender = user.Gender,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Avatarattachment = user.Avatar.HasValue ? user.Avatarattachment.FileName : "",
                    LandlinePhone = user.LandlinePhone,
                    NationalCode = user.NationalCode,
                    BirthDate = user.BirthDate,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed

                };
                var step2 = new EditProfileViewModelStep2V2
                {
                    Id = userId,
                    City = user.CityId.HasValue ? user.CityEntity.Name : "",
                    CityId = user.CityId,
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    PostalCode = user.PostalCode,
                    State = user.State,
                    Address = user.Address,

                    AddressNumber = user.AddressNumber,
                    AddressUnit = user.AddressUnit,
                    FooterGoogleMapLongitude = user.FooterGoogleMapLongitude,
                    FooterGoogleMapLatitude = user.FooterGoogleMapLatitude,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed

                };
                var step3 = new EditProfileViewModelStep3V2
                {
                    Id = userId,

                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    CardNumber = user.CardNumber,
                    AccountNumber = user.AccountNumber,
                    SHBNNumber = user.SHBNNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed

                };
                ViewBag.returnurl = returnurl;
                int ProvinceId = 8;
                if (step2.CityId.HasValue)
                    ProvinceId = user.CityEntity.ProvinceId;


                return Json(new
                {
                    m = m.HasValue ? m : null,
                    r = r,
                    ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => new { x.Id, x.Name }), "Id", "Name", ProvinceId),
                    cityList = new SelectList(uow.CityRepository.Get(x => new { x.Id, x.Name }, x => x.ProvinceId == ProvinceId), "Id", "Name", step2.CityId.HasValue ? step2.CityId : 304),
                    data = new EditProfileV2() { EditProfileViewModelStep1 = step1, EditProfileViewModelStep2 = step2, EditProfileViewModelStep3 = step3 },
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "edit", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [JWTAuthorize]
        public ActionResult ProfileSideBar()
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var user = uow.UserRepository.GetQueryList().Include(x => x.UserMessage2s).Include(x => x.Avatarattachment).AsNoTracking().FirstOrDefault(u => u.Id == userId);
                var showSideBarProfileViewModel = new ShowSideBarProfileViewModelV2()
                {
                    Avatar = user.Avatar,
                    Avatarattachment = user.Avatar.HasValue ? user.Avatarattachment.FileName : "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MessageCout = user.UserMessage2s.Where(x => x.state == false).Count()
                };

                return Json(new
                {
                    data = showSideBarProfileViewModel,
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "changepassword", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg", "heic" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.ToLower().EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<ActionResult> Edit(EditProfileViewModel model, HttpPostedFileBase file, string ShamsiBirthDate, string r, string returnurl)
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var userinfo = UserManager.FindById(userId);

                model.CityEntity = uow.CityRepository.GetByID(model.CityId);

                int ProvinceId = 8;
                if (model.CityId.HasValue)
                    ProvinceId = uow.CityRepository.Get(x => x.ProvinceId, x => x.Id == model.CityId).First();

                string msg = "";
                bool IsValid = true;
                string attachementId = "";

                if (!model.CityId.HasValue)
                {
                    msg = "شهر انتخاب نشده است ! ";
                    IsValid = false;
                }
                if (model.Gender == null)
                {
                    msg += "جنسیت انتخاب نشده است !";
                    IsValid = false;
                }

                if (file != null)
                {
                    if (IsImage(file))
                    {
                        if ((file.ContentLength / 1024) <= 150)
                        {

                            string extention = file.FileName.Substring(file.FileName.LastIndexOf("."));
                            var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                            if (oFiletype != null)//Check Extension is Valid?
                            {
                                try
                                {

                                    attachment NewAttachement = new attachment();
                                    NewAttachement.Capacity = file.ContentLength / 1024;
                                    NewAttachement.DisplaySort = 0;
                                    NewAttachement.FileName = userinfo.UserName + "/" + file.FileName;
                                    NewAttachement.FileTypeId = oFiletype.Id;
                                    NewAttachement.HasMultiSize = false;
                                    NewAttachement.HasWatermark = false;
                                    NewAttachement.InsertDate = DateTime.Now;
                                    NewAttachement.IsActive = true;
                                    NewAttachement.LanguageId = 1;
                                    NewAttachement.Title = " آواتارِ کاربرِ " + userinfo.UserName;
                                    NewAttachement.UpdateDate = DateTime.Now;
                                    NewAttachement.UseCount = 1;
                                    IdentityManager im = new IdentityManager();
                                    NewAttachement.UserId = userId;

                                    uow.AttachmentRepository.Insert(NewAttachement);
                                    uow.Save();

                                    attachementId = NewAttachement.Id.ToString();
                                    string fileName = "LG_" + NewAttachement.Id.ToString() + NewAttachement.FileType.FileTypeName;
                                    attachment upAttachement = uow.AttachmentRepository.GetByID(NewAttachement.Id);
                                    upAttachement.FileName = userinfo.UserName + "/" + fileName;
                                    uow.Save();

                                    if (!System.IO.Directory.Exists(Server.MapPath("~/Content/UploadFiles/" + userinfo.UserName)))
                                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/UploadFiles/" + userinfo.UserName));

                                    string targetFolder = Server.MapPath("~/Content/UploadFiles/" + userinfo.UserName);
                                    string targetPath = System.IO.Path.Combine(targetFolder, fileName);
                                    file.SaveAs(targetPath);
                                }
                                catch (Exception x)
                                {
                                    msg = x.Message;
                                    IsValid = false;
                                }
                            }
                            else
                            {
                                msg = " پسوند فایل مجاز نیست. ";
                                IsValid = false;
                            }
                        }
                        else
                        {
                            msg = "حجم فایل باید کمتر از 150 کیلوبایت باشد.";
                            IsValid = false;
                        }
                    }
                    else
                    {
                        msg = " فایل انتخاب شده باید .jpg،.png،.jpeg یا .gif باشد . ";
                        IsValid = false;
                    }
                }
                if (IsValid)
                {

                    if (uow.UserRepository.Any(x => x.Id, x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id))
                    {
                        return Json(new
                        {
                            Message = " تلفن همراه وارد شده تکراری است.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var user = UserManager.FindById(userId);
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    user.Gender = model.Gender;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.About = model.About;
                    user.LandlinePhone = model.LandlinePhone;
                    user.PostalCode = model.PostalCode;
                    user.State = model.State;
                    user.City = model.City;
                    user.Address = model.Address;
                    user.AddressNumber = model.AddressNumber;
                    user.AddressUnit = model.AddressUnit;
                    user.NationalCode = model.NationalCode;
                    user.CityId = model.CityId;
                    user.CardNumber = model.CardNumber;
                    user.AccountNumber = model.AccountNumber;
                    user.SHBNNumber = model.SHBNNumber;
                    user.FooterGoogleMapLatitude = model.FooterGoogleMapLatitude;
                    user.FooterGoogleMapLongitude = model.FooterGoogleMapLongitude;
                    if (ShamsiBirthDate != "")
                    {
                        user.BirthDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ShamsiBirthDate);
                    }
                    if (file != null)
                    {
                        //delete old file
                        if (user.Avatar != null)
                        {
                            attachment OldAttachement = uow.AttachmentRepository.GetByID(user.Avatar);
                            if (OldAttachement != null)
                            {
                                //Delete File
                                string OldFileName = Server.MapPath("~/Content/UploadFiles/" + OldAttachement.FileName);
                                //Delete Row
                                uow.AttachmentRepository.Delete(OldAttachement);
                                uow.Save();

                                if (System.IO.File.Exists(OldFileName))
                                    System.IO.File.Delete(OldFileName);
                            }
                        }
                        //update user avatart Id
                        Guid g = new Guid(attachementId);
                        user.Avatar = g;

                    }
                    IdentityResult ir = await UserManager.UpdateAsync(user);
                    if (ir.Succeeded)
                    {

                        return Json(new
                        {
                            returnurl = returnurl,
                            r = r,
                            ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                            cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                            url = !String.IsNullOrEmpty(returnurl) ? returnurl : r != "" ? r : "/profile",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new
                        {
                            returnurl = returnurl,
                            r = r,
                            ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                            cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                            Message = "خطایی رخ داد.",
                            url = !String.IsNullOrEmpty(returnurl) ? returnurl : r != "" ? r : "/profile",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        returnurl = returnurl,
                        r = r,
                        ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                        cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                        Message = msg,
                        url = !String.IsNullOrEmpty(returnurl) ? returnurl : r != "" ? r : "/profile",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "edit", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // فقط برای مرحله‌ی «تکمیل نام و نام‌خانوادگی» که بلافاصله بعد از ثبت‌نام و تایید کد
        // فعال‌سازی (فرانت‌اند: login-flow.tsx) نمایش داده میشه؛ برخلاف EditStep1 فقط همین دو
        // فیلد رو می‌گیره و ذخیره می‌کنه، نه کل پروفایل رو.
        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<JsonResult> CompleteRegistrationName(string FirstName, string LastName)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { statusCode = 401, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrWhiteSpace(FirstName))
                    return Json(new { statusCode = 400, Message = "نام را وارد کنید." }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrWhiteSpace(LastName))
                    return Json(new { statusCode = 400, Message = "نام خانوادگی را وارد کنید." }, JsonRequestBehavior.AllowGet);

                var user = UserManager.FindById(userId);
                if (user == null)
                    return Json(new { statusCode = 404, Message = "کاربر یافت نشد." }, JsonRequestBehavior.AllowGet);

                user.FirstName = FirstName.Trim();
                user.LastName = LastName.Trim();

                IdentityResult ir = await UserManager.UpdateAsync(user);
                if (ir.Succeeded)
                {
                    return Json(new { statusCode = 200, Message = "اطلاعات با موفقیت ذخیره شد." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { statusCode = 500, Message = string.Join(" ", ir.Errors) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "edit", false, 500, ex.Message, DateTime.Now, "c22d4e8c-5e3e-4c69-96f2-1f9d7e9c7d95");
                return Json(new { statusCode = 500, Message = "خطایی رخ داد." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<JsonResult> EditStep1(EditProfileViewModelStep1 model, HttpPostedFileBase file, string ShamsiBirthDate, string r, string returnurl)
        {
            try
            {
                int ProvinceId = 8;


                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var userinfo = UserManager.FindById(userId);

                string msg = "";
                bool IsValid = true;
                string attachementId = "";

                if (model.Gender == null)
                {
                    msg += "جنسیت انتخاب نشده است !";
                    IsValid = false;
                }

                if (file != null)
                {
                    if (IsImage(file))
                    {
                        if ((file.ContentLength / 1024) <= 150)
                        {

                            string extention = file.FileName.Substring(file.FileName.LastIndexOf("."));
                            var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                            if (oFiletype != null)//Check Extension is Valid?
                            {
                                try
                                {
                                    attachment NewAttachement = new attachment();
                                    NewAttachement.Capacity = file.ContentLength / 1024;
                                    NewAttachement.DisplaySort = 0;
                                    NewAttachement.FileName = userinfo.UserName + "/" + file.FileName;
                                    NewAttachement.FileTypeId = oFiletype.Id;
                                    NewAttachement.HasMultiSize = false;
                                    NewAttachement.HasWatermark = false;
                                    NewAttachement.InsertDate = DateTime.Now;
                                    NewAttachement.IsActive = true;
                                    NewAttachement.LanguageId = 1;
                                    NewAttachement.Title = " آواتارِ کاربرِ " + userinfo.UserName;
                                    NewAttachement.UpdateDate = DateTime.Now;
                                    NewAttachement.UseCount = 1;
                                    IdentityManager im = new IdentityManager();
                                    NewAttachement.UserId = userId;

                                    uow.AttachmentRepository.Insert(NewAttachement);
                                    uow.Save();

                                    attachementId = NewAttachement.Id.ToString();
                                    string fileName = "LG_" + NewAttachement.Id.ToString() + NewAttachement.FileType.FileTypeName;
                                    attachment upAttachement = uow.AttachmentRepository.GetByID(NewAttachement.Id);
                                    upAttachement.FileName = userinfo.UserName + "/" + fileName;
                                    uow.Save();

                                    if (!System.IO.Directory.Exists(Server.MapPath("~/Content/UploadFiles/" + userinfo.UserName)))
                                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/UploadFiles/" + userinfo.UserName));

                                    string targetFolder = Server.MapPath("~/Content/UploadFiles/" + userinfo.UserName);
                                    string targetPath = System.IO.Path.Combine(targetFolder, fileName);
                                    file.SaveAs(targetPath);
                                }
                                catch (Exception x)
                                {
                                    msg = x.Message;
                                    IsValid = false;
                                }
                            }
                            else
                            {
                                msg = " پسوند فایل مجاز نیست. ";
                                IsValid = false;
                            }
                        }
                        else
                        {
                            msg = "حجم فایل باید کمتر از 150 کیلوبایت باشد.";
                            IsValid = false;
                        }
                    }
                    else
                    {
                        msg = " فایل انتخاب شده باید .jpg،.png،.jpeg یا .gif باشد . ";
                        IsValid = false;
                    }
                }
                if (IsValid)
                {

                    if (uow.UserRepository.Any(x => x.Id, x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id))
                    {
                        return Json(new
                        {
                            statusCode = 500,
                            Message = "تلفن همراه وارد شده تکراری است."
                        }, JsonRequestBehavior.AllowGet);

                    }
                    var user = UserManager.FindById(userId);
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    user.Gender = model.Gender;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.About = model.About;
                    user.LandlinePhone = model.LandlinePhone;
                    if (ShamsiBirthDate != "")
                    {
                        user.BirthDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ShamsiBirthDate);
                    }
                    if (file != null)
                    {
                        //delete old file
                        if (user.Avatar != null)
                        {
                            try
                            {
                                attachment OldAttachement = uow.AttachmentRepository.GetByID(user.Avatar);
                                if (OldAttachement != null)
                                {
                                    //Delete File
                                    string OldFileName = Server.MapPath("~/Content/UploadFiles/" + OldAttachement.FileName);
                                    //Delete Row
                                    uow.AttachmentRepository.Delete(OldAttachement);
                                    uow.Save();

                                    if (System.IO.File.Exists(OldFileName))
                                        System.IO.File.Delete(OldFileName);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        //update user avatart Id
                        Guid g = new Guid(attachementId);
                        user.Avatar = g;

                    }
                    IdentityResult ir = await UserManager.UpdateAsync(user);
                    if (ir.Succeeded)
                    {

                        if (!String.IsNullOrEmpty(returnurl))
                        {
                            if (!await uow.UserAddressRepository.UserHasAnyAddress(userId))
                            {

                                return Json(new
                                {
                                    returnurl = returnurl,
                                    r = r,
                                    ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                    cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", 304),
                                    Url = (returnurl == "1" ? "/profile/addresses" : returnurl),
                                    // Url = returnurl,
                                    //Url = (returnurl == "1" ? "/profile/addresses" : "/profile/addresses?returnurl=" + returnurl),
                                    //Message = "ثبت شد . جهت ثبت اطلاعات آدرس به صفحه آدرسها منتقل می شوید.",
                                    statusCode = 201,
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    returnurl = returnurl,
                                    r = r,
                                    ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                    cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", 304),
                                    Url = (returnurl == "1" ? "/profile/addresses" : returnurl),
                                    //Url = returnurl,
                                    statusCode = 201,
                                }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else if (r != "")
                        {
                            if (!await uow.UserAddressRepository.UserHasAnyAddress(userId))
                            {

                                return Json(new
                                {
                                    returnurl = returnurl,
                                    r = r,
                                    ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                    cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", 304),
                                    Url = (returnurl == "1" ? "/profile/addresses" : returnurl),
                                    //Url = returnurl,
                                    //Url = (returnurl == "1" ? "/profile/addresses" : "/profile/addresses?returnurl=" + returnurl),
                                    //Message = "ثبت شد . جهت ثبت اطلاعات آدرس به صفحه آدرسها منتقل می شوید.",
                                    statusCode = 201,
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    returnurl = returnurl,
                                    r = r,
                                    ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                    cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", 304),
                                    Url = r,
                                    statusCode = 201,
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {

                            return Json(new
                            {
                                returnurl = returnurl,
                                r = r,
                                ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", 304),
                                Url = "/profile",
                                statusCode = 200,
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        ViewBag.ReturnUrl = r;
                        return Json(new
                        {
                            Message = "خطایی رخ داد.",
                            statusCode = 500,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ViewBag.ReturnUrl = r;
                    return Json(new
                    {
                        Message = msg,
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "edit", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<JsonResult> EditStep2(EditProfileViewModelStep2 model, string r, string returnurl)
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var userinfo = UserManager.FindById(userId);

                int ProvinceId = 8;
                if (model.CityId.HasValue)
                    ProvinceId = uow.CityRepository.Get(x => x.ProvinceId, x => x.Id == model.CityId).First();

                model.CityEntity = uow.CityRepository.GetByID(model.CityId);



                string msg = "";
                bool IsValid = true;

                if (!model.CityId.HasValue)
                {
                    msg = "شهر انتخاب نشده است ! ";
                    IsValid = false;
                }


                if (IsValid)
                {

                    //if (uow.UserRepository.Get(x => x, x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id).Any())
                    //{
                    //    return Json(new
                    //    {
                    //        statusCode = 500,
                    //        Message = "تلفن همراه وارد شده تکراری است."
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
                    var user = UserManager.FindById(userId);
                    // user.PhoneNumber = model.PhoneNumber;
                    user.PostalCode = model.PostalCode;
                    user.State = model.State;
                    user.City = model.City;
                    user.Address = model.Address;
                    user.AddressNumber = model.AddressNumber;
                    user.AddressUnit = model.AddressUnit;
                    user.CityId = model.CityId;
                    user.FooterGoogleMapLatitude = model.FooterGoogleMapLatitude;
                    user.FooterGoogleMapLongitude = model.FooterGoogleMapLongitude;
                    user.AddressTitle = model.AddressTitle;

                    IdentityResult ir = await UserManager.UpdateAsync(user);
                    if (!String.IsNullOrEmpty(returnurl))
                    {
                        if (!await uow.UserAddressRepository.UserHasAnyPersonalInfo(userId))
                        {

                            return Json(new
                            {
                                returnurl = returnurl,
                                r = r,
                                ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                                Url = (returnurl == "1" ? "/profile/edit" : "/profile/edit?returnurl=" + returnurl),
                                Message = "ثبت شد . جهت ثبت مشخصات فردی به صفحه ویرایش اطلاعات کاربری منتقل می شوید.",
                                statusCode = 201,
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Url = returnurl,
                                statusCode = 201,
                            }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (r != "")
                    {
                        if (!await uow.UserAddressRepository.UserHasAnyPersonalInfo(userId))
                        {

                            return Json(new
                            {
                                returnurl = returnurl,
                                r = r,
                                ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                                Url = (returnurl == "1" ? "/profile/edit" : "/profile/edit?returnurl=" + returnurl),
                                Message = "ثبت شد . جهت ثبت مشخصات فردی به صفحه ویرایش اطلاعات کاربری منتقل می شوید.",
                                statusCode = 201,
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                returnurl = returnurl,
                                r = r,
                                ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                                cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                                Url = r,
                                statusCode = 201,
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        return Json(new
                        {
                            returnurl = returnurl,
                            r = r,
                            ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId),
                            cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304),
                            Url = "/profile",
                            statusCode = 200,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    ViewBag.ReturnUrl = r;
                    return Json(new
                    {
                        Message = "خطایی رخ داد.",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "edit", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<JsonResult> EditStep3(EditProfileViewModelStep3 model, string r, string returnurl)
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var userinfo = UserManager.FindById(userId);

                string msg = "";
                bool IsValid = true;

                if (IsValid)
                {

                    if (uow.UserRepository.Get(x => x, x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id).Any())
                    {
                        return Json(new
                        {
                            statusCode = 500,
                            Message = "تلفن همراه وارد شده تکراری است."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var user = UserManager.FindById(userId);
                    user.PhoneNumber = model.PhoneNumber;
                    user.CardNumber = model.CardNumber;
                    user.AccountNumber = model.AccountNumber;
                    user.SHBNNumber = model.SHBNNumber;

                    IdentityResult ir = await UserManager.UpdateAsync(user);
                    if (!String.IsNullOrEmpty(returnurl))
                    {
                        return Json(new
                        {
                            returnurl = returnurl,
                            r = r,
                            Url = (returnurl == "1" ? "/profile/edit" : returnurl),
                            statusCode = 201,
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else if (r != "")
                    {
                        return Json(new
                        {
                            returnurl = returnurl,
                            r = r,
                            Url = (returnurl == "1" ? "/profile/edit" : returnurl),
                            statusCode = 201,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        return Json(new
                        {
                            returnurl = returnurl,
                            r = r,
                            Url = "/profile",
                            statusCode = 200,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    return Json(new
                    {
                        Message = "خطایی رخ داد.",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "edit", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [JWTAuthorize]
        public virtual async Task<JsonResult> ChangePasswordAjax(ChangePasswordViewModel model)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var userinfo = UserManager.FindById(userId);
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        Message = "خطایی رخ داد.",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }
                var token = await UserManager.GeneratePasswordResetTokenAsync(userId);

                var result = await UserManager.ResetPasswordAsync(userId, token, model.NewPassword);

                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return Json(new
                    {
                        Message = "رمز عبور تغییر یافت.",
                        statusCode = 200,
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "edit", "profile", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region Favorates

        [JWTAuthorize]
        public JsonResult favorites(int? page)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            try
            {
                var Products = uow.ProductRepository.ProductItemList(x => x.ProductFavorates.Any(s => s.UserId == userId)).ToList();

                int pageSize = 2;

                int pageNumber = (page ?? 1);
                var data = Products.ToPagedList(pageNumber, pageSize);

                return Json(new
                {
                    data = data,
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = data.TotalItemCount,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "favorites", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        [HttpPost]
        public async Task<JsonResult> RemoveFavorate(int? id)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                if (id.HasValue)
                {
                    int? pf = uow.ProductFavorateRepository.checkUserProductFavorate(id.Value, userId);
                    if (pf.HasValue)
                    {
                        await uow.ProductFavorateRepository.removeFavorate(pf.Value);
                        await uow.ProductRepository.removeProductFavorate(id.Value);
                        return Json(new
                        {
                            statusCode = 200,
                            Message = "حذف شد "
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 500,
                            Message = "محصول شما پیدا نشد !"
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "محصولی انتخاب نشده است ! "
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "RemoveFavorate", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region Notices

        [JWTAuthorize]
        public ActionResult Notices(int? page)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                var ProductLetmeknows = uow.ProductLetmeknowRepository.GetList(userId).ToList().Select(x => new
                {
                    title = x.Product.Name,
                    ProductId = x.ProductId,
                    ProductPriceId = x.ProductPriceId,
                    PageAddress = x.Product.PageAddress,
                    Descr = x.Product.Descr,
                    Available = x.Available,
                    AmazingOffer = x.AmazingOffer,
                    NotificationType = x.NotificationType,
                    ProductAttributeSelectModel = x.ProductPrice.ProductAttributeSelectModelId.HasValue ? x.ProductPrice.ProductAttributeSelectModel.Value : "",
                    ProductAttributeSelectSize = x.ProductPrice.ProductAttributeSelectSizeId.HasValue ? x.ProductPrice.ProductAttributeSelectSize.Value : "",
                    ProductAttributeSelectColor = x.ProductPrice.ProductAttributeSelectColorId.HasValue ? uow.context.ProductAttributeItemColors.Where(b => b.Id == int.Parse(x.ProductPrice.ProductAttributeSelectColor.Value)).First().Color : "",
                    Unit = x.ProductPrice.ProductAttributeSelectSizeId.HasValue ? x.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "",
                    ImageFileName =
                        x.ProductPrice.ProductImages.Any()
                            ? (
                                x.ProductPrice.ProductImages.Any(i => i.IsMain)
                                    ? x.ProductPrice.ProductImages
                                        .Where(i => i.IsMain)
                                        .Select(i => i.Image.FileName)
                                        .FirstOrDefault()
                                    : x.ProductPrice.ProductImages
                                        .Select(i => i.Image.FileName)
                                        .FirstOrDefault()
                              )
                            : (
                                x.Product.ProductPrices.Any(p => p.ProductImages.Any())
                                    ? x.Product.ProductPrices
                                        .Where(p => p.ProductImages.Any())
                                        .SelectMany(p => p.ProductImages)
                                        .Select(i => i.Image.FileName)
                                        .FirstOrDefault()
                                    : null
                              )

                }).ToPagedList(pageNumber, pageSize);


                return Json(new
                {
                    data = ProductLetmeknows,
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = ProductLetmeknows.TotalItemCount,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "Notices", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        [HttpPost]
        public async Task<JsonResult> RemoveNotice(int? id)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                if (id.HasValue)
                {
                    int? pf = uow.ProductLetmeknowRepository.checkUserProductletmeKnow(id.Value, userId);
                    if (pf.HasValue)
                    {
                        await uow.ProductLetmeknowRepository.removeLetmeKnow(pf.Value);
                        return Json(new
                        {
                            statusCode = 200,
                            Message = "حذف شد "
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 500,
                            Message = "محصول شما پیدا نشد !"
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "محصولی انتخاب نشده است ! "
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "RemoveNotice", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region ProductComments

        [JWTAuthorize]
        public ActionResult Comments(int? page, int? tab)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var waitingProducts = uow.ProductCommentRepository.GetNoUserCommenProducts(userId).ToPagedList(pageNumber, pageSize);
                var myComments = uow.ProductCommentRepository.GetUserComments(userId).ToPagedList(pageNumber, pageSize);

                return Json(new
                {
                    tab = tab,
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = waitingProducts.TotalItemCount,
                    data = waitingProducts,
                    totalcountComments = myComments.TotalItemCount,
                    Productcomments = myComments,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "Comments", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        [HttpPost]
        public async Task<JsonResult> RemoveComment(int? id)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                if (id.HasValue)
                {
                    int? pf = uow.ProductCommentRepository.checkUserProductComment(id.Value, userId);
                    if (pf.HasValue)
                    {
                        await uow.ProductCommentRepository.removeProductComment(pf.Value);
                        return Json(new
                        {
                            statusCode = 200,
                            Message = "حذف شد "
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 500,
                            Message = "نظر شما پیدا نشد !"
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "نظر انتخاب نشده است ! "
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "RemoveComment", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Addresses

        [JWTAuthorize]
        public async Task<ActionResult> addresses(string returnurl, string m, string r)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            try
            {
                var user = uow.UserRepository.Get(x => x, x => x.Id == userId, null, "CityEntity,Avatarattachment").First();

                var UserAddress = uow.UserAddressRepository.Get(x => x, x => x.UserId == userId, null, "CityEntity.Province");

                var step2 = new EditProfileViewModelStep2V2
                {
                    Id = userId,
                    FullName = user.FirstName + " " + user.LastName,
                    City = user.CityId.HasValue ? user.CityEntity.Name : "",
                    ProvinceId = user.CityId.HasValue ? user.CityEntity.ProvinceId : 8,
                    CityId = user.CityId,
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    PostalCode = user.PostalCode,
                    State = user.State,
                    Address = user.Address,
                    AddressNumber = user.AddressNumber,
                    AddressUnit = user.AddressUnit,
                    FooterGoogleMapLongitude = user.FooterGoogleMapLongitude,
                    FooterGoogleMapLatitude = user.FooterGoogleMapLatitude,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    AddressTitle = user.AddressTitle

                };
                //  var data = UserAddress.Select(x => new UserAddressV2() { Id = x.Id, Address = x.Address, AddressNumber = x.AddressNumber, AddressUnit = x.AddressUnit, CityId = x.CityId, provinceId = x.CityId.HasValue ? x.CityEntity.ProvinceId : 8, CityName = x.CityId.HasValue ? x.CityEntity.Name : "", FooterGoogleMapLatitude = x.FooterGoogleMapLatitude, FooterGoogleMapLongitude = x.FooterGoogleMapLongitude, FullName = x.FullName, PostalCode = x.PostalCode, ProvinceName = x.CityId.HasValue ? x.CityEntity.Province.Name : "", Title = x.Title }).OrderByDescending(x => x.Id).ToList();

                return Json(new
                {
                    r = r,
                    m = !String.IsNullOrEmpty(m) ? m : null,
                    EditProfileViewModelStep2 = step2,
                    returnurl = returnurl,
                    ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => new { x.Id, x.Name }), "Id", "Name", 8),
                    cityList = new SelectList(uow.CityRepository.Get(x => new { x.Id, x.Name }, x => x.ProvinceId == 8), "Id", "Name", 304),
                    user = new { user.Id, user.FirstName, user.LastName, user.PhoneNumber },
                    data = UserAddress.Select(x => new UserAddressV2() { Id = x.Id, PhoneNumber = x.PhoneNumber, Address = x.Address, AddressNumber = x.AddressNumber, AddressUnit = x.AddressUnit, CityId = x.CityId, provinceId = x.CityId.HasValue ? x.CityEntity.ProvinceId : 8, CityName = x.CityId.HasValue ? x.CityEntity.Name : "", FooterGoogleMapLatitude = x.FooterGoogleMapLatitude, FooterGoogleMapLongitude = x.FooterGoogleMapLongitude, FullName = x.FullName, PostalCode = x.PostalCode, ProvinceName = x.CityId.HasValue ? x.CityEntity.Province.Name : "", Title = x.Title }).OrderByDescending(x => x.Id).ToList(),
                    ProvinceList2 = step2.CityId.HasValue ? new SelectList(uow.ProvinceRepository.Get(x => new { x.Id, x.Name }), "Id", "Name", step2.ProvinceId) : null,
                    cityList2 = new SelectList(uow.CityRepository.Get(x => new { x.Id, x.Name }, x => x.ProvinceId == step2.ProvinceId), "Id", "Name", step2.CityId),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "addresses", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> RemoveAddress(int? id)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (id.HasValue)
                {
                    int? pf = uow.UserAddressRepository.checkUserUserAddress(id.Value, userId);
                    if (pf.HasValue)
                    {
                        await uow.UserAddressRepository.removeUserAddress(pf.Value);
                        return Json(new
                        {
                            statusCode = 200,
                            Message = "حذف شد "
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 500,
                            Message = "آدرس شما پیدا نشد !"
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "آدرس انتخاب نشده است ! "
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "RemoveAddress", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [JWTAuthorize]
        public JsonResult AddAddress(string returnurl)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name"),
                    returnurl = returnurl,
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "RemoveAddress", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> AddAddress(UserAddress useraddress, string returnurl)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (!useraddress.CityId.HasValue)
                {
                    return Json(new
                    {
                        error = " شهر انتخاب نشده است ! ",
                        data = useraddress,
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                useraddress.UserId = userId;
                await uow.UserAddressRepository.addUserAddress(useraddress);
                if (!String.IsNullOrEmpty(returnurl))
                    return Json(new
                    {
                        url = "/" + returnurl + "?aid=" + useraddress.Id,
                        statusCode = 200,
                    }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new
                    {
                        url = "/profile/addresses",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "AddAddress", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> AddAddressAjax(UserAddress useraddress, string returnurl)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            try
            {
                if (!useraddress.CityId.HasValue)
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "شهر انتخاب نشده است ! "
                    }, JsonRequestBehavior.AllowGet);
                }
                useraddress.UserId = userId;
                await uow.UserAddressRepository.addUserAddress(useraddress);


                if (!String.IsNullOrEmpty(returnurl))
                {
                    return Json(new
                    {
                        Url = returnurl + "?aid=" + useraddress.Id,
                        Message = "ثبت شد . در حال انتقال به صفحه ای که بودید...",
                        statusCode = 201,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string userid = useraddress.UserId;
                    var user = uow.UserRepository.Get(x => x, x => x.Id == userid, null, "CityEntity,Avatarattachment").First();
                    var step2 = new EditProfileViewModelStep2V2
                    {
                        Id = userid,
                        CityId = user.CityId,
                        PhoneNumber = await UserManager.GetPhoneNumberAsync(userid),
                        PostalCode = user.PostalCode,
                        State = user.State,
                        City = user.CityId.HasValue ? user.CityEntity.Name : "",
                        Address = user.Address,
                        AddressNumber = user.AddressNumber,
                        AddressUnit = user.AddressUnit,
                        FooterGoogleMapLongitude = user.FooterGoogleMapLongitude,
                        FooterGoogleMapLatitude = user.FooterGoogleMapLatitude,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed

                    };
                    return Json(new
                    {
                        user = new EditAddressUser() { firstName = user.FirstName, LastName = user.LastName },
                        editProfileViewModelStep2 = step2,
                        data = uow.UserAddressRepository.GetQueryList().AsNoTracking().Include("CityEntity.Province").Where(x => x.UserId == useraddress.UserId).OrderByDescending(x => x.Id).Select(x => new EditAddressViewModel() { Id = x.Id, Title = x.Title, City = x.CityEntity.Name, FullName = x.FullName, AddressNumber = x.AddressNumber, AddressUnit = x.AddressUnit }).ToList(),
                        Message = "ثبت شد",
                        statusCode = 200,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "AddAddressAjax", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [JWTAuthorize]
        public JsonResult EditAddress(int? id, string returnurl)
        {
            try
            {
                string token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(token);

                if (userId == null)
                {
                    return Json(new
                    {
                        statusCode = 401,
                        Message = "نشست کاربری منقضی شده است."
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!id.HasValue)
                {
                    return Json(new
                    {
                        statusCode = 400,
                        Message = "آدرس انتخاب نشده است."
                    }, JsonRequestBehavior.AllowGet);
                }

                var useraddress = uow.UserAddressRepository.Get(
                    x => x,
                    x => x.Id == id.Value && x.UserId == userId,
                    null,
                    "CityEntity.Province"
                ).FirstOrDefault();

                if (useraddress == null)
                {
                    return Json(new
                    {
                        url = "/profile/addresses",
                        statusCode = 404,
                        Message = "آدرس موردنظر پیدا نشد."
                    }, JsonRequestBehavior.AllowGet);
                }

                int provinceId = useraddress.CityId.HasValue
                    ? useraddress.CityEntity.ProvinceId
                    : 8;

                return Json(new
                {
                    returnurl = returnurl,

                    // اطلاعات کامل همان آدرسی که باید ویرایش شود
                    data = new
                    {
                        useraddress.Id,
                        useraddress.Title,
                        useraddress.FullName,
                        useraddress.PhoneNumber,
                        useraddress.CityId,
                        useraddress.Address,
                        useraddress.AddressNumber,
                        useraddress.AddressUnit,
                        useraddress.PostalCode,
                        useraddress.FooterGoogleMapLatitude,
                        useraddress.FooterGoogleMapLongitude,

                        // برای راحتی فرانت‌اند
                        ProvinceId = provinceId,
                        CityName = useraddress.CityEntity?.Name,
                        ProvinceName = useraddress.CityEntity?.Province?.Name
                    },

                    ProvinceList = uow.ProvinceRepository
                        .Get(x => new { x.Id, x.Name })
                        .Select(x => new
                        {
                            x.Id,
                            x.Name
                        })
                        .ToList(),

                    cityList = uow.CityRepository
                        .Get(
                            x => new { x.Id, x.Name },
                            x => x.ProvinceId == provinceId
                        )
                        .Select(x => new
                        {
                            x.Id,
                            x.Name
                        })
                        .ToList(),

                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(
                    5,
                    "api",
                    "EditAddress",
                    false,
                    500,
                    ex.Message,
                    DateTime.Now,
                    "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84"
                );

                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [JWTAuthorize]
        public async Task<ActionResult> EditAddress(UserAddress useraddress, string returnurl)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!", ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name") }, JsonRequestBehavior.AllowGet);
            try
            {

                if (!useraddress.CityId.HasValue)
                {
                    return Json(new
                    {
                        Message = "شهر انتخاب نشده است!",
                        ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name"),
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                useraddress.UserId = userId;
                await uow.UserAddressRepository.EditUserAddress(useraddress);
                // معادل CartController.EditAddress(POST): return Redirect("~" + returnurl) — یعنی
                // returnurl عیناً (بدون افزودن ?aid) استفاده می‌شه، چون لینکی که کاربر رو به این
                // صفحه آورده از قبل ?aid رو توی خودِ returnurl جاسازی کرده (نه اینجا اضافه بشه).
                // returnurl از فرانت همیشه با / شروع می‌شه (مثل AddAddressAjax که مستقیم استفاده‌ش
                // می‌کنه)، پس این‌جا هم چیزی بهش اضافه نمی‌کنیم که به // دوتایی نرسیم
                string url = String.IsNullOrEmpty(returnurl) ? "/profile/addresses" : returnurl;
                return Json(new
                {
                    Url = url,
                    Message = "ویرایش شد",
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "EditAddress", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }


        public virtual JsonResult GetCities(int? ProvinceId)
        {
            if (!ProvinceId.HasValue)
            {
                var jsonResult = Json(new
                {
                    data = "",
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                var jsonResult = Json(new
                {
                    data = uow.CityRepository.GetByReturnQueryable(x => x, x => x.ProvinceId == ProvinceId, x => x.OrderBy(s => s.Name), "", 0, 0).Select(x => new { x.Id, x.Name }).ToList(),
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        #endregion

        #region Bon
        [JWTAuthorize]
        public JsonResult GiftBon(int? page, int? tab)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            try
            {
                var UserBons = uow.UserBonRepository.GetQueryList().Include("UserBonLogs").AsNoTracking().Where(x => x.UserId == userId);
                int sumvalue = UserBons.Sum(x => x.Value);
                int sumUsedvalue = UserBons.Sum(x => x.UsedValue);
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                var data = UserBons.Select(x => new
                {
                    x.Id,
                    InsertDate = x.InsertDate,
                    ExpireDate = x.ExpireDate,
                    Value = x.Value,
                    UsedValue = x.UsedValue,
                    UserBonLogs = x.UserBonLogs.Select(a => new { a.InsertDate, BankOrderId = a.Order.BankOrderId, Value = a.Value })
                }).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);
                return Json(new
                {
                    sumvalue = sumvalue,
                    sumUsedvalue = sumUsedvalue,
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = data.TotalItemCount,
                    data = data,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "GiftBon", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GiftCode

        [JWTAuthorize]
        public JsonResult GiftCode(int? page, int? tab)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            try
            {

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var data = uow.UserCodeGiftRepository.GetQueryList().Include("UserCodeGiftLogs.Order").Include("Offer").AsNoTracking().Where(x => x.UserId == userId)
              .Select(a => new
              {
                  a.Id,
                  Title = a.Offer.Title,
                  InsertDate = a.Offer.InsertDate,
                  ExpireDate = a.ExpireDate,
                  OfferExpireDate = a.Offer.ExpireDate,
                  MaxValue = a.MaxValue,
                  Code = a.Code,
                  CountUse = a.CountUse,
                  IsActive = a.IsActive,
                  OfferIsActive = a.Offer.IsActive,
                  Offerstate = a.Offer.state,
                  UserCodeGiftLogs = a.UserCodeGiftLogs.Select(b => new { b.Id, b.InsertDate, CustomerOrderId = b.OrderId.HasValue ? b.Order.CustomerOrderId : "", Value = b.Value })

              }).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);

                return Json(new
                {
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = data.TotalItemCount,
                    data = data,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "GiftBon", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Messages

        [JWTAuthorize]
        public JsonResult Messages(int? page, int? tab)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var UserMessages = uow.UserMessageRepository.GetQueryList().AsNoTracking().Where(x => x.UserIdTo == userId || x.UserIdTo == null || x.UserId == "").Select(x => new
                {
                    x.Id,
                    x.UserIdTo,
                    x.Title,
                    x.InsertDate,
                    x.Text,
                    x.state
                }).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);


                return Json(new
                {
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = UserMessages.TotalItemCount,
                    data = UserMessages,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "GiftBon", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [JWTAuthorize]
        [HttpPost]
        public JsonResult ReadMessage(int? id)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            if (id.HasValue)
            {
                var um = uow.UserMessageRepository.Get(x => x, x => x.Id == id.Value && x.UserIdTo == userId).FirstOrDefault();
                if (um != null)
                {
                    um.state = true;
                    uow.UserMessageRepository.Update(um);
                    uow.Save();
                    return Json(new
                    {
                        statusCode = 200,
                        Message = " بروز شد "
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "پیام شما پیدا نشد !"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new
                {
                    statusCode = 500,
                    Message = "پیام انتخاب نشده است ! "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region orderGifts
        [JWTAuthorize]
        public JsonResult orderGifts(int? page, int? tab)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                // فقط فیلدهایی که در Views/profile/orderGifts.cshtml استفاده می‌شوند پروجکت می‌شوند
                // تا از circular reference هنگام سریالایز شدن گراف کامل موجودیت جلوگیری شود.
                var data = uow.ProductGiftPackageOrderRepository.GetQueryList().AsNoTracking()
                    .Where(x => x.Order.UserId == userId)
                    .Select(x => new
                    {
                        x.Id,
                        InsertDate = x.InsertDate,
                        Title = x.ProductGiftPackage.Title,
                        HasCover = x.ProductGiftPackage.Cover.HasValue,
                        AttachmentFileName = x.ProductGiftPackage.attachment.FileName,
                        AttachmentTitle = x.ProductGiftPackage.attachment.Title,
                        CustomerOrderId = x.Order.CustomerOrderId
                    }).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);

                return Json(new
                {
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    totalcount = data.TotalItemCount,
                    data = data,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "OrderGifts", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Orders

        //[Infrastructure.Filter.AutoExecueFilter]

        [JWTAuthorize]
        public JsonResult Orders(int? page, int? tab, string datee, string dateeFilter, string keyword, string keywordFilter, string sort, int? state)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (string.IsNullOrEmpty(datee))
                    datee = dateeFilter;
                else
                    page = 1;

                if (string.IsNullOrEmpty(keyword))
                    keyword = keywordFilter;
                else
                    page = 1;

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                int tabId = tab ?? 1;

                // فقط تب فعلی fetch می‌شه (نه هر ۳ تا با هم) تا کاربر با کلیک روی هر تب فقط همون
                // یکی رو از سرور بگیره؛ بار سنگین صفحه‌بندی/Include هم داخل GetXxxOrderV2 حل شده
                // (نگاه کن به توضیحات #region OrderV2 pagination helpers توی OrderService.cs).
                // سه‌تبِ جدید (جایگزینِ ۴ تبِ قدیمیِ همه/در حال انجام/مرجوعی/لغو شده):
                //   1 = جاری، 2 = بسته‌شده، 3 = پرداخت‌نشده (رجوع کنید به توضیحاتِ Get*OrdersV2 توی
                //   OrderService.cs برای تعریفِ دقیقِ هر سه)
                PagedList.IPagedList<OrderV2> orders;
                switch (tabId)
                {
                    case 2:
                        orders = uow.OrderRepository.GetClosedOrdersV2(userId, datee, keyword, pageNumber, pageSize, sort, state);
                        break;
                    case 3:
                        orders = uow.OrderRepository.GetUnpaidOrdersV2(userId, datee, keyword, pageNumber, pageSize, sort, state);
                        break;
                    default:
                        tabId = 1;
                        orders = uow.OrderRepository.GetInProgressOrdersV2(userId, datee, keyword, pageNumber, pageSize, sort, state);
                        break;
                }

                return Json(new
                {
                    tab = tabId,
                    sort = sort,
                    state = state,
                    pageSize = pageSize,
                    pageNumber = pageNumber,
                    datee = datee,
                    keyword = keyword,
                    Orders = orders,
                    TotalCount = orders.TotalItemCount,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "Orders", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Infrastructure.Filter.AutoExecueFilter]
        [JWTAuthorize]
        public JsonResult Detail(int id)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                string userid = userId;

                // مثل رفع تایم‌اوت Orders: قبلاً یه کوئری با ~۳۰ تا Include از Collectionهای نامرتبط
                // (OrderDeliveries + ChildOrders + ParentOrder + OrderWallets + OrderAttributeSelects + ...)
                // با هم می‌اومد که باعث ضرب کارتزین و کند شدن شدید صفحه می‌شد. الان به دو کوئری سبک‌تر
                // تقسیم شده: یکی برای خودِ سفارش (بدون مرسوله‌ها)، یکی جدا برای مرسوله‌ها/ردیف‌ها.
                var order = uow.OrderRepository.GetQueryList().AsNoTracking()
                    .Include("OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute")
                    .Include("OrderWallets.Wallet.BankAccount")
                    .Include("OrderAttributeSelects.OrderAttribute")
                    .Include("OrderStates")
                    .Include("User.CityEntity.Province")
                    .Where(x => x.CustomerOrderId == id.ToString() && x.UserId == userid)
                    .SingleOrDefault();

                if (order == null)
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                bool IsEstelam = false;
                if (order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.Value.ToLower() == "false" && a.WalletAttribute.DataType == 23)) && (order.OrderStates.Max(s => s.state) == OrderStatus.در_انتظار_تایید || order.OrderStates.Max(s => s.state) == OrderStatus.تایید_سفارش))
                {
                    IsEstelam = true;
                }

                var setting = GetSetting();

                var deliveries = uow.OrderDeliveryRepository.GetQueryList().AsNoTracking()
                    .Include("ProductSendWayWorkTime")
                    .Include("AdminSelectedSendway")
                    .Include("ProductSendWay")
                    .Include("OrderStates")
                    .Include("WalletAttributeWallets.WalletAttribute")
                    .Include("UserAddress.CityEntity.Province")
                    .Include("OrderRows.Product")
                    .Include("OrderRows.ProductPrice.ProductImages.Image")
                    .Include("OrderRows.ProductPrice.ProductAttributeSelectModel")
                    .Include("OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute")
                    .Include("OrderRows.ProductPrice.ProductAttributeSelectColor")
                    .Include("OrderRows.Product.ProductPrices.ProductImages.Image")
                    .Where(d => d.OrderId == order.Id)
                    .ToList();

                var colorlist = uow.ProductAttributeItemColorRepository.GetQueryList().AsNoTracking().ToList();

                // چون همین mapping دقیقاً هم برای ردیف‌های هر مرسوله لازمه هم برای ردیف‌های سفارش‌های
                // فرزند (ChildOrders)، یه‌بار به‌صورت local function تعریف می‌شه.
                Func<OrderRow, OrderRowsDetailV2> mapRow = r => new OrderRowsDetailV2
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    PageAddress = r.Product.PageAddress,
                    Title = r.Product.Title,
                    Quantity = r.Quantity,
                    Price = r.Price,
                    Disabled = r.Disabled,

                    ProductAttributeSelectModelId = r.ProductPrice.ProductAttributeSelectModelId ?? null,
                    ProductAttributeSelectModel = r.ProductPrice.ProductAttributeSelectModel != null ? r.ProductPrice.ProductAttributeSelectModel.Value : null,
                    ProductAttributeSelectSizeId = r.ProductPrice.ProductAttributeSelectSizeId ?? null,
                    ProductAttributeSelectSize = r.ProductPrice.ProductAttributeSelectSize != null ? r.ProductPrice.ProductAttributeSelectSize.Value : null,
                    Unit = r.ProductPrice.ProductAttributeSelectSize != null ? r.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : null,
                    ProductAttributeSelectColorId = r.ProductPrice.ProductAttributeSelectColorId ?? null,
                    ProductAttributeSelectColor = r.ProductPrice.ProductAttributeSelectColor != null
                        ? colorlist.Where(a => a.Id == Convert.ToInt32(r.ProductPrice.ProductAttributeSelectColor.Value)).First().Color
                        : "",
                    ImageFileName = r.ProductPrice.ProductImages.Any()
                        ? (r.ProductPrice.ProductImages.Any(i => i.IsMain)
                            ? r.ProductPrice.ProductImages.Where(i => i.IsMain).Select(i => i.Image.FileName).FirstOrDefault()
                            : r.ProductPrice.ProductImages.Select(i => i.Image.FileName).FirstOrDefault())
                        : (r.Product.ProductPrices.Any(p => p.ProductImages.Any())
                            ? r.Product.ProductPrices.Where(p => p.ProductImages.Any()).SelectMany(p => p.ProductImages).Select(i => i.Image.FileName).FirstOrDefault()
                            : null)
                };

                // معادل: var child = Model.ChildOrders.FirstOrDefault(); while (child != null) { ...; child = child.ChildOrders.FirstOrDefault(); }
                var childOrderRows = new List<OrderRowsDetailV2>();
                Guid? currentParentId = order.Id;
                for (int depth = 0; depth < 10; depth++) // سقف برای اطمینان از عدم حلقه‌ی بی‌نهایت روی دیتای خراب
                {
                    var childOrder = uow.OrderRepository.GetQueryList().AsNoTracking()
                        .Include("OrderRows.Product")
                        .Include("OrderRows.ProductPrice.ProductImages.Image")
                        .Include("OrderRows.ProductPrice.ProductAttributeSelectModel")
                        .Include("OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute")
                        .Include("OrderRows.ProductPrice.ProductAttributeSelectColor")
                        .Include("OrderRows.Product.ProductPrices.ProductImages.Image")
                        .Where(x => x.OrderId == currentParentId)
                        .FirstOrDefault();

                    if (childOrder == null)
                        break;

                    childOrderRows.AddRange(childOrder.OrderRows.Select(mapRow));
                    currentParentId = childOrder.Id;
                }

                OrderDetailV2 detailV2 = new OrderDetailV2()
                {
                    BankOrderId = order.BankOrderId,
                    Commision = order.Commision,
                    CustomerOrderId = order.CustomerOrderId,
                    Id = order.Id,
                    InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(order.InsertDate),
                    BankId = order.OrderWallets.First().Wallet.BankAccount.BankId,
                    Price = order.OrderWallets.First().Wallet.Price,
                    IsEstelam = IsEstelam,
                    EstelamDeadlineRaw = order.InsertDate.AddMinutes(setting.ShoppingEstelamMinutes),
                    OrderAttributeSelects = order.OrderAttributeSelects.Select(a => new OrderAttributeSelectDetailV2
                    {
                        DataType = a.OrderAttribute.DataType,
                        Value = a.Value,
                        OrderDeliveryId = a.OrderDeliveryId
                    }).ToList(),
                    Orderstates = order.OrderStates.OrderBy(s => s.LogDate).Select(s => new OrderStateDetailV2
                    {
                        Id = s.Id,
                        state = s.state,
                        LogDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(s.LogDate) + " - " + s.LogDate.ToString("HH:mm")
                    }).ToList(),

                    ChildOrderRows = childOrderRows,

                    OrderDeliveries = deliveries.Select(d => new OrderDeliveryDetailV2
                    {
                        Id = d.Id,

                        SendWayId = d.ProductSendWayId,
                        PhoneNumber = d.UserAddressId.HasValue
        ? d.UserAddress.PhoneNumber
        : order.User.PhoneNumber,

                        FullName = d.UserAddressId.HasValue
        ? d.UserAddress.FullName
        : order.User.FirstName + " " + order.User.LastName,

                        Province = d.UserAddressId.HasValue
        ? d.UserAddress.CityEntity.Province.Name
        : (order.User.CityId.HasValue
            ? order.User.CityEntity.Province.Name
            : null),

                        City = d.UserAddressId.HasValue
        ? d.UserAddress.CityEntity.Name
        : (order.User.CityId.HasValue
            ? order.User.CityEntity.Name
            : null),

                        Address = !d.UserAddressId.HasValue ? order.User.Address : d.UserAddress.Address,
                        AddressUnit = !d.UserAddressId.HasValue ? (String.IsNullOrEmpty(order.User.AddressUnit) ? "" : " ، پلاک  " + order.User.AddressNumber) : (String.IsNullOrEmpty(d.UserAddress.AddressNumber) ? "" : " ، پلاک  " + d.UserAddress.AddressNumber),
                        AddressNumber = !d.UserAddressId.HasValue ? (String.IsNullOrEmpty(order.User.AddressUnit) ? "" : " ، پلاک  " + order.User.AddressUnit) : (String.IsNullOrEmpty(d.UserAddress.AddressUnit) ? "" : " ، پلاک  " + d.UserAddress.AddressUnit),
                        RequestDate = d.RequestDate,
                        PredictDate = d.PredictDate,
                        AdminSelectedSendwayId = d.AdminSelectedSendway != null ? d.AdminSelectedSendway.Id : 0,
                        AdminSelectedSendwayTitle =
                            d.AdminSelectedSendway != null
                                ? d.AdminSelectedSendway.Title
                                : null,

                        AdminSelectedSendwayTrackingUrl =
                            d.AdminSelectedSendway != null
                                ? d.AdminSelectedSendway.TrackingUrl
                                : null,

                        AdminSelectedSendwayTrackingCode =
                            d.AdminSelectedTrackCode,

                        ProductSendWayTitle = d.ProductSendWay != null ? d.ProductSendWay.Title : null,
                        ProductSendWayTrackingUrl = d.ProductSendWay != null ? d.ProductSendWay.TrackingUrl : null,

                        ProductSendWayWorkTime =
                            d.ProductSendWayWorkTime == null
                                ? null
                                : new ProductSendWayWorkTimeDetailV2
                                {
                                    StartTime = d.RequestDate.HasValue ? d.ProductSendWayWorkTime.StartTime.ToString(@"hh\:mm") : "",
                                    EndTime = d.RequestDate.HasValue ? d.ProductSendWayWorkTime.EndTime.ToString(@"hh\:mm") : ""
                                },

                        Orderstates = d.OrderStates
                            .OrderBy(x => x.LogDate)
                            .Select(x => new OrderStateDetailV2
                            {
                                Id = x.Id,
                                state = x.state,
                                LogDate = CoreLib.Infrastructure.DateTime.DateTimeConverter
                                    .ChangeMiladiToLongShamsi(x.LogDate)
                                    + " - " +
                                    x.LogDate.ToString("HH:mm")
                            }).ToList(),

                        WalletAttributeWalles = d.WalletAttributeWallets != null ? d.WalletAttributeWallets
                            .Select(w => new WalletAttributeWalletDetailV2
                            {
                                DataType = w.WalletAttribute.DataType
                            }).ToList() : null,

                        OrderRows = d.OrderRows.Select(mapRow).ToList(),
                    }).ToList()
                };
                return Json(new
                {
                    data = detailV2,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "Orders", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        public JsonResult invoice(string id)
        {
            try
            {
                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                if (String.IsNullOrEmpty(id))
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                Guid gid = new Guid(id);
                if (!uow.OrderStateRepository.Any(s => s.OrderId == gid && s.state >= OrderStatus.تایید_پرداخت))
                {

                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    orderId = id,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "invoice", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetFactorPdf(string id)
        {
            // ۱. بررسی هدر و توکن
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
            {
                Response.StatusCode = 401;
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            }

            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = 400;
                return Json(new { status = 400, Message = "شناسه سفارش ارسال نشده است." }, JsonRequestBehavior.AllowGet);
            }

            Guid gid = new Guid(id);
            if (!uow.OrderStateRepository.Any(s => s.OrderId == gid && s.state >= OrderStatus.تایید_پرداخت))
            {
                Response.StatusCode = 403;
                return Json(new { status = 403, Message = "سفارش معتبر نیست یا پرداخت نشده است." }, JsonRequestBehavior.AllowGet);
            }

            // ۲. واکشی اطلاعات تنظیمات و سفارش
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment,Province,City,FactorAttachment").SingleOrDefault();
            XMLReader readXml = new XMLReader(setting.StaticContentDomain);

            StiReport report = new StiReport();
            report.Load(Server.MapPath("~/Content/Reports/FactorReport.mrt"));

            var order = uow.OrderRepository.Get(x => new { x.CustomerOrderId, x.BankOrderId, x.InsertDate, x.OrderDeliveries.First().UserAddress, x.OrderDeliveries.First().UserAddressId, x.User, x.OrderRows, x.OrderWallets, ProductSendWay = x.OrderDeliveries.First().ProductSendWay.Title, x.OrderAttributeSelects }, x => x.Id == gid,
                    null, "OrderDeliveries.UserAddress.CityEntity.Province,OrderDeliveries.ProductSendWay,User.CityEntity.Province,OrderWallets.Wallet,OrderAttributeSelects.OrderAttribute,OrderRows,OrderRows.ProductPrice.Product,OrderRows.ProductPrice.ProductAttributeSelectColor,OrderRows.ProductPrice.ProductAttributeSelectSize,OrderRows.ProductPrice.ProductAttributeSelectModel,OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").First();

            Areas.Admin.ViewModels.Report.CustomerOrderInfo CustomerOrderInfo = new Areas.Admin.ViewModels.Report.CustomerOrderInfo()
            {
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(DateTime.Now),
                Logo = setting.FactorAttachment.FileName,
                SerialNumber = order.CustomerOrderId,
                ShoppingName = setting.WebSiteName,
                ShoppingProvience = setting.Province.Name,
                ShoppingCity = setting.City.Name,
                ShoppingAddress = setting.Address,
                ShoppingPostalCode = setting.PostalCode,
                ShoppingTele = setting.Tele,
                ShoppingTaxNumber = setting.TaxNumber,
                CustomerName = order.UserAddressId.HasValue ? order.UserAddress.FullName : order.User.FirstName + " " + order.User.LastName,
                CustomerProvience = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Province.Name : order.User.CityEntity.Province.Name,
                CustomerCity = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Name : order.User.CityEntity.Name,
                CustomerAddress = order.UserAddressId.HasValue ? string.Format("{0}{1}{2}", order.UserAddress.Address, (!String.IsNullOrEmpty(order.UserAddress.AddressNumber) ? " ، پلاک" + order.UserAddress.AddressNumber : ""), (!String.IsNullOrEmpty(order.UserAddress.AddressUnit) ? " ، واحد" + order.UserAddress.AddressUnit : "")) : string.Format("{0}{1}{2}", order.User.Address, (!String.IsNullOrEmpty(order.User.AddressNumber) ? " ، پلاک" + order.User.AddressNumber : ""), (!String.IsNullOrEmpty(order.User.AddressUnit) ? " ، واحد" + order.User.AddressUnit : "")),
                CustomerPostalCode = order.UserAddressId.HasValue ? order.UserAddress.PostalCode : order.User.PostalCode,
                CustomerTele = order.UserAddressId.HasValue ? order.UserAddress.PhoneNumber : order.User.PhoneNumber,
                CustomerTaxNumber = "",
                ShoppingOrderId = "TF-" + order.BankOrderId,
                ShoppingPayWay = order.OrderWallets.First().Wallet.PaymentType == 1 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 2 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 3 ? "کارت به کارت" : order.OrderWallets.First().Wallet.PaymentType == 4 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 5 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 6 ? "فیش نقدی" : "---",
                ShoppingSenWay = order.ProductSendWay,
                ShoppingUserDescr = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 18) ? order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 18).First().Value : "---",
                ShoppingSenWayPrice = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 14) ? Convert.ToInt64(order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 14).First().Value) * 10 : 0,
                ShoppingTotalPrice = order.OrderWallets.First().Wallet.Price * 10
            };

            var CustomerOrderRows = new List<Areas.Admin.ViewModels.Report.CustomerOrderRow>();
            int i = 1;
            foreach (var item in order.OrderRows)
            {
                string name = item.Product.Name;
                if (item.ProductPrice.ProductAttributeSelectModelId.HasValue)
                {
                    name += " مدل " + item.ProductPrice.ProductAttributeSelectModel.Value;
                }
                if (item.ProductPrice.ProductAttributeSelectSizeId.HasValue)
                    name += " سایز " + item.ProductPrice.ProductAttributeSelectSize.Value + item.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                if (item.ProductPrice.ProductAttributeSelectColorId.HasValue)
                    name += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(item.ProductPrice.ProductAttributeSelectColorId).Value)).Color;

                CustomerOrderRows.Add(new CustomerOrderRow()
                {
                    Id = i,
                    Code = item.ProductPrice.code,
                    Name = name,
                    RawPrice = item.RawPrice * 10,
                    SumPrice = (item.RawPrice * item.Quantity) * 10,
                    Quantity = item.Quantity,
                    TaxPrice = item.taxValue * 10,
                    OffPrice = Math.Abs(item.RawPrice - item.Price) * item.Quantity * 10,
                    Price = (item.RawPrice - (item.RawPrice - item.Price)) * item.Quantity * 10,
                    FinalPrice = ((item.Price * item.Quantity) + item.taxValue) * 10
                });
                i++;
            }

            var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + CustomerOrderInfo.Logo));
            byte[] array1 = imageToByteArray(img);
            MemoryStream ms = new MemoryStream(array1);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

            report.Dictionary.Variables.Add("Logo", image);
            report.RegBusinessObject("Order", CustomerOrderInfo);
            report.RegBusinessObject("CustomerOrderRow", CustomerOrderRows);

            // ۳. رندر کردن گزارش و استخراج به صورت PDF Stream
            report.Render(false);
            MemoryStream pdfStream = new MemoryStream();
            report.ExportDocument(StiExportFormat.Pdf, pdfStream);
            pdfStream.Position = 0;

            // ۴. بازگرداندن فایل با فرمت هماهنگ با دانلود مرورگر
            string fileName = $"factor-{order.CustomerOrderId}.pdf";
            return File(pdfStream, "application/pdf", fileName);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }

        [HttpGet]
        [JWTAuthorize]
        public ActionResult GetFactorReport(string id)
        {
            // ۱. اعتبارسنجی توکن
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(token);
            if (userId == null)
            {
                Response.StatusCode = 401;
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
            }

            // ۲. اعتبارسنجی شناسه سفارش
            Guid orderId;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out orderId))
            {
                Response.StatusCode = 400;
                return Json(new { status = 400, Message = "شناسه سفارش معتبر نیست." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment,Province,City,FactorAttachment").SingleOrDefault();
                XMLReader readXml = new XMLReader(setting.StaticContentDomain);

                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Content/Reports/FactorReport.mrt"));

                var order = uow.OrderRepository.Get(x => new { x.Commision, x.CustomerOrderId, x.BankOrderId, x.InsertDate, x.OrderDeliveries.First().UserAddress, x.OrderDeliveries.First().UserAddressId, x.User, x.OrderRows, x.OrderWallets, ProductSendWay = x.OrderDeliveries.First().ProductSendWay.Title, x.OrderAttributeSelects }, x => x.Id == orderId && x.UserId == userId,
                        null, "OrderDeliveries.UserAddress.CityEntity.Province,OrderDeliveries.ProductSendWay,User.CityEntity.Province,OrderWallets.Wallet,OrderAttributeSelects.OrderAttribute,OrderRows,OrderRows.ProductPrice.Product,OrderRows.ProductPrice.ProductAttributeSelectColor,OrderRows.ProductPrice.ProductAttributeSelectSize,OrderRows.ProductPrice.ProductAttributeSelectModel,OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").FirstOrDefault();

                if (order == null)
                {
                    Response.StatusCode = 404;
                    return Json(new { status = 404, Message = "سفارش یافت نشد." }, JsonRequestBehavior.AllowGet);
                }

                Areas.Admin.ViewModels.Report.CustomerOrderInfo CustomerOrderInfo = new Areas.Admin.ViewModels.Report.CustomerOrderInfo()
                {
                    InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(DateTime.Now),
                    Logo = setting.FactorAttachment.FileName,
                    SerialNumber = order.CustomerOrderId,
                    ShoppingName = setting.WebSiteName,
                    ShoppingProvience = setting.Province.Name,
                    ShoppingCity = setting.City.Name,
                    ShoppingAddress = setting.Address,
                    ShoppingPostalCode = setting.PostalCode,
                    ShoppingTele = setting.Tele,
                    ShoppingTaxNumber = setting.TaxNumber,
                    CustomerName = order.UserAddressId.HasValue ? order.UserAddress.FullName : order.User.FirstName + " " + order.User.LastName,
                    CustomerProvience = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Province.Name : order.User.CityEntity.Province.Name,
                    CustomerCity = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Name : order.User.CityEntity.Name,
                    CustomerAddress = order.UserAddressId.HasValue ? string.Format("{0}{1}{2}", order.UserAddress.Address, (!String.IsNullOrEmpty(order.UserAddress.AddressNumber) ? " ، پلاک" + order.UserAddress.AddressNumber : ""), (!String.IsNullOrEmpty(order.UserAddress.AddressUnit) ? " ، واحد" + order.UserAddress.AddressUnit : "")) : string.Format("{0}{1}{2}", order.User.Address, (!String.IsNullOrEmpty(order.User.AddressNumber) ? " ، پلاک" + order.User.AddressNumber : ""), (!String.IsNullOrEmpty(order.User.AddressUnit) ? " ، واحد" + order.User.AddressUnit : "")),
                    CustomerPostalCode = order.UserAddressId.HasValue ? order.UserAddress.PostalCode : order.User.PostalCode,
                    CustomerTele = order.UserAddressId.HasValue ? order.UserAddress.PhoneNumber : order.User.PhoneNumber,
                    CustomerTaxNumber = "",
                    ShoppingOrderId = "TF-" + order.BankOrderId,
                    ShoppingPayWay = order.OrderWallets.First().Wallet.PaymentType == 1 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 2 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 3 ? "کارت به کارت" : order.OrderWallets.First().Wallet.PaymentType == 4 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 5 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 6 ? "فیش نقدی" : order.OrderWallets.First().Wallet.PaymentType == 7 ? "ترب پِی" : order.OrderWallets.First().Wallet.PaymentType == 8 ? "دیچی پِی" : "---",
                    ShoppingSenWay = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 32) ? order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 32).First().Value == "1" ? order.ProductSendWay + " - پس کرایه" : order.ProductSendWay : order.ProductSendWay,
                    ShoppingUserDescr = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 18) ? order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 18).First().Value : "---",
                    ShoppingSenWayPrice = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 14) ? Convert.ToInt64(order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 14).First().Value) * 10 : 0,
                    ShoppingTotalPrice = order.OrderWallets.First().Wallet.Price * 10
                };

                var CustomerOrderRows = new List<Areas.Admin.ViewModels.Report.CustomerOrderRow>();
                int i = 1;
                foreach (var item in order.OrderRows)
                {
                    string name = item.Product.Name;
                    if (item.ProductPrice.ProductAttributeSelectModelId.HasValue)
                    {
                        name += " مدل " + item.ProductPrice.ProductAttributeSelectModel.Value;
                    }
                    if (item.ProductPrice.ProductAttributeSelectSizeId.HasValue)
                        name += " سایز " + item.ProductPrice.ProductAttributeSelectSize.Value + item.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                    if (item.ProductPrice.ProductAttributeSelectColorId.HasValue)
                        name += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(item.ProductPrice.ProductAttributeSelectColorId).Value)).Color;

                    CustomerOrderRows.Add(new CustomerOrderRow()
                    {
                        Id = i,
                        Code = item.ProductPrice.code,
                        Name = name,
                        RawPrice = item.RawPrice * 10,
                        SumPrice = (item.RawPrice * item.Quantity) * 10,
                        Quantity = item.Quantity,
                        TaxPrice = item.taxValue * 10,
                        OffPrice = Math.Abs(item.RawPrice - item.Price) * item.Quantity * 10,
                        Price = (item.RawPrice - (item.RawPrice - item.Price)) * item.Quantity * 10,
                        FinalPrice = ((item.Price * item.Quantity) + item.taxValue) * 10
                    });
                    i++;
                }

                // هزینه ارسال
                int sendrawPrice = 0;
                if (order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 33))
                {
                    sendrawPrice = Convert.ToInt32(order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 33).First().Value);
                    if (sendrawPrice > 0)
                    {
                        CustomerOrderRows.Add(new CustomerOrderRow()
                        {
                            Id = i,
                            Code = "1",
                            Name = "هزینه ارسال",
                            RawPrice = sendrawPrice * 10,
                            SumPrice = sendrawPrice * 10,
                            Quantity = 1,
                            TaxPrice = 0,
                            OffPrice = 0,
                            Price = sendrawPrice * 10,
                            FinalPrice = sendrawPrice * 10
                        });
                        i++;
                    }
                }

                // هزینه بسته بندی
                int sendPackagePrice = 0;
                if (order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 31))
                {
                    sendPackagePrice = Convert.ToInt32(order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 31).First().Value);
                    if (sendPackagePrice > 0)
                    {
                        CustomerOrderRows.Add(new CustomerOrderRow()
                        {
                            Id = i,
                            Code = "2",
                            Name = "هزینه بسته بندی",
                            RawPrice = sendPackagePrice * 10,
                            SumPrice = sendPackagePrice * 10,
                            Quantity = 1,
                            TaxPrice = 0 * 10,
                            OffPrice = 0 * 10,
                            Price = sendPackagePrice * 10,
                            FinalPrice = sendPackagePrice * 10
                        });
                        i++;
                    }
                }

                // مالیات درگاه
                if (order.Commision.HasValue)
                {
                    if (order.Commision.Value > 0)
                    {
                        CustomerOrderRows.Add(new CustomerOrderRow()
                        {
                            Id = i,
                            Code = "3",
                            Name = "مالیات درگاه",
                            RawPrice = order.Commision.Value * 10,
                            SumPrice = order.Commision.Value * 10,
                            Quantity = 1,
                            TaxPrice = 0,
                            OffPrice = 0,
                            Price = order.Commision.Value * 10,
                            FinalPrice = order.Commision.Value * 10
                        });
                        i++;
                    }
                }

                var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + CustomerOrderInfo.Logo));
                byte[] array1 = imageToByteArray(img);
                MemoryStream ms = new MemoryStream(array1);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                report.Dictionary.Variables.Add("Logo", image);

                report.RegBusinessObject("Order", CustomerOrderInfo);
                report.RegBusinessObject("CustomerOrderRow", CustomerOrderRows);

                // تبدیل و خروجی مستقیم فایل PDF به جای StiMvcReportResponse
                report.Render(false);
                StiPdfExportSettings settings = new StiPdfExportSettings();
                settings.AutoPrintMode = StiPdfAutoPrintMode.Dialog;

                MemoryStream pdfStream = new MemoryStream();
                report.ExportDocument(StiExportFormat.Pdf, pdfStream, settings);
                pdfStream.Position = 0;

                string fileName = "factor-" + order.CustomerOrderId + ".pdf";
                return File(pdfStream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { status = 500, Message = "خطا در فرآیند تولید فاکتور." }, JsonRequestBehavior.AllowGet);
            }
        }


        //[Infrastructure.Filter.AutoExecueFilter]
        [JWTAuthorize]
        public JsonResult CancelOrder(int id)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == id && x.Order.UserId == userId, null, "Order,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
                if (orderDelivery == null)
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                if (orderDelivery.OrderStates.Last().state > OrderStatus.پردازش_انبار)
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                List<SelectListItem> OrderStateSelectListItem;
                if (orderDelivery.Order.OrderWallets.FirstOrDefault().Wallet.PaymentType < 5)//پرداخت به پیک ها نیست

                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };
                else
                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };



                return Json(new
                {
                    userCard = orderDelivery.Order.User.CardNumber,
                    deliveryId = orderDelivery.Id,
                    orderId = orderDelivery.Order.Id,
                    customerOrderId = orderDelivery.Order.CustomerOrderId,
                    // فقط فیلدهایی که Views/profile/CancelOrder.cshtml استفاده می‌کنه پروجکت می‌شن
                    // (نه خودِ orderDelivery که به‌خاطر گراف دوطرفه‌ی Order/OrderDelivery باعث circular reference می‌شه).
                    orderRows = orderDelivery.OrderRows.Select(item => new
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        PageAddress = item.Product.PageAddress,
                        Title = item.Product.Name,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        ImageFileName = item.ProductPrice.ProductImages.Any()
                            ? (item.ProductPrice.ProductImages.Any(x => x.IsMain)
                                ? item.ProductPrice.ProductImages.Where(x => x.IsMain).Select(x => x.Image.FileName).FirstOrDefault()
                                : item.ProductPrice.ProductImages.Select(x => x.Image.FileName).FirstOrDefault())
                            : (item.ProductPrice.Product.ProductImages.Any()
                                ? (item.ProductPrice.Product.ProductImages.Any(x => x.IsMain)
                                    ? item.ProductPrice.Product.ProductImages.Where(x => x.IsMain).Select(x => x.Image.FileName).FirstOrDefault()
                                    : item.ProductPrice.Product.ProductImages.Select(x => x.Image.FileName).FirstOrDefault())
                                : null)
                    }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "CancelOrder", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public async Task<JsonResult> CancelOrder(CancelOrderReson cancelOrderReson, int DeliveryId, Guid OrderId, string descr)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);



            var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == DeliveryId && x.Order.UserId == userId, null, "Order.OrderRows,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
            try
            {
                if (orderDelivery == null)
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                if (orderDelivery.OrderStates.Last().state >= OrderStatus.ارسال_شده)
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                if (orderDelivery.OrderStates.Any(s => s.state == OrderStatus.عدم_تایید_درخواست_لغو) || orderDelivery.OrderStates.Any(s => s.state == OrderStatus.عدم_تایید_درخواست_مرجوعی))
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }

                ViewBag.userCard = orderDelivery.Order.User.CardNumber;
                if (orderDelivery.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value == "True")))
                {
                    return Json(new
                    {
                        status = "0",
                        msg = " به علت وجود کالاهای استعلامی در سفارش شما، امکان لغو سفارش وجود ندارد. با تشکر ",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }


                if (String.IsNullOrEmpty(descr))
                {
                    return Json(new
                    {
                        status = "0",
                        msg = " توضیحات در مورد لغو سفارش وارد نشده است ",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }

                orderDelivery.OrderStates.Add(new OrderState()
                {
                    cancelOrderReson = cancelOrderReson,
                    LogDate = DateTime.Now,
                    state = OrderStatus.درخواست_لغو,
                    OrderDeliveryId = DeliveryId,
                    OrderId = OrderId,
                    Description = descr
                });
                uow.OrderDeliveryRepository.Update(orderDelivery);
                uow.Save();


                SmsService ss = new SmsService();
                IdentityMessage iMessage = new IdentityMessage();

                iMessage.Destination = orderDelivery.Order.User.PhoneNumber;
                iMessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_درخواست_لغو_کاربر, orderDelivery.Order.UserId, orderDelivery.Order.CustomerOrderId, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Detail/" + orderDelivery.Order.CustomerOrderId, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Rate/" + orderDelivery.Order.CustomerOrderId, null, null, null, OrderStatus.درخواست_لغو.EnumDisplayNameFor(), null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Edit");
                if (!String.IsNullOrEmpty(iMessage.Body))
                    await ss.SendSMSAsync(iMessage, "NewCancelOrder", orderDelivery.Order.CustomerOrderId, null, null, null, null, true);


                var setting = GetSetting();
                iMessage.Destination = setting.Mobile;
                iMessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.ثبت_درخواست_لغو_مدیر, orderDelivery.Order.UserId, orderDelivery.Order.CustomerOrderId, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Detail/" + orderDelivery.Order.CustomerOrderId, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Rate/" + orderDelivery.Order.CustomerOrderId, null, null, null, OrderStatus.درخواست_لغو.EnumDisplayNameFor(), null, null, null, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/profile/Edit");
                if (!String.IsNullOrEmpty(iMessage.Body))
                    await ss.SendSMSAsync(iMessage, "NewCancelOrder", orderDelivery.Order.CustomerOrderId, null, null, null, null, true);


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "0",
                    msg = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);

            }


            TfShop.Infrastructure.Cart.order inforder = new TfShop.Infrastructure.Cart.order();
            inforder.CheckProductPriceDefault(orderDelivery.Order.OrderRows.Select(x => x.ProductId).ToList());
            //uow.ProductRepository.CheckProductPriceDefault(orderDelivery.Order.OrderRows.Select(x => x.ProductId).ToList());
            return Json(new
            {
                status = "1",
                msg = "درخواست اولیه شما ثبت شد. پس از بررسی درخواست شما، اطلاع رسانی خواهد شد.",
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }

        [JWTAuthorize]
        public JsonResult CancelUserOrderForm(int id)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            var setting = GetSetting();

            try
            {

                var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == id && x.Order.UserId == userId, null, "Order,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,Order.OrderWallets.Wallet.BankAccount,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
                if (orderDelivery == null)
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                if (orderDelivery.OrderStates.Last().state >= OrderStatus.ارسال_شده)
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);

                List<SelectListItem> OrderStateSelectListItem;
                if (orderDelivery.Order.OrderWallets.FirstOrDefault().Wallet.PaymentType < 5)//پرداخت به پیک ها نیست

                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };
                else
                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };


                return Json(new
                {
                    data = orderDelivery,
                    Pay = orderDelivery.OrderStates.Any(x => x.state > OrderStatus.تایید_سفارش) ? true : false,
                    OrderStateSelectListItem = OrderStateSelectListItem,
                    userCard = orderDelivery.Order.User.CardNumber,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "CancelOrder", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JWTAuthorize]
        public JsonResult CancelUserOrderForm(int id, CancelOrderReson cancelOrderReson)
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

                var setting = GetSetting();

                TfShop.Infrastructure.Cart.order inforder = new TfShop.Infrastructure.Cart.order();
                var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == id && x.Order.UserId == userId, null, "Order.OrderRows,Order.User,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,Order.OrderWallets.Wallet.ForWhat,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
                if (orderDelivery == null)
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                if (orderDelivery.OrderStates.Last().state >= OrderStatus.ارسال_شده || orderDelivery.OrderStates.Any(x => x.state == OrderStatus.لغو_شده))
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                var userCard = orderDelivery.Order.User.CardNumber;

                if (orderDelivery.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value == "True")))
                {
                    return Json(new
                    {
                        userCard = userCard,
                        data = orderDelivery,
                        status = "0",
                        msg = " به علت وجود کالاهای استعلامی در سفارش شما، امکان لغو سفارش وجود ندارد. با تشکر ",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    orderDelivery.OrderStates.Add(new OrderState()
                    {
                        cancelOrderReson = cancelOrderReson,
                        LogDate = DateTime.Now,
                        state = OrderStatus.درخواست_لغو,
                        OrderDeliveryId = orderDelivery.Id,
                        OrderId = orderDelivery.OrderId.Value,
                    });
                    orderDelivery.OrderStates.Add(new OrderState()
                    {
                        cancelOrderReson = cancelOrderReson,
                        LogDate = DateTime.Now,
                        state = OrderStatus.لغو_شده,
                        OrderDeliveryId = orderDelivery.Id,
                        OrderId = orderDelivery.OrderId.Value
                    });
                    uow.OrderDeliveryRepository.Update(orderDelivery);
                    uow.Save();

                    orderDelivery.Order.IsActive = false;
                    orderDelivery.Order.New = false;
                    orderDelivery.Order.IsExpire = true;
                    uow.Save();

                    inforder.CheckQuantity(orderDelivery.Order);

                    if (cancelOrderReson == CancelOrderReson.InvalidEdit)
                    {
                        return Json(new
                        {
                            userCard = userCard,
                            data = orderDelivery,
                            status = "2",
                            msg = orderDelivery.Order.User.FirstName + " " + orderDelivery.Order.User.LastName + " عزیز،</br>" + " سفارش " + orderDelivery.Order.CustomerOrderId + " توسط شما با موفقیت لغو گردید و موجودی کالا به حالت قبل بازگشت.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (cancelOrderReson == CancelOrderReson.InvalidSendway)
                    {
                        return Json(new
                        {
                            userCard = userCard,
                            data = orderDelivery,
                            status = "1",
                            msg = orderDelivery.Order.User.FirstName + " " + orderDelivery.Order.User.LastName + " عزیز،</br>" + " بزودی با شما تماس گرفته خواهد شد. ",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            userCard = userCard,
                            data = orderDelivery,
                            status = "1",
                            msg = orderDelivery.Order.User.FirstName + " " + orderDelivery.Order.User.LastName + " عزیز،</br>" + " سفارش " + orderDelivery.Order.CustomerOrderId + " توسط شما با موفقیت لغو گردید و موجودی کالا به حالت قبل بازگشت.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {

                    return Json(new
                    {
                        userCard = userCard,
                        data = orderDelivery,
                        status = "0",
                        msg = " خطایی رخ داد. " + ex.Message,
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
                inforder.CheckProductPriceDefault(orderDelivery.Order.OrderRows.Select(x => x.ProductId).ToList());

                //uow.ProductRepository.CheckProductPriceDefault(orderDelivery.Order.OrderRows.Select(x => x.ProductId).ToList());
                return Json(new
                {
                    userCard = userCard,
                    data = orderDelivery,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "CancelUserOrderForm", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        public JsonResult CancelUserOrder(int id)
        {
            try
            {

                string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
                string userId = Authentication.ValidateToken(Token);
                if (userId == null)
                    return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);
                string msg = "";
                string status = "";
                TfShop.Infrastructure.Cart.order inforder = new TfShop.Infrastructure.Cart.order();
                var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == id && x.Order.UserId == userId, null, "Order.OrderRows,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
                if (orderDelivery == null)
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }
                if (orderDelivery.OrderStates.Last().state >= OrderStatus.ارسال_شده || orderDelivery.OrderStates.Any(x => x.state == OrderStatus.لغو_شده))
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }


                var setting = GetSetting();


                var userCard = orderDelivery.Order.User.CardNumber;
                if (orderDelivery.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value == "True")))
                {

                    return Json(new
                    {
                        userCard = userCard,
                        data = orderDelivery,
                        status = "0",
                        msg = " به علت وجود کالاهای استعلامی در سفارش شما، امکان لغو سفارش وجود ندارد. با تشکر ",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }


                try
                {
                    orderDelivery.OrderStates.Add(new OrderState()
                    {
                        cancelOrderReson = CancelOrderReson.InvalidCancel,
                        LogDate = DateTime.Now,
                        state = OrderStatus.درخواست_لغو,
                        OrderDeliveryId = orderDelivery.Id,
                        OrderId = orderDelivery.OrderId.Value
                    });
                    orderDelivery.OrderStates.Add(new OrderState()
                    {
                        cancelOrderReson = CancelOrderReson.InvalidCancel,
                        LogDate = DateTime.Now,
                        state = OrderStatus.لغو_شده,
                        OrderDeliveryId = orderDelivery.Id,
                        OrderId = orderDelivery.OrderId.Value
                    });
                    uow.OrderDeliveryRepository.Update(orderDelivery);
                    uow.Save();


                    orderDelivery.Order.IsActive = false;
                    orderDelivery.Order.New = false;
                    orderDelivery.Order.IsExpire = true;
                    uow.Save();

                    inforder.CheckQuantity(orderDelivery.Order);
                    //uow.OrderRepository.CheckQuantity(orderDelivery.Order);

                    status = "1";
                    msg = "سفارش شما با شماره " + orderDelivery.Order.CustomerOrderId + " لغو شد.";

                }
                catch (Exception ex)
                {
                    Infrastructure.EventLog.Logger.Add(5, "api", "CancelUserOrderForm", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return Json(new
                    {
                        Message = "خطایی رخ داد.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }

                inforder.CheckProductPriceDefault(orderDelivery.Order.OrderRows.Select(x => x.ProductId).ToList());
                //uow.ProductRepository.CheckProductPriceDefault(orderDelivery.Order.OrderRows.Select(x => x.ProductId).ToList());

                return Json(new
                {
                    userCard = userCard,
                    data = orderDelivery,
                    status = "1",
                    msg = "سفارش شما با شماره " + orderDelivery.Order.CustomerOrderId + " لغو شد.",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "CancelUserOrderForm", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [JWTAuthorize]
        public JsonResult Rate(int id, int? step, int? status)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            var setting = GetSetting();
            string msg = "";
            bool first = false;
            int? stp = null;
            #region Message
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case -1: msg = "خطایی در ثبت نظر شما اتفاق افتاد."; break;
                    default: break;
                }
            }
            else
            {
                if (step.HasValue)
                    if (step.Value == 1)
                        first = true;
            }
            #endregion

            try
            {
                if (!step.HasValue)
                    step = 0;
                // با AsNoTracking + Include صریح (نه متد قدیمی Get که entity رو tracked/lazy برمی‌گردوند)
                // تا هیچ navigation propertyـی که Include نشده لمس نشه (همون چیزی که باعث
                // circular reference موقع serialize شدن می‌شد).
                var order = uow.OrderRepository.GetQueryList().AsNoTracking()
                    .Include("OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute")
                    .Include("OrderAttributeSelects.OrderAttribute")
                    .Include("OrderWallets.Wallet.BankAccount")
                    .Include("User.CityEntity.Province")
                    .Include("OrderStates")
                    // دقیقاً همون مسیری که Rate.cshtml برای ۳ عکس پیش‌نمایش استفاده می‌کنه:
                    // item2.Product.ProductImages (نه ProductPrice.ProductImages).
                    .Include("OrderRows.Product.ProductImages.Image")
                    // برای Model.order.OrderDeliveries.First().ProductSendWay.Image (عکس بالای step 1 اول)
                    .Include("OrderDeliveries.ProductSendWay.Image")
                    .Where(x => x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت) && x.OrderStates.Any(s => s.state == OrderStatus.تحویل_داده_شده) && !x.OrderStates.Any(s => s.state == OrderStatus.لغو_شده) && !x.OrderStates.Any(s => s.state == OrderStatus.مرجوعی) && x.CustomerOrderId == id.ToString() && x.UserId == userId)
                    .SingleOrDefault();
                if (order == null)
                {
                    return Json(new
                    {
                        url = "/profile/orders",
                        statusCode = 404
                    }, JsonRequestBehavior.AllowGet);
                }



                stp = step;

                var savedOrderRates = uow.OrderRateRepository.Get(x => x.orderRateId, x => x.Order.CustomerOrderId == id.ToString());
                int count = 0;
                // توجه: orderRateVM.order (کلاس OrderRateVM) عمداً پر نمی‌شه. اون فیلد فقط برای View قدیمی
                // Razor (ProfileController.Rate) لازم بود که مستقیم Model.order.XYZ می‌خوند؛ اینجا اکشن API
                // همه‌جا از متغیر محلی order (که با AsNoTracking + Include محدود گرفته شده) استفاده می‌کنه،
                // پس نگه‌داشتن یک رفرنس اضافه به کل entity خام داخل VM نه لازمه و نه با اصل "فقط دیتای
                // موردنیاز فرانت" همخونی داره.
                OrderRateVM orderRateVM = new OrderRateVM()
                {
                    ProductAdvantageTitless = uow.ProductAdvantageTitleRepository.Get(x => x, x => x.IsActive, x => x.OrderBy(s => s.DisplayOrder)),
                    ProductDisAdvantageTitles = uow.ProductDisAdvantageTitleRepository.Get(x => x, x => x.IsActive, x => x.OrderBy(s => s.DisplayOrder)),
                    OrderRateTitle = uow.SettingRepository.Get(x => x.OrderRateTitle, null).First(),
                    orderRateItems = uow.OrderRateItemRepository.Get(x => x, x => !savedOrderRates.Contains(x.Id), x => x.OrderBy(s => s.Title), "", 0, 1),
                    products = uow.ProductCommentRepository.GetNoUserCommenProductsRanks(order.Id, userId, out count),
                    productCount = order.OrderRows.Count,
                    productRemainCount = count
                };

                if (step != 0)
                {
                    if (orderRateVM.orderRateItems.Any() && (step != null && step != 1))
                    {
                        return Json(new
                        {
                            url = "/profile/rate/" + id + "?step=1",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (!orderRateVM.orderRateItems.Any() && orderRateVM.products.Any() && (step != null && step != 2))
                    {
                        return Json(new
                        {
                            url = "/profile/rate/" + id + "?step=2",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                // این چک عمداً بیرون از "if (step != 0)" هست: وقتی نظرسنجی از قبل کامل تکمیل شده
                // (هم آیتم امتیازدهی و هم محصولات بدون نظر خالی‌ان)، باید step=3 برگرده حتی وقتی
                // step درخواستی 0 بوده (یعنی کاربر مستقیم وارد صفحه‌ی شروع نظرسنجی شده). قبلاً این
                // حالت فقط زمانی چک می‌شد که step != 0 بود، برای همین وقتی step=0 بود همیشه صفحه‌ی
                // شروع (step=0) برمی‌گشت و هیچ‌وقت step=3 نمی‌اومد.
                if (!orderRateVM.orderRateItems.Any() && !orderRateVM.products.Any() && step != 3)
                {
                    return Json(new
                    {
                        url = "/profile/rate/" + id + "?step=3",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                //step 3
                if (!orderRateVM.orderRateItems.Any() && !orderRateVM.products.Any())
                {
                    ViewBag.finished = true;

                    // order با AsNoTracking و چند Include هم‌پوشان گرفته شده (مثلاً ProductPrice هم از
                    // مسیر OrderRows.ProductPrice و هم از مسیر OrderRows.Product.ProductPrices) و چون
                    // AsNoTracking هیچ identity resolution‌ای انجام نمی‌ده، همون entity با همون Primary Key
                    // دوبار به‌صورت instance جدا توی گراف هست، پس Update(order) کل گراف رو Attach می‌کنه و
                    // خطای "already has the same primary key value" می‌ده. یه stub تازه‌ساز هم جواب نداد،
                    // چون EF موقع Save() تمام Required propertyهای entity (مثل UserId) رو ولیدیت می‌کنه،
                    // نه فقط اونی که IsModified شده، و چون stub فقط Id/compeleteRate داشت خطای
                    // "UserId اجباریه" می‌داد.
                    // راه‌حل درست: خودِ رکورد واقعی سفارش رو (بدون هیچ navigation propertyـی، فقط
                    // ستون‌های خودش) با Find می‌گیریم؛ این‌طوری هم tracked می‌شه (تغییرش قابل Save هست)
                    // هم چون هیچ Include‌ای نداره فقط یک instance واحد برای این Id توی context هست (نه
                    // گراف تکراری)، هم چون از دیتابیس واقعی خونده شده همه‌ی فیلدهای Required (از جمله
                    // UserId) از قبل مقدار درست دارن.
                    var orderToUpdate = uow.context.Set<Domain.Order>().Find(order.Id);
                    if (orderToUpdate != null)
                    {
                        orderToUpdate.compeleteRate = true;
                        uow.Save();
                    }
                }


                // فقط فیلدهایی که فرانت استفاده می‌کنه پروجکت می‌شن؛ orderRateVM.order دیگه اصلاً پر
                // نمی‌شه (بالاتر حذف شد) و متغیر order هم مستقیم سریالایز نمی‌شه، فقط از دلش فیلد
                // اسکالر/لیست ساده استخراج می‌شه تا به circular reference نخوریم.
                return Json(new
                {
                    orderRateTitle = orderRateVM.OrderRateTitle,
                    customerOrderId = order.CustomerOrderId,
                    orderInsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(order.InsertDate),
                    orderInsertTime = order.InsertDate.ToShortTimeString(),
                    cityName = order.User.CityId.HasValue ? order.User.CityEntity.Name : null,
                    // معادل دقیق Rate.cshtml: item2.Product.ProductImages.Where(x => x.IsMain).First().Image.FileName
                    // (نه ProductPrice.ProductImages؛ منبع عکس باید همون Product.ProductImages باشه).
                    previewImages = order.OrderRows.OrderBy(x => x.Id).Take(3).Select(x =>
                        x.Product.ProductImages.Where(i => i.IsMain).Select(i => i.Image.FileName).FirstOrDefault()
                    ).ToList(),
                    // معادل Model.order.OrderDeliveries.First().ProductSendWay.Image.FileName (عکس بالای step 1
                    // موقعی که ViewBag.first فعاله)
                    firstItemImage = order.OrderDeliveries.OrderBy(d => d.Id).FirstOrDefault()?.ProductSendWay?.Image?.FileName,
                    orderRowsCount = order.OrderRows.Count,
                    orderRateItem = orderRateVM.orderRateItems.Select(x => new { x.Id, x.Title }).FirstOrDefault(),
                    products = orderRateVM.products.Select(p => new { p.Id, p.Title, p.MainImageFileName }).ToList(),
                    productCount = orderRateVM.productCount,
                    productRemainCount = orderRateVM.productRemainCount,
                    msg = msg,
                    step = stp,
                    first = first,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Infrastructure.EventLog.Logger.Add(5, "api", "Rate", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpParamAction]
        [HttpPost]
        [JWTAuthorize]
        public JsonResult RateOrder(string id, Int16 RateOrder, int RateItem, bool? likerate, string reason)
        {
            string Token = Request.Headers.GetValues("AuthToken").FirstOrDefault();
            string userId = Authentication.ValidateToken(Token);
            if (userId == null)
                return Json(new { status = 10, Message = "Token Expired!" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (RateOrder == 0)
                    RateOrder = 4;
                else if (RateOrder == 1)
                    RateOrder = 3;
                else if (RateOrder == 3)
                    RateOrder = 1;
                else if (RateOrder == 4)
                    RateOrder = 0;
                OrderRate orderRate = new OrderRate()
                {
                    LogDate = DateTime.Now,
                    OrderId = uow.OrderRepository.Get(x => x.Id, x => x.CustomerOrderId == id).First(),
                    orderRateId = RateItem,
                    state = (OrderRateStatus)RateOrder,
                    likerate = likerate,
                    reason = reason

                };
                uow.OrderRateRepository.Insert(orderRate);
                uow.Save();


                var order = uow.OrderRepository.Get(x => x, x => x.CustomerOrderId == id).FirstOrDefault();
                order.compeleteRate = true;
                uow.OrderRepository.Update(order);
                uow.Save();

                var savedOrderRates = uow.OrderRateRepository.Get(x => x.orderRateId, x => x.Order.CustomerOrderId == id.ToString());

                // معادل دقیق منطق تشخیص step توی اکشن Rate (GET): اگه هنوز آیتم نظرسنجی رتبه‌بندی‌نشده
                // هست step=1، وگرنه اگه محصول بدون نظر باقی مونده step=2، وگرنه (هر دو خالی) step=3.
                // قبلاً این‌جا فقط بین 1 و 2 انتخاب می‌شد و هیچ‌وقت 3 برنمی‌گشت چون چک محصولات باقی‌مونده
                // انجام نمی‌شد.
                int nextStep;
                if (uow.OrderRateItemRepository.Any(x => x, x => !savedOrderRates.Contains(x.Id)))
                {
                    nextStep = 1;
                }
                else
                {
                    int remainingProductsCount;
                    uow.ProductCommentRepository.GetNoUserCommenProductsRanks(order.Id, userId, out remainingProductsCount);
                    nextStep = remainingProductsCount > 0 ? 2 : 3;
                }

                return Json(new
                {
                    url = "Rate",
                    id = id,
                    step = nextStep,
                    status = 1,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "api", "Rate", false, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return Json(new
                {
                    Message = "خطایی رخ داد.",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion
        public String GetUserPlatform(HttpRequestBase request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }

        private SettingDto GetSetting()
        {
            SettingDto setting = null;
            var configuration = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Setting, SettingDto>()
                .ForMember(dto => dto.attachmentFileName, conf => conf.MapFrom(ol => ol.attachment.FileName))
                .ForMember(dto => dto.attachmentFileNameMag, conf => conf.MapFrom(ol => ol.attachmentLogoMag.FileName));
            });
            setting = context.Settings.AsQueryable().AsNoTracking().Include(c => c.attachmentLogoMag).Include(c => c.attachment).Where(c => c.LanguageId == 1)
                .ProjectTo<SettingDto>(configuration).FirstOrDefault();

            return setting;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}