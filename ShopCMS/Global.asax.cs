using TfShop.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TfShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-Frame-Options");
            Response.AddHeader("X-Frame-Options", "AllowAll");

        }
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    if (!Request.Url.Host.StartsWith("www") && !Request.Url.IsLoopback)
        //    {
        //        UriBuilder builder = new UriBuilder(Request.Url);
        //        builder.Host = "www." + Request.Url.Host;
        //        Response.StatusCode = 301;
        //        Response.AddHeader("Location", builder.ToString());
        //        Response.End();
        //    }
        //}
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteTable.Routes.Add("ImagesRoute", new Route("tf-Products/{id}/{folder}/{w}/{h}/{size}/{q}", new RouteValueDictionary() { { "folder", "" }, { "w", "1280" }, { "h", "1280" }, { "size", "LG" }, { "q", "8" } }, new ImageRouteHandler()));
            // این پروژه فقط apiv1 رو سرو می‌کنه (بدون Viewِ صفحه‌ساز) - پوشه‌ی Scripts که
            // BundleConfig بهش نیاز داشت حذف شده، پس فراخوانیِ bundling هم دیگه لازم نیست (وگرنه
            // Application_Start با ArgumentException می‌ترکه چون ~/Scripts وجود نداره).
            // BundleConfig.RegisterBundles(BundleTable.Bundles);

            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom == "LoggedUserName")
            {
                if (context.Request.IsAuthenticated)
                {
                    return context.User.Identity.Name;
                }
                return null;
            }
            return base.GetVaryByCustomString(context, custom);
        }

    }
}
