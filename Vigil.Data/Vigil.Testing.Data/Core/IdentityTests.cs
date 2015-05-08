using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Testing.Data.TestClasses;

namespace Vigil.Testing.Data.Core
{
    [TestClass]
    public class IdentityTests
    {
        [TestMethod]
        public void Identity_Equality_Fails_For_Unequal_Ids()
        {
            var a = TestIdentity.CreateTestIdentity(Guid.NewGuid());
            var b = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void Identity_Equality_Fails_For_Null_Other()
        {
            var a = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            Assert.IsFalse(a.Equals(null));
        }

        [TestMethod]
        public void Identity_Equality_Passes_For_Equal_Id_Values()
        {
            Guid id = Guid.NewGuid();
            var a = TestIdentity.CreateTestIdentity(id);
            var b = TestIdentity.CreateTestIdentity(id);

            Assert.IsTrue(a.Equals(b));
        }
    }
}
