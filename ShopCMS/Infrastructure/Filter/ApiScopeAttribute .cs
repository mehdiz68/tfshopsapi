using Domain;
using Glimpse.Core.ClientScript;
using System.Web.Mvc;

public class ApiScopeAttribute : ActionFilterAttribute
{
    private readonly string scope;

    public ApiScopeAttribute(string scope)
    {
        this.scope = scope;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //var client = (ApiClients)filterContext.HttpContext.Items["ApiClient"];
        var request = filterContext.HttpContext.Request;
        string clientId = request.Headers["X-Client-Id"];

        var client = ApiClientCache.Get(clientId);

        if (client == null || !client.Scopes.Contains(scope))
        {
            filterContext.Result = new HttpStatusCodeResult(403);
        }

        base.OnActionExecuting(filterContext);
    }
}
