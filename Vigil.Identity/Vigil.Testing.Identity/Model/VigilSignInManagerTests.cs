using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    [ContractVerification(false)]
    public class VigilSignInManagerTests
    {
        [Fact]
        public void VigilSignInManager_DefaultConstructor()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(new InMemoryUserStore()), new InMemoryAuthenticationManager());
            Assert.NotNull(signInMgr);
        }

        /// <summary>Tests the VigilSignInManager.Create static method.
        /// </summary>
        [Fact(Skip="Requires an IOwinContext implementation to obtain VigilUserManager and IAuthenticationManager.")]
        public void Create_Test()
        {
            IdentityFactoryOptions<IVigilSignInManager> options = new IdentityFactoryOptions<IVigilSignInManager>();
            IOwinContext context = null;

            var signInManager = VigilSignInManager.Create(options, context);

            Assert.NotNull(signInManager);
            Assert.Same(context.Get<VigilUserManager>(), signInManager.UserManager);
            Assert.Same(context.Authentication, signInManager.AuthenticationManager);
        }

        [Fact]
        public void CreateUserIdentityAsync_Returns_Valid_ClaimsIdentity()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(new InMemoryUserStore()), new InMemoryAuthenticationManager());
            var user = new VigilUser() { UserName = "TestUser", Email = "signinmanager@example.com" };
            var claimsIdentity = signInMgr.CreateUserIdentityAsync(user).Result;

            Assert.NotNull(claimsIdentity);
            Assert.Equal("TestUser", claimsIdentity.Name);
        }
    }
}
