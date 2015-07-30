using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;

namespace Vigil.Testing.Identity.Model
{
    [TestClass]
    public class VigilRoleManagerTests
    {
        [TestMethod]
        public void VigilRoleManagerTest()
        {
            var rolestore = new InMemoryRoleStore();
            VigilRoleManager vrman = new VigilRoleManager(rolestore);

            Assert.IsNotNull(vrman);
        }

        [TestMethod]
        [Ignore]
        public void CreateTest()
        {
            throw new NotImplementedException();
        }
    }
}
