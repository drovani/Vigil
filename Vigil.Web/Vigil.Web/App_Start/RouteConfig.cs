using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using Vigil.Web.Mvc;

namespace Vigil.Web
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default_With_Culture",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Vigil.Web.Controllers" },
                constraints: new { culture = new CultureConstraint("en-US", "[a-z]{2,3}(?:-[A-Z]{2,3})?") }
            );
            routes.MapRoute(
                name: "Default_Without_Culture",
                url: "{controller}/{action}/{id}",
                defaults: new { culture = "en-US", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Vigil.Web.Controllers" }
            );
        }
    }
}
