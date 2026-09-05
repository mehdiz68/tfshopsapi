using CoreLib;
using Domain;
using Microsoft.AspNet.Identity;
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
using System.Web.UI;
using UnitOfWork;

namespace TfShop.Infrastructure.Filter
{
    public class NonAuthenticatedOnlyCacheAttribute : OutputCacheAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                // it's crucial not to cache Authenticated content
                Location = OutputCacheLocation.None;
            }

            // this smells a little but it works
            httpContext.Response.Cache.AddValidationCallback(IgnoreAuthenticated, null);

            base.OnResultExecuting(filterContext);
        }

        // This method is called each time when cached page is going to be
        // served and ensures that cache is ignored for authenticated users.
        private void IgnoreAuthenticated(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            if (context.User.Identity.IsAuthenticated)
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
            else
                validationStatus = HttpValidationStatus.Valid;
        }
    }
}