using System.Web.Routing;
using Vigil.Web;
using Xunit;

namespace Vigil.Testing.Web
{
    public class RouteConfigTests
    {
        [Fact]
        public void DefaultRoute_Correctly_Routes()
        {
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            TestHelpers.AssertRoute(routes, "~/", new { culture = "en-US", controller = "Home", action = "Index" });
        }
    }
}
