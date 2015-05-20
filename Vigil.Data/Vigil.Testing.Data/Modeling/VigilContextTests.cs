using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Data.Modeling;

namespace Vigil.Testing.Data.Modeling
{
    [TestClass]
    [ContractVerification(false)]
    public class VigilContextTests
    {
        VigilUser testUser;
        DateTime now;

        [TestInitialize]
        public void TestInitialize()
        {
            testUser = new VigilUser { UserName = "TestUser" };
            now = new DateTime(2015, 4, 23, 13, 33, 12, DateTimeKind.Utc);
        }

        [TestMethod]
        public void Initialize_And_Create_Database_Cause_Database_To_Exists()
        {
            using (VigilContext context = new VigilContext(testUser, now))
            {
                Database.SetInitializer<VigilContext>(new DropCreateDatabaseAlways<VigilContext>());
                context.Database.Initialize(true);

                Assert.IsTrue(context.Database.Exists());
            }
        }

        [TestMethod]
        public void Explicit_Constructor_Sets_AffectedBy_And_Now()
        {
            using (VigilContext context = new VigilContext(testUser, now))
            {
                Assert.AreEqual(testUser, context.AffectedBy);
                Assert.AreEqual(now, context.Now);
            }
        }

        [TestMethod]
        public void Validate_VigilUser_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser, now))
            {
                tableName = GetTableName<VigilUser>(context);
            }
            Assert.AreEqual("[vigil].[VigilUser]", tableName);
        }

        [TestMethod]
        public void Validate_VigilUserClaim_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser, now))
            {
                tableName = GetTableName<VigilUserClaim>(context);
            }
            Assert.AreEqual("[vigil].[VigilUserClaim]", tableName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login"), TestMethod]
        public void Validate_VigilUserLogin_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser, now))
            {
                tableName = GetTableName<VigilUserLogin>(context);
            }
            Assert.AreEqual("[vigil].[VigilUserLogin]", tableName);
        }

        [TestMethod]
        public void Validate_VigilUserRole_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser, now))
            {
                tableName = GetTableName<VigilUserRole>(context);
            }
            Assert.AreEqual("[vigil].[VigilUserRole]", tableName);
        }

        [TestMethod]
        public void Validate_VigilRole_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser, now))
            {
                tableName = GetTableName<VigilRole>(context);
            }
            Assert.AreEqual("[vigil].[VigilRole]", tableName);
        }

        [TestMethod]
        public void Set_Method_Returns_Valid_Objects()
        {
            using (VigilContext context = new VigilContext(testUser, now))
            {
                Assert.IsInstanceOfType(context.Set<VigilUser>(), typeof(IDbSet<VigilUser>));
            }
        }

        private static string GetTableName<TEntity>(DbContext context) where TEntity : class
        {
            ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
            var sql = objContext.CreateObjectSet<TEntity>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            return match.Groups["table"].Value;
        }
    }
}
