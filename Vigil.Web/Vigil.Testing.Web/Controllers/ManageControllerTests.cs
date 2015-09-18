using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Web.Controllers;
using Xunit;

namespace Vigil.Testing.Web.Controllers
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class ManageControllerTests
    {
        private readonly Mock<IPrincipal> MockUser;
        private readonly Mock<VigilUserManager> MockUserManager;
        private readonly Mock<IVigilSignInManager> MockSignInManager;
        private readonly Mock<IAuthenticationManager> MockAuthenticationManager = new Mock<IAuthenticationManager>();

        public ManageControllerTests()
        {
            MockUser = new Mock<IPrincipal>();
            MockUserManager = new Mock<VigilUserManager>(Mock.Of<IUserStore<VigilUser, Guid>>());
            MockAuthenticationManager = new Mock<IAuthenticationManager>();
            MockSignInManager = new Mock<IVigilSignInManager>();
            MockSignInManager.SetupGet(msim => msim.AuthenticationManager).Returns(MockAuthenticationManager.Object);
            MockSignInManager.SetupGet(msim => msim.UserManager).Returns(MockUserManager.Object);
        }

        [Fact]
        public void ManageController_Default_Constructor()
        {
            var ctrl = new ManageController();
            Assert.NotNull(ctrl);
        }

        [Fact]
        public void ManageController_Explicit_Constructor()
        {
            var ctrl = GetManageController();

            Assert.NotNull(ctrl);
            Assert.Same(MockSignInManager.Object, ctrl.SignInManager);
            Assert.Same(MockUserManager.Object, ctrl.UserManager);
        }

        private ManageController GetManageController()
        {
            var ctrl = new ManageController(MockUserManager.Object, MockSignInManager.Object, MockAuthenticationManager.Object);

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.SetupGet(ctx => ctx.User).Returns(MockUser.Object);
            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.SetupGet(con => con.HttpContext).Returns(mockHttpContext.Object);

            ctrl.ControllerContext = mockControllerContext.Object;
            return ctrl;
        }
    }
}
