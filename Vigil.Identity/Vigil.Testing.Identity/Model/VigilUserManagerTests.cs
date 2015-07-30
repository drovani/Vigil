using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;

namespace Vigil.Testing.Identity.Model
{
    [TestClass]
    public class VigilUserManagerTests
    {
        [TestMethod]
        public void VigilUserManagerTest()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            Assert.IsNotNull(vuman);
        }

        [TestMethod]
        [Ignore]
        public void CreateTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateAsync_Sets_Empty_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var user = new VigilUser { UserName = "TestUser" };

            var result = vuman.CreateAsync(user).Result;
            Assert.AreNotEqual(Guid.Empty, user.Id);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void CreateAsync_Preserves_Specified_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var newguid = Guid.NewGuid();
            var user = new VigilUser {Id = newguid, UserName = "TestUser" };

            var result = vuman.CreateAsync(user).Result;
            Assert.AreEqual(newguid, user.Id);
            Assert.IsTrue(result.Succeeded);
        }
        
        [TestMethod]
        public void CreateAsync_With_Password_Sets_Empty_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var user = new VigilUser { UserName = "TestUser" };

            var result = vuman.CreateAsync(user, "testPassword.01").Result;
            Assert.AreNotEqual(Guid.Empty, user.Id);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void CreateAsync_With_Password_Preserves_Specified_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var newguid = Guid.NewGuid();
            var user = new VigilUser { Id = newguid, UserName = "TestUser" };

            var result = vuman.CreateAsync(user, "testPassword.01").Result;
            Assert.AreEqual(newguid, user.Id);
            Assert.IsTrue(result.Succeeded);
        }
    }
}
