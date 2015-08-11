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
        public void VigilRoleManager_Constructor_Accepts_RoleStore()
        {
            var rolestore = new InMemoryRoleStore();
            VigilRoleManager vrman = new VigilRoleManager(rolestore);

            Assert.IsNotNull(vrman);
        }

        [TestMethod]
        [Ignore]
        public void VigilRoleManager_Static_Create_Returns_Valid_Manager()
        {
            throw new NotImplementedException();
        }
    }
}
