using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Data.Modeling
{
    [TestClass]
    [ContractVerification(false)]
    public class VigilContextTests
    {
        private VigilContext context;
        private VigilUser testUser;
        private DateTime now;

        [TestInitialize]
        public void TestInitialize()
        {
            testUser = new VigilUser { UserName = "TestUser" };
            now = new DateTime(2015, 4, 23, 13, 33, 12, DateTimeKind.Utc);
            context = new VigilContext(testUser, now);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void Initialize_And_Create_Database_Cause_Database_To_Exists()
        {
            Database db = context.Database;
            db.Initialize(true);
            if (db.Exists())
            {
                db.Delete();
            }
            db.Create();

            Assert.IsTrue(db.Exists());
        }

        [TestMethod]
        public void Explicit_Constructor_Sets_AffectedBy_And_Now()
        {
            Assert.AreEqual(testUser, context.AffectedBy);
            Assert.AreEqual(now, context.Now);
        }

        [TestMethod]
        public void Validate_VigilUser_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUser>();
            Assert.AreEqual("[vigil].[VigilUser]", tableName);
        }

        [TestMethod]
        public void Validate_VigilUserClaim_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUserClaim>();
            Assert.AreEqual("[vigil].[VigilUserClaim]", tableName);
        }

        public void Validate_VigilUserLogin_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUserLogin>();
            Assert.AreEqual("[vigil].[VigilUserLogin]", tableName);
        }

        [TestMethod]
        public void Validate_VigilUserRole_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUserRole>();
            Assert.AreEqual("[vigil].[VigilUserRole]", tableName);
        }

        [TestMethod]
        public void Validate_VigilRole_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilRole>();
            Assert.AreEqual("[vigil].[VigilRole]", tableName);
        }

        [TestMethod]
        public void Set_Method_Returns_Valid_Objects()
        {
            Assert.IsInstanceOfType(context.Set<VigilUser>(), typeof(IDbSet<VigilUser>));
        }

        private string GetTableName<TEntity>() where TEntity : class
        {
            Contract.Requires<ArgumentNullException>(context != null);

            ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
            var sql = objContext.CreateObjectSet<TEntity>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            return match.Groups["table"].Value;
        }
    }
}
