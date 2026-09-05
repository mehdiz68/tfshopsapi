using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class SmartCampaignService : GenericRepository<SmartCampaign>
    {
        public SmartCampaignService(TfShopDbContext context) : base(context)
        {

        }
        public List<PrsBanner> GetBannerRecomment(int? cookieId, int id, int typeid, SmartCampaignDevice smartCampaignDevice)
        {
            DateTime dtnow = DateTime.Now;

            if (Any(s => s.Id, x => x.SmartCampaignType == SmartCampaignType.Banner && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtnow) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtnow) || x.StartDate == null)))
            {
                //آیا بازدیدکننده جدید است؟
                if (!cookieId.HasValue)
                {
                    return null;
                }
                //بازدید کننده قبلی است
                else
                {

                    //آیا علائق بازدیدکننده مشخص شده است؟
                    if (context.UserActivityFavorates.Any(a => a.UserActivityId == cookieId.Value))
                    {

                        var UserFavorates = context.UserActivityFavorates.Where(a => a.UserActivityId == cookieId.Value).ToList();
                        return BannerFavorateList(cookieId.Value, smartCampaignDevice, UserFavorates);


                    }
                    //بازدید کننده هنوز علائق مشخص شده ای ندارد
                    else
                    {
                        switch (typeid)
                        {
                            case 1:
                                var list = from x in context.UserActivities
                                           where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")
                                           join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                           select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")))
                                if (list.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);

                                    if (favs.Any())
                                    {
                                        return BannerFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return BannerFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                //else if(context.UserActivities.Any(x => x.Id == cookieId.Value))
                                //{
                                //    var z = context.UserActivities.Where(x => x.Id == cookieId || x.CookieId == cookieId).OrderBy; 
                                //    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                //    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                //}
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return BannerFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            default:
                                break;
                        }

                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }
        private List<PrsBanner> BannerFavorateList(int? cookieId, SmartCampaignDevice smartCampaignDevice, List<UserActivityFavorate> UserActivityFavorates)
        {
            DateTime dtnow = DateTime.Now;

            DateTime dt90 = dtnow.AddDays(-90);
            List<Domain.ViewModels.PrsBanner> BannerList = new List<Domain.ViewModels.PrsBanner>();
            // لیست مورد علاقه ها
            var UserFavorates = UserActivityFavorates;
            var UserFavorateCatIds = UserFavorates.Select(x => x.CatId).ToList();
            // کمپین محصول طبق گروه مورد علاقه ها یا همه گروهها
            if (Any(x => x, x => (x.SmartCampaignDevice == smartCampaignDevice || x.SmartCampaignDevice == SmartCampaignDevice.All) && (UserFavorateCatIds.Contains(x.UserActivityGroupId.Value) || x.UserActivityGroupId == null) && x.SmartCampaignType == SmartCampaignType.Banner && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtnow) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtnow) || x.StartDate == null)))
            {
                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "4", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                int c = 0;
                // بنرهای هر گروه طبق مورد علاقه ها به ترتیب به شرطی که هر بنر بیش از 30 بار نشون داده نشده باشه
                foreach (var item in UserFavorates.OrderByDescending(x => x.Rate))
                {
                    var banners = context.Database.SqlQuery<PrsBanner>("exec [GetBannerRecommends] @cookieid,@catid", new SqlParameter("@cookieid", cookieId.Value), new SqlParameter("@catid", item.CatId)).ToList();
                    if (banners.Any())
                    {
                        //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "7", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                        //context.SaveChanges();
                        c += banners.Count();
                        //ذخیره در لاگ کمپین جهت نمایش
                        List<int> adids = banners.Select(s => s.AdId).ToList();
                        context.SmartAds.Where(x => adids.Contains(x.Id)).ForEachAsync(x => x.visits++);
                        //context.SmartAdLogs.AddRange(banners.Select(x => new SmartAdLog() { AdId = x.AdId, SmartAdImageId = x.Id, SmartCampaignId = x.CampaignId, InsertDate = DateTime.Now, ViewOrClick = true, UserActivityId = cookieId.Value }));
                        context.SaveChanges();
                        //ذخیره در لاگ موقت
                        foreach (var item2 in banners)
                        {
                            if (!context.SmartBannerTempLogs.Any(x => x.UserActivityId == cookieId.Value && x.SmartAdImageId == item2.Id))
                                context.SmartBannerTempLogs.Add(new SmartBannerTempLog() { SmartAdImageId = item2.Id, UserActivityId = cookieId.Value, Count = 1 });
                            else
                                context.SmartBannerTempLogs.Where(x => x.UserActivityId == cookieId.Value && x.SmartAdImageId == item2.Id).First().Count += 1;
                            context.SaveChanges();
                        }
                        BannerList.AddRange(banners);

                    }
                    if (c >= 20)
                        break;
                }

                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "8", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                //لاگهای 30 عددی و بیشتر رو 0 کن برای مخاطب
                context.SmartBannerTempLogs.Where(x => x.UserActivityId == cookieId.Value && x.Count >= 30).ToList().ForEach(a => a.Count = 0);
                context.SaveChanges();

                return BannerList;
            }
            else
            {
                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "9", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                return null;
            }
        }

        public List<PrsProduct> GetProductRecommend(int? cookieId, int id, int typeid, SmartCampaignDevice smartCampaignDevice)
        {
            DateTime dtnow = DateTime.Now;

            if (Any(s => s.Id, x => x.SmartCampaignType == SmartCampaignType.Product && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtnow) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtnow) || x.StartDate == null)))
            {
                //آیا بازدیدکننده جدید است؟
                if (!cookieId.HasValue)
                {
                    //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "1", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                    //context.SaveChanges();
                    // اگر در 
                    return null;
                }
                //بازدید کننده قبلی است
                else
                {
                    //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "2", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                    //context.SaveChanges();
                    //آیا علائق بازدیدکننده مشخص شده است؟
                    if (context.UserActivityFavorates.Any(a => a.UserActivityId == cookieId.Value))
                    {

                        var UserFavorates = context.UserActivityFavorates.Where(a => a.UserActivityId == cookieId.Value).ToList();
                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, UserFavorates);
                        //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "3", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                        //context.SaveChanges();

                    }
                    //بازدید کننده هنوز علائق مشخص شده ای ندارد
                    else
                    {
                        switch (typeid)
                        {
                            //content
                            case 1:

                                var list = from x in context.UserActivities
                                           where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")
                                           join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                           select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")))
                                if (list.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfmag/blog")
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                //else if(context.UserActivities.Any(x => x.Id == cookieId.Value))
                                //{
                                //    var z = context.UserActivities.Where(x => x.Id == cookieId || x.CookieId == cookieId).OrderBy; 
                                //    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                //    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                //}
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            //category
                            case 2:
                                var list2 = from x in context.UserActivities
                                            where x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/category") || x.PATH_INFO.ToLower().StartsWith("/tfmag/videocategory") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcastcategory"))
                                            join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                            select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/category") || x.PATH_INFO.ToLower().StartsWith("/tfmag/videocategory") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcastcategory"))))
                                if (list2.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/category") || x.PATH_INFO.ToLower().StartsWith("/tfmag/videocategory") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcastcategory"))).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/category") || x.PATH_INFO.ToLower().StartsWith("/tfmag/videocategory") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcastcategory"))
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            //content type
                            case 3:
                                var list3 = from x in context.UserActivities
                                           where x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/videos") || x.PATH_INFO.ToLower().StartsWith("/tfmag/blogs") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcasts") || x.PATH_INFO.ToLower().StartsWith("/tfmag/search"))
                                            join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                           select x.Id;
                               // if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/videos") || x.PATH_INFO.ToLower().StartsWith("/tfmag/blogs") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcasts") || x.PATH_INFO.ToLower().StartsWith("/tfmag/search"))))
                               if(list3.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/videos") || x.PATH_INFO.ToLower().StartsWith("/tfmag/blogs") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcasts") || x.PATH_INFO.ToLower().StartsWith("/tfmag/search"))).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && (x.PATH_INFO.ToLower().StartsWith("/tfmag/videos") || x.PATH_INFO.ToLower().StartsWith("/tfmag/blogs") || x.PATH_INFO.ToLower().StartsWith("/tfmag/podcasts") || x.PATH_INFO.ToLower().StartsWith("/tfmag/search"))
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            //brand
                            case 4:
                                var list4 = from x in context.UserActivities
                                            where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfb")
                                            join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                            select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfb")))
                                if(list4.Any())
                                {
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfb")
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);

                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfb")).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            //product tag
                            case 5:
                                var list5 = from x in context.UserActivities
                                            where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/ptag")
                                            join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                            select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/ptag")))
                                if(list5.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/ptag")).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/ptag")
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            //product search
                            case 6:
                                var list6 = from x in context.UserActivities
                                           where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfs")
                                            join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                           select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfs")))
                                if(list6.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfs")).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfs")
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }
                            //product
                            case 7:
                                var list7 = from x in context.UserActivities
                                            where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfp")
                                            join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                            select x.Id;
                                //if (context.UserActivities.Any(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfp")))
                                if(list7.Any())
                                {
                                    //var favs = context.UserActivities.Where(x => x.UserActivityFavorates.Any() && x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfp")).SelectMany(x => x.UserActivityFavorates).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    var favs = (from x in context.UserActivities
                                                where x.Id != cookieId.Value && x.RouteValueId == id && x.PATH_INFO.ToLower().StartsWith("/tfp")
                                                join y in context.UserActivityFavorates on x.Id equals y.UserActivityId
                                                select y).Distinct().OrderByDescending(x => x.Rate).Skip(() => 0).Take(() => 3);
                                    if (favs.Any())
                                    {
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, favs.ToList());
                                    }
                                    else
                                    {
                                        var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                        return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                    }
                                }
                                else
                                {
                                    var randomFavs = context.UserActivityFavorates.OrderBy(x => Guid.NewGuid()).Skip(() => 0).Take(() => 3);
                                    return ProductFavorateList(cookieId.Value, smartCampaignDevice, randomFavs.ToList());
                                }

                            default:
                                break;
                        }

                        return null;
                    }
                }
            }
            else
            {
                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "11", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                return null;
            }
        }


        private List<Domain.ViewModels.PrsProduct> ProductFavorateList(int? cookieId, SmartCampaignDevice smartCampaignDevice, List<UserActivityFavorate> UserActivityFavorates)
        {
            DateTime dtnow = DateTime.Now;

            DateTime dt90 = dtnow.AddDays(-90);
            List<Domain.ViewModels.PrsProduct> ProductList = new List<Domain.ViewModels.PrsProduct>();

            return ProductList;
            // لیست مورد علاقه ها
            var UserFavorates = UserActivityFavorates;
            var UserFavorateCatIds = UserFavorates.Select(x => x.CatId).ToList();
            // کمپین محصول طبق گروه مورد علاقه ها یا همه گروهها
            var campaign = Get(x => x, x => (x.SmartCampaignDevice == smartCampaignDevice || x.SmartCampaignDevice == SmartCampaignDevice.All) && (UserFavorateCatIds.Contains(x.UserActivityGroupId.Value) || x.UserActivityGroupId == null) && x.SmartCampaignType == SmartCampaignType.Product && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= dtnow) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= dtnow) || x.StartDate == null)).FirstOrDefault();
            if (campaign != null)
            {
                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "4", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                int c = 0;
                //محصولات دیده شده ی قبلی
                var visitedProducts = context.UserActivities.Where(x => (x.Id == cookieId || x.CookieId == cookieId) && x.LogDateTime >= dt90 && x.PATH_INFO.Contains("tfp/")).Select(x => x.RouteValueId).Distinct().ToList();
                //بررسی محصولات در سفارشات قبلی
                var userActivityLogonUser = context.UserActivities.Find(cookieId).LOGON_USER.Trim();
                if (!String.IsNullOrEmpty(userActivityLogonUser))
                {
                    //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "5", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                    //context.SaveChanges();
                    var user = context.Users.Where(x => x.UserName == userActivityLogonUser).FirstOrDefault();
                    if (user != null)
                    {
                        //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "6", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                        //context.SaveChanges();
                        var buyprs = context.OrderRows.Where(x => x.Order.UserId == user.Id && x.Order.InsertDate >= dt90 && visitedProducts.Contains(x.ProductId)).Select(x => x.ProductId).ToList();
                        if (buyprs.Any())
                            visitedProducts.RemoveAll(x => buyprs.Contains(x.Value));
                    }
                }
                // محصولات دیده شده قبلی، طبق مورد علاقه ها به ترتیب به شرطی که هر محصول بیش از 30 بار نشون داده نشده باشه
                foreach (var item in UserFavorates.OrderByDescending(x => x.Rate))
                {
                    List<PrsProduct> products = null;
                    if (visitedProducts.Any())
                        products = context.Database.SqlQuery<PrsProduct>("exec [GetProductRecommends] @cookieid,@catid,@campaignId,@ids", new SqlParameter("@cookieid", cookieId.Value), new SqlParameter("@catid", item.CatId), new SqlParameter("@campaignId", campaign.Id), new SqlParameter("@ids", string.Join(",", visitedProducts))).ToList();
                    else
                        products = context.Database.SqlQuery<PrsProduct>("exec [GetProductRecommends] @cookieid,@catid,@campaignId", new SqlParameter("@cookieid", cookieId.Value), new SqlParameter("@catid", item.CatId), new SqlParameter("@campaignId", campaign.Id)).ToList();
                    if (products.Any())
                    {
                        //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "7", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                        //context.SaveChanges();
                        c += products.Count();
                        //ذخیره در لاگ کمپین جهت نمایش

                        List<int> adids = products.Select(s => s.CampaignId).ToList();
                        context.SmartCampaigns.Where(x => adids.Contains(x.Id)).ForEachAsync(x => x.visits++);
                        //context.SmartAdLogs.AddRange(products.Select(x => new SmartAdLog() { ProductId = x.Id, SmartCampaignId = campaign.Id, InsertDate = DateTime.Now, ViewOrClick = true, UserActivityId = cookieId.Value }));
                        context.SaveChanges();
                        //ذخیره در لاگ موقت
                        foreach (var item2 in products)
                        {
                            if (!context.SmartProductTempLogs.Any(x => x.UserActivityId == cookieId.Value && x.ProductId == item2.Id))
                                context.SmartProductTempLogs.Add(new SmartProductTempLog() { ProductId = item2.Id, UserActivityId = cookieId.Value, Count = 1 });
                            else
                                context.SmartProductTempLogs.Where(x => x.UserActivityId == cookieId.Value && x.ProductId == item2.Id).First().Count += 1;
                            context.SaveChanges();
                        }
                        ProductList.AddRange(products);

                    }
                    if (c >= 20)
                        break;
                }

                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "8", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                //لاگهای 30 عددی و بیشتر رو 0 کن برای مخاطب
                context.SmartProductTempLogs.Where(x => x.UserActivityId == cookieId.Value && x.Count >= 30).ToList().ForEach(a => a.Count = 0);
                context.SaveChanges();

                return ProductList;
            }
            else
            {
                //context.EventLogs.Add(new Domain.EventLog() { IP = "127.0.0.1", LogType = 5, ControllerName = "GetHeader", ActionName = "Home", RequestType = true, StatusCode = 500, Description = "9", LogDateTime = DateTime.Now, UserId = "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84" });
                //context.SaveChanges();
                return null;
            }
        }
    }
}
