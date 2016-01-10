using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using System.Web.Routing;
using Vigil.Web;
using Vigil.Web.Areas.Patron;
using Xunit;

namespace Vigil.Testing.Web.Areas.Patron
{
    public class PatronAreaRegistrationTests
    {
        [Theory, MemberData("PatronRoutes")]
        public void Route_PatronArea_Tests(string url, object expectations)
        {
            var routes = new RouteCollection();
            var area = new PatronAreaRegistration();

            var areaRegContext = new AreaRegistrationContext(area.AreaName, routes);
            area.RegisterArea(areaRegContext);
            RouteConfig.RegisterRoutes(routes);

            TestHelpers.AssertRoute(routes, url, expectations);
        }

        public static IEnumerable<object> PatronRoutes
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<object>>() != null);
                return new[]
                {
                    new object[] { "~/en-US/Patron/Search",
                        new {
                            culture = "en-US",
                            controller = "Search",
                            action = "Index"
                        } },
                    new object[] { "~/fr-CA/Patron/850827/Account",
                        new {
                            culture ="fr-CA",
                            accountNumber = "850827",
                            controller ="Account",
                            action ="Index"
                        } }
                };
            }
        }
    }
}
