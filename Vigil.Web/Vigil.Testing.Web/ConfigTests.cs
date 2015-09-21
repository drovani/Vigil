using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Vigil.Web;
using Xunit;

namespace Vigil.Testing.Web
{
    public class ConfigTests
    {
        [Fact]
        public void BundleConfig_RegisterBundles_Adds_Bundles()
        {
            BundleCollection bundles = new BundleCollection();
            BundleConfig.RegisterBundles(bundles);

            Assert.NotEqual(0, bundles.Count);
        }

        [Fact]
        public void FilterConfig_RegisterGlobalFilters_Adds_Filters()
        {
            GlobalFilterCollection filters = new GlobalFilterCollection();
            FilterConfig.RegisterGlobalFilters(filters);

            Assert.NotEqual(0, filters.Count);
        }

        [Fact]
        public void RouteConfig_RegisterRoutes_Adds_Routes()
        {
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            Assert.NotEqual(0, routes.Count);
        }
    }
}
