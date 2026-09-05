using CoreLib;
using Domain;
using Domain.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UnitOfWork;

namespace TfShop.Infrastructure.Filter
{
    public class UserActivitiesObj 
    {

        public void SaveUserActivity(ActionExecutedContext filterContext = null, int? Id = null)
        {
            try
            {
                int? id = 0;
                if (filterContext != null)
                    id = Convert.ToInt32(filterContext.RouteData.Values["id"]);
                else if (Id.HasValue)
                    id = Id.Value;
                string pathinfo = getPathInfo().ToLower();
                string HTTP_METHOD = gethttp_method().ToLower();
                string UserAgent = HttpContext.Current.Request.UserAgent;
                if (!pathinfo.Contains("profile") && !pathinfo.Contains("admin") && (HTTP_METHOD == "post" || HTTP_METHOD == "get") && !UserAgent.ToLower().Contains("bot"))
                {
                    //HttpCookie BasketCookie = HttpContext.Current.Request.Cookies["BasketProductPrice"];
                    //var baskets = new List<UserActivityBasketItem>();
                    //if (BasketCookie != null)
                    //{

                    //    var s = new JavaScriptSerializer();
                    //    var BasketItems = s.Deserialize<List<Basket>>(BasketCookie.Value);
                    //    foreach (var item in BasketItems)
                    //        baskets.Add(new UserActivityBasketItem() { ProductPriceId = item.ProductPriceId });
                    //}
                    int? CookieId = null;
                    HttpCookie aCookie = HttpContext.Current.Request.Cookies["tfactivity"];
                    using (UnitOfWorkClass UOW = new UnitOfWorkClass())
                    {

                        if (aCookie != null)
                        {
                            if (!String.IsNullOrEmpty(aCookie.Value))
                            {
                                CookieId = Convert.ToInt32(aCookie.Value);
                                if (!UOW.UserActivityRepository.Any(x => x.Id, x => x.Id == CookieId.Value))
                                {
                                    CookieId = null;
                                    aCookie.Value = "";
                                    aCookie.Expires = DateTime.Now.AddDays(-1);
                                    HttpContext.Current.Response.Cookies.Add(aCookie);
                                }
                            }
                        }

                        //var countrycity =  getCountryCity().Result;
                        var ua = new Domain.UserActivity()
                        {
                            SessionId = HttpContext.Current.Session.SessionID,
                            CookieId = CookieId,
                            RouteValueId = id,
                            IP = getIpAddress(),
                            //Country = countrycity.Country,
                            //City = countrycity.City,
                            HTTP_METHOD = HTTP_METHOD,
                            LOGON_USER = getLogonUser(),
                            PATH_INFO = pathinfo,
                            Platform = GetUserPlatform(),
                            UserAgent = UserAgent,
                            Browser = getBrowser(),
                            Referer = getHttpReferer(),
                            URL = getHTTP_URL(),
                            LogDateTime = DateTime.Now,
                            //UserActivityBasketItems = baskets
                        };
                        QuerySecion querySecion = getQueryString();
                        ua.QUERY_STRING = querySecion.queryString;
                        ua.UTMcampaign = querySecion.UTMcampaign;
                        ua.UTMsource = querySecion.UTMsource;
                        ua.UTMmedium = querySecion.UTMmedium;

                        ua.UserActivityRefererId = getHttpRefererId(querySecion);

                        UOW.UserActivityRepository.Insert(ua);
                        UOW.Save();

                        if (aCookie == null)
                        {
                            aCookie = new HttpCookie("tfactivity");
                            CookieId = ua.Id;
                            ua.CookieId = CookieId;
                            UOW.UserActivityRepository.Update(ua);
                            UOW.Save();
                        }
                        else if (String.IsNullOrEmpty(aCookie.Value))
                        {
                            CookieId = ua.Id;
                            ua.CookieId = CookieId;
                            UOW.UserActivityRepository.Update(ua);
                            UOW.Save();
                        }
                        aCookie.Value = CookieId.ToString();
                        aCookie.Expires = DateTime.Now.AddYears(10);
                        HttpContext.Current.Response.Cookies.Add(aCookie);

                        if (HttpContext.Current.User.Identity.IsAuthenticated)
                            Infrastructure.EventLog.UpdateUserActivity.SetUserActivities();

                    }
                }

            }
            catch (Exception)
            {

            }
        }
        public async Task<CountryCity> getCountryCity()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://ip-geolocation-ipwhois-io.p.rapidapi.com/json/?ip=" + getIpAddress()),
                Headers =
    {
        { "X-RapidAPI-Key", "ad5644641cmsh7758c2f547ec978p12c8fajsnf45c902952be" },
        { "X-RapidAPI-Host", "ip-geolocation-ipwhois-io.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CountryCity>(body);
            }
        }
        public string getHTTP_URL()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_URL"];
        }
        public QuerySecion getQueryString()
        {
            string q = HttpContext.Current.Request.ServerVariables["QUERY_STRING"].ToLower();
            QuerySecion querySecion = new QuerySecion() { queryString = q };
            if (q.Contains("utm_source"))
            {
                var utms = q.Split('&');
                if (utms.Any())
                {
                    if (utms.Any(x => x.Contains("utm_source")))
                        querySecion.UTMsource = utms.Where(x => x.Contains("utm_source")).First().Replace("utm_source=", "");
                    if (utms.Any(x => x.Contains("utm_medium")))
                        querySecion.UTMmedium = utms.Where(x => x.Contains("utm_medium")).First().Replace("utm_medium=", "");
                    if (utms.Any(x => x.Contains("utm_campaign")))
                        querySecion.UTMcampaign += utms.Where(x => x.Contains("utm_campaign")).First().Replace("utm_campaign=", "");
                }
                else
                {
                    querySecion.UTMsource = q.Replace("utm_source=", "");
                }
            }

            return querySecion;
        }
        public string getPathInfo()
        {
            return HttpContext.Current.Request.ServerVariables["PATH_INFO"];
        }
        public string getHttpReferer()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
        }
        public int? getHttpRefererId(QuerySecion querySecion)
        {
            string referer = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            using (UnitOfWorkClass UOW = new UnitOfWorkClass())
            {
                string host = "";
                //Referer has UTM
                if (!String.IsNullOrEmpty(querySecion.UTMsource))
                {
                    if (String.IsNullOrEmpty(referer))
                        referer = host = querySecion.UTMsource;

                    // Source Medium Campaign
                    if (!String.IsNullOrEmpty(querySecion.UTMmedium) && !String.IsNullOrEmpty(querySecion.UTMcampaign))
                    {
                        var rid = UOW.UserActivityRefererRepository.Get(x => x, x => x.UTMsource == querySecion.UTMsource && x.UTMmedium == querySecion.UTMmedium && x.UTMcampaign == querySecion.UTMcampaign).FirstOrDefault();
                        if (rid != null)
                            return rid.Id;
                        else
                        {
                            UserActivityReferer userActivityReferer = new UserActivityReferer()
                            {
                                Link = referer,
                                Title = host,
                                UTMsource = querySecion.UTMsource,
                                UTMmedium = querySecion.UTMmedium,
                                UTMcampaign = querySecion.UTMcampaign
                            };
                            UOW.UserActivityRefererRepository.Insert(userActivityReferer);
                            UOW.Save();

                            return userActivityReferer.Id;
                        }
                    }
                    // Source Medium
                    else if (!String.IsNullOrEmpty(querySecion.UTMmedium))
                    {
                        var rid = UOW.UserActivityRefererRepository.Get(x => x, x => x.UTMsource == querySecion.UTMsource && x.UTMmedium == querySecion.UTMmedium && x.UTMcampaign==null).FirstOrDefault();
                        if (rid != null)
                            return rid.Id;
                        else
                        {
                            UserActivityReferer userActivityReferer = new UserActivityReferer()
                            {
                                Link = referer,
                                Title = host,
                                UTMsource = querySecion.UTMsource,
                                UTMmedium = querySecion.UTMmedium
                            };
                            UOW.UserActivityRefererRepository.Insert(userActivityReferer);
                            UOW.Save();

                            return userActivityReferer.Id;
                        }
                    }
                    // Source Campaign
                    else if (!String.IsNullOrEmpty(querySecion.UTMcampaign))
                    {
                        var rid = UOW.UserActivityRefererRepository.Get(x => x, x => x.UTMsource == querySecion.UTMsource && x.UTMcampaign == querySecion.UTMcampaign && x.UTMmedium==null).FirstOrDefault();
                        if (rid != null)
                            return rid.Id;
                        else
                        {
                            UserActivityReferer userActivityReferer = new UserActivityReferer()
                            {
                                Link = referer,
                                Title = host,
                                UTMsource = querySecion.UTMsource,
                                UTMcampaign = querySecion.UTMcampaign
                            };
                            UOW.UserActivityRefererRepository.Insert(userActivityReferer);
                            UOW.Save();

                            return userActivityReferer.Id;
                        }
                    }

                    // Source only
                    else
                    {
                        var rid = UOW.UserActivityRefererRepository.Get(x => x, x => x.UTMsource == querySecion.UTMsource && x.UTMmedium==null && x.UTMcampaign==null).FirstOrDefault();
                        if (rid != null)
                            return rid.Id;
                        else
                        {
                            UserActivityReferer userActivityReferer = new UserActivityReferer()
                            {
                                Link = referer,
                                Title = host,
                                UTMsource = querySecion.UTMsource
                            };
                            UOW.UserActivityRefererRepository.Insert(userActivityReferer);
                            UOW.Save();

                            return userActivityReferer.Id;
                        }
                    }
                }
                // dose not have UTM
                else
                {
                    if (!String.IsNullOrEmpty(referer))
                    {
                        if (!referer.ToLower().Contains("tfshops.com") && !referer.ToLower().Contains("shaparak.ir"))
                        {
                            referer = referer.Substring(referer.IndexOf("://") + 3);

                            if (!referer.Contains("http"))
                                referer = "http://" + referer;
                            Uri myUri = new Uri(referer);
                            host = myUri.Host.Replace("wwww.", "");



                            var rid = UOW.UserActivityRefererRepository.Get(x => x, x => x.Link.Contains(referer)).FirstOrDefault();
                            if (rid != null)
                                return rid.Id;
                            else if (UOW.UserActivityRefererRepository.Any(x => x.Id, x => x.Link.Contains(host) && x.ParrentId == null))
                            {
                                rid = UOW.UserActivityRefererRepository.Get(x => x, x => x.Link.Contains(host) && x.ParrentId == null).FirstOrDefault();
                                UserActivityReferer userActivityReferer = new UserActivityReferer()
                                {
                                    Link = referer,
                                    Title = host,
                                    Cover = rid.Cover,
                                    ParrentId = rid.Id,
                                    ppc = rid.ppc
                                };
                                UOW.UserActivityRefererRepository.Insert(userActivityReferer);
                                UOW.Save();

                                return userActivityReferer.Id;
                            }
                            else
                            {
                                UserActivityReferer userActivityReferer = new UserActivityReferer()
                                {
                                    Link = referer,
                                    Title = host
                                };
                                UOW.UserActivityRefererRepository.Insert(userActivityReferer);
                                UOW.Save();

                                return userActivityReferer.Id;
                            }



                        }
                    }
                }
            }
            return null;
        }
        public string gethttp_method()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_METHOD"];
        }
        public string getRequestMode()
        {
            return HttpContext.Current.Request.ServerVariables["REQUEST_METHOD"];
        }
        public string getContentType()
        {
            return HttpContext.Current.Request.ServerVariables["CONTENT_TYPE"];
        }
        public string getLogonUser()
        {
            return HttpContext.Current.Request.ServerVariables["LOGON_USER"];
        }
        public string getAUTH_USER()
        {
            return HttpContext.Current.Request.ServerVariables["AUTH_USER"];
        }
        public string getREMOTE_USER()
        {
            return HttpContext.Current.Request.ServerVariables["REMOTE_USER"];
        }
        public string getIpAddress()
        {
            string ipAddress = "127.0.0.1";
            if (HttpContext.Current != null)
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            return ipAddress;
        }

        public string getBrowser()
        {
            return HttpContext.Current.Request.Browser.Browser;
        }
        public String GetUserPlatform()
        {
            var ua = HttpContext.Current.Request.UserAgent;

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
            return HttpContext.Current.Request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
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

    }



}