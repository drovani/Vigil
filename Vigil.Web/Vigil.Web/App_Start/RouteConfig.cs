using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vigil.Web
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = CultureInfo.CurrentCulture.Name, controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Vigil.Web.Controllers" }
            );
        }
    }
}
