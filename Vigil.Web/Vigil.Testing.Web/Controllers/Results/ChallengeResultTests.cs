using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Moq;
using Vigil.Testing.Identity;
using Vigil.Web.Controllers.Results;
using Xunit;

namespace Vigil.Testing.Web.Controllers.Results
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class ChallengeResultTests
    {
        [Fact]
        public void ExecuteResult_Should_Call_OwinContext_Authentication_Challenge()
        {
            var challenge = new ChallengeResult("TestProvider", "/", Guid.NewGuid());
            // HttpContext.GetOwinContext() is an extension method that calls context.Items[OwinEnvironmentKey] which passes that to a new OwinContext(environment);
            var mockHttpContext = new Mock<HttpContextBase>();
            IDictionary<string, object> owinEnvironment = new Dictionary<string, object>();
            mockHttpContext.Setup(ctx => ctx.Items[It.Is<string>(s => s == IdentityGlobalConstant.OwinEnvironmentKey)])
                           .Returns(owinEnvironment)
                           .Verifiable();
            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.SetupGet<HttpContextBase>(mcc => mcc.HttpContext)
                                 .Returns(mockHttpContext.Object)
                                 .Verifiable();

            challenge.ExecuteResult(mockControllerContext.Object);

            mockHttpContext.Verify();
            mockControllerContext.Verify();

            // @TODO: Determine that owinContext.Authentication.Challenge method was called.
        }
    }
}
