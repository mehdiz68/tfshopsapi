using FirebaseAdmin.Auth.Hash;
using System.Linq;
using System.Web.Mvc;

public class ApiClientAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var request = filterContext.HttpContext.Request;

        string clientId = request.Headers["X-Client-Id"];
        string clientSecret = request.Headers["X-Client-Secret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            filterContext.Result = new HttpStatusCodeResult(403);
            return;
        }

        var client = ApiClientCache.Get(clientId);

        if (client == null)
        {
            filterContext.Result = new HttpStatusCodeResult(403);
            return;
        }

        if (!BCrypt.Net.BCrypt.Verify(clientSecret, client.ClientSecretHash))
        {
            filterContext.Result = new HttpStatusCodeResult(403);
            return;
        }

        // IP whitelist
        if (!string.IsNullOrEmpty(client.AllowedIPs))
        {
            var ip = request.UserHostAddress;

            if (!client.AllowedIPs.Split(',').Contains(ip))
            {
                filterContext.Result = new HttpStatusCodeResult(403);
                return;
            }
        }

        base.OnActionExecuting(filterContext);
    }
}
