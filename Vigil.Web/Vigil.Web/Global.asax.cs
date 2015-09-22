using System.Diagnostics.Contracts;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Vigil.Web
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Contract.Assume(GlobalFilters.Filters != null);
            Contract.Assume(RouteTable.Routes != null);
            Contract.Assume(BundleTable.Bundles != null);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
