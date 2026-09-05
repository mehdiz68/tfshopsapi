[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(TfShop.App_Start.Combres), "PreStart")]
namespace TfShop.App_Start {
	using System.Web.Routing;
	using global::Combres;
	
    public static class Combres {
        public static void PreStart() {
            RouteTable.Routes.AddCombresRoute("Combres");
        }
    }
}