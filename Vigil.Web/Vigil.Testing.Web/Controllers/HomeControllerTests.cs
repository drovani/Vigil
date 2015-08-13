using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Web.Controllers;

namespace Vigil.Testing.Web.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_Returns_View()
        {
            HomeController ctrl = new HomeController();
            var result = ctrl.Index();

            Assert.IsNotNull(result);
        }
    }
}
