using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;

namespace Vigil.Testing.Identity.Model
{
    [TestClass()]
    public class IdentityVigilContextTests
    {
        [TestMethod()]
        public void IdentityVigilContext_Default_Constructor_Sets_Now()
        {
            var context = new IdentityVigilContext();
            Assert.AreNotEqual(DateTime.MinValue, context.Now);
        }

        [TestMethod()]
        public void IdentityVigilContext_Explicit_Contructor_Sets_Now_and_AffectedBy()
        {
            VigilUser affectedBy = new VigilUser(){ Id = Guid.NewGuid(), UserName = "TestUser" };
            DateTime now = new DateTime(2015, 7, 30, 15, 10, 12, DateTimeKind.Utc);
            var context = new IdentityVigilContext(affectedBy, now);
            Assert.AreEqual(affectedBy, context.AffectedBy);
            Assert.AreEqual(now, context.Now);
        }

        [TestMethod()]
        public void SetAffectingUser_Sets_AffectedBy()
        {
            var context = new IdentityVigilContext();
            Assert.IsNull(context.AffectedBy);

            VigilUser affectedBy = new VigilUser() { Id = Guid.NewGuid(), UserName = "TestUser" };
            context.SetAffectingUser(affectedBy);
            Assert.AreEqual(affectedBy, context.AffectedBy);
        }

        [TestMethod()]
        public void Create_Static_Method_Returns_New_IdentityVigilContext()
        {
            var context = IdentityVigilContext.Create();

            Assert.IsNull(context.AffectedBy);
            Assert.AreNotEqual(DateTime.MinValue, context.Now);
        }
    }
}
