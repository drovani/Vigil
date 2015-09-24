using System;
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
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class VigilSignInManagerTests
    {
        [Fact]
        public void VigilSignInManager_DefaultConstructor()
        {
            using (var ivContext = new IdentityVigilContext())
            {
                var context = new Mock<IOwinContext>();
                context.Setup(c => c.Get<IdentityVigilContext>(IdentityGlobalConstant.IdentityKeyPrefix + typeof(IdentityVigilContext).AssemblyQualifiedName))
                       .Returns(ivContext);
                using(var vigUman = new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>()))
                using (var signInMgr = new VigilSignInManager(vigUman, Mock.Of<IAuthenticationManager>()))
                {
                    Assert.NotNull(signInMgr);
                }
            }
        }

        [Fact]
        public void Create_Static_Method_Returns_New_VigilSignInManager()
        {
            using (var vuman = new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>()))
            {
                var context = new Mock<IOwinContext>();
                context.Setup(c => c.Get<VigilUserManager>(IdentityGlobalConstant.IdentityKeyPrefix + typeof(VigilUserManager).AssemblyQualifiedName))
                       .Returns(vuman);
                context.SetupGet(c => c.Authentication).Returns(Mock.Of<IAuthenticationManager>());

                using (var signInManager = VigilSignInManager.Create(context.Object))
                {

                    Assert.NotNull(signInManager);
                    Assert.Same(context.Object.Get<VigilUserManager>(), signInManager.UserManager);
                    Assert.Same(context.Object.Authentication, signInManager.AuthenticationManager);
                }
            }
        }

        [Fact]
        public void CreateUserIdentityAsync_Returns_Valid_ClaimsIdentity()
        {
            using (var vigUman = new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>()))
            using (var signInMgr = new VigilSignInManager(vigUman, Mock.Of<IAuthenticationManager>()))
            {
                var user = new VigilUser() { UserName = "TestUser", Email = "signinmanager@example.com" };
                var claimsIdentity = signInMgr.CreateUserIdentityAsync(user).Result;

                Assert.NotNull(claimsIdentity);
                Assert.Equal("TestUser", claimsIdentity.Name);
            }
        }
    }
}
