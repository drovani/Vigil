using System.Diagnostics.Contracts;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Web.Controllers;

namespace Vigil.Testing.Web.Controllers
{
    [TestClass]
    [ContractVerification(false)]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
