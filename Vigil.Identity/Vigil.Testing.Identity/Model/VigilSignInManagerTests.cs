using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;

namespace Vigil.Testing.Identity.Model
{
    [TestClass()]
    public class VigilSignInManagerTests
    {
        [TestMethod]
        public void VigilSignInManager_DefaultConstructor()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(new InMemoryUserStore()), new InMemoryAuthenticationManager());
            Assert.IsNotNull(signInMgr);
        }

        /// <summary>Tests the VigilSignInManager.Create static method.
        /// <remarks>Uses the context.GetUserManager to obtain the VigilUserManager
        /// and the context.Authentication to obtain the IAuthenticationManager
        /// needed for the explicit constructor.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public void Create_Test()
        {
            IdentityFactoryOptions<VigilSignInManager> options = new IdentityFactoryOptions<VigilSignInManager>();
            IOwinContext context = null;

            var signInManager = VigilSignInManager.Create(options, context);

            Assert.IsNotNull(signInManager);
            Assert.AreSame(context.Get<VigilUserManager>(), signInManager.UserManager);
            Assert.AreSame(context.Authentication, signInManager.AuthenticationManager);
        }

        [TestMethod]
        public void CreateUserIdentityAsync_Returns_Valid_ClaimsIdentity()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(new InMemoryUserStore()), new InMemoryAuthenticationManager());
            var user = new VigilUser() { UserName = "TestUser", Email = "signinmanager@example.com" };
            var claimsIdentity = signInMgr.CreateUserIdentityAsync(user).Result;

            Assert.IsNotNull(claimsIdentity);
            Assert.AreEqual("TestUser", claimsIdentity.Name);
        }
    }
}
