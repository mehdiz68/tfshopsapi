using CoreLib;
using Domain;
using Domain.ViewModels;
using Fasterflect;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace TfShop.Infrastructure.Filter
{
    public class JWTAuthorize : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var tokens = context.HttpContext.Request.Headers.GetValues("AuthToken");
            if (tokens == null)
            {
                //context.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Forbidden");
                context.Result = new JsonResult()
                {
                    Data = new { status = -3, Message = "forbidden" },
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
                base.OnActionExecuting(context);
        }
       
    }

}