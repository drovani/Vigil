using Vigil.Web.Controllers;
using Xunit;

namespace Vigil.Testing.Web.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_Returns_View()
        {
            HomeController ctrl = new HomeController();
            var result = ctrl.Index();

            Assert.NotNull(result);
        }
    }
}
