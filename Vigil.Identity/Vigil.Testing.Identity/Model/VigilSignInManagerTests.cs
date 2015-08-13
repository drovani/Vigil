using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;

namespace Vigil.Testing.Identity.Model
{
    [TestClass()]
    public class VigilSignInManagerTests
    {
        [TestMethod()]
        public void VigilSignInManager_DefaultConstructor()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(new InMemoryUserStore()), new InMemoryAuthenticationManager());
            Assert.IsNotNull(signInMgr);
        }

        [TestMethod()]
        public void Create_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void CreateUserIdentityAsync_Test()
        {
            var signInMgr = new VigilSignInManager(new VigilUserManager(new InMemoryUserStore()), new InMemoryAuthenticationManager());
            var user = new VigilUser() { UserName = "TestUser", Email = "signinmanager@example.com" };
            var claimsIdentity = signInMgr.CreateUserIdentityAsync(user).Result;

            Assert.IsNotNull(claimsIdentity);
        }
    }
}
