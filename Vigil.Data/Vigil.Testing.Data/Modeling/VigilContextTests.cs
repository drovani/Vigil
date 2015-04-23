using System;
using System.Data.Entity;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Data.Modeling
{
    [TestClass]
    public class VigilContextTests
    {
        [TestMethod]
        public void Initialize_And_Create_Database_Cause_Database_To_Exists()
        {
            VigilContext context = new VigilContext();
            context.Database.Initialize(true);
            if (context.Database.Exists())
            {
                context.Database.Delete();
            }
            context.Database.Create();

            Assert.IsTrue(context.Database.Exists());
        }

        [TestMethod]
        public void Explicit_Constructor_Sets_AffectedBy_and_Now()
        {
            VigilUser testUser = new VigilUser{ UserName = "TestUser" };

            DateTime now = new DateTime(2015, 4, 23, 13, 33, 12, DateTimeKind.Utc);
            VigilContext context = new VigilContext(testUser, now);

            Assert.AreEqual(testUser, context.AffectedBy);
            Assert.AreEqual(now, context.Now);
        }
    }
}
