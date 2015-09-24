using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using Vigil.Testing.Identity;
using Vigil.Web.Controllers;
using Xunit;

namespace Vigil.Testing.Web.Controllers
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class TestAccountController : AccountController
    {
        public TestAccountController()
            : base()
        {
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.SetupGet(con => con.HttpContext).Returns(mockHttpContext.Object);

            this.ControllerContext = mockControllerContext.Object;
        }

        [Fact]
        public void AuthenticationManager_Property_Gets_Manager_From_OwinContext_When_Not_Explicitly_Specified()
        {
            var owinEnvironment = new Dictionary<string, object>();
            Mock.Get(HttpContext).Setup(ctx => ctx.Items[It.Is<string>(s => s == IdentityGlobalConstant.OwinEnvironmentKey)])
                           .Returns(owinEnvironment);

            IAuthenticationManager manager = AuthenticationManager;

            Assert.NotNull(manager);
        }
    }
}
