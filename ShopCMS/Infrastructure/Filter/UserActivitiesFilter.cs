using CoreLib;
using Domain;
using Domain.ViewModels;
using Fasterflect;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TfShop.Infrastructure.RSS;
using UnitOfWork;

namespace TfShop.Infrastructure.Filter
{
    public class UserActivitiesFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            UserActivitiesObj userActivitiesObj = new UserActivitiesObj();
            userActivitiesObj.SaveUserActivity(filterContext);
        }

    }

    public class QuerySecion
    {
        public string queryString { get; set; }
        public string UTMcampaign { get; set; }
        public string UTMsource { get; set; }
        public string UTMmedium { get; set; }
    }
    public class CountryCity
    {
        public string Country { get; set; }
        public string City { get; set; }
    }

}