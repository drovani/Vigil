using Moq;
using System;
using System.Web;
using System.Web.Routing;
using Xunit;

namespace Vigil.Testing.Web
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class TestHelpers
    {
        public static void AssertRoute(RouteCollection routes, string url, object expectations)
        {
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns(url);
            RouteData routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.NotNull(routeData);

            foreach (var kvp in new RouteValueDictionary(expectations))
            {
                Assert.True(routeData.Values.ContainsKey(kvp.Key),
                        string.Format("Route Key '{0}' not found.", kvp.Key));
                Assert.True(string.Equals(kvp.Value.ToString(), routeData.Values[kvp.Key].ToString(), StringComparison.OrdinalIgnoreCase),
                        string.Format("Expected '{0}', got '{1}' for '{2}'.", kvp.Value, routeData.Values[kvp.Key], kvp.Key));
            }
        }
    }
}
