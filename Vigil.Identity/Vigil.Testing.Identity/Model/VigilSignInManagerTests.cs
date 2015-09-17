using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    [ContractVerification(false)]
    public class VigilSignInManagerTests
    {
        [Fact]
        public void VigilSignInManager_DefaultConstructor()
        {
            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Get<IdentityVigilContext>(GlobalConstant.IdentityKeyPrefix + typeof(IdentityVigilContext).AssemblyQualifiedName))
                   .Returns(new IdentityVigilContext());

            var signInMgr = new VigilSignInManager(new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>()), Mock.Of<IAuthenticationManager>());
            Assert.NotNull(signInMgr);
        }

        [Fact]
        public void Create_Test()
        {
            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Get<VigilUserManager>(GlobalConstant.IdentityKeyPrefix + typeof(VigilUserManager).AssemblyQualifiedName))
                   .Returns(new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>()));
            context.SetupGet(c => c.Authentication).Returns(Mock.Of<IAuthenticationManager>());
            IdentityFactoryOptions<IVigilSignInManager> options = new IdentityFactoryOptions<IVigilSignInManager>();

            var signInManager = VigilSignInManager.Create(options, context.Object);

            Assert.NotNull(signInManager);
            Assert.Same(context.Object.Get<VigilUserManager>(), signInManager.UserManager);
            Assert.Same(context.Object.Authentication, signInManager.AuthenticationManager);
        }

        [Fact]
        public void CreateUserIdentityAsync_Returns_Valid_ClaimsIdentity()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>()), Mock.Of<IAuthenticationManager>());
            var user = new VigilUser() { UserName = "TestUser", Email = "signinmanager@example.com" };
            var claimsIdentity = signInMgr.CreateUserIdentityAsync(user).Result;

            Assert.NotNull(claimsIdentity);
            Assert.Equal("TestUser", claimsIdentity.Name);
        }
    }
}
