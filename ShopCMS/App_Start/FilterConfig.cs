using System.Web;
using System.Web.Mvc;
//using TfShop.Infrastructure.Filter;

namespace TfShop
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new Infrastructure.Filter.UserActivitiesFilter());
        }
    }
}
