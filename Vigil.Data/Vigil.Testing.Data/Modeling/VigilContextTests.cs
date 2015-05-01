using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
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
            using (VigilContext context = new VigilContext())
            {
                context.Database.Initialize(true);
                if (context.Database.Exists())
                {
                    context.Database.Delete();
                }
                context.Database.Create();

                Assert.IsTrue(context.Database.Exists());
            }
        }

        [TestMethod]
        public void Explicit_Constructor_Sets_AffectedBy_And_Now()
        {
            VigilUser testUser = new VigilUser { UserName = "TestUser" };

            DateTime now = new DateTime(2015, 4, 23, 13, 33, 12, DateTimeKind.Utc);
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
            using (VigilContext context = new VigilContext())
            {
                tableName = GetTableName<VigilUser>(context);
            }
            Assert.AreEqual("[vigil].[VigilUser]", tableName);
        }

        [TestMethod]
        public void Validate_VigilUserClaim_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext())
            {
                tableName = GetTableName<VigilUserClaim>(context);
            }
            Assert.AreEqual("[vigil].[VigilUserClaim]", tableName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login"), TestMethod]
        public void Validate_VigilUserLogin_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext())
            {
                tableName = GetTableName<VigilUserLogin>(context);
            }
            Assert.AreEqual("[vigil].[VigilUserLogin]", tableName);
        }
        
        [TestMethod]
        public void Validate_VigilUserRole_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext())
            {
                tableName = GetTableName<VigilUserRole>(context);
            }
            Assert.AreEqual("[vigil].[VigilUserRole]", tableName);
        }
        
        [TestMethod]
        public void Validate_VigilRole_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext())
            {
                tableName = GetTableName<VigilRole>(context);
            }
            Assert.AreEqual("[vigil].[VigilRole]", tableName);
        }
        
        [TestMethod]
        public void Set_Method_Returns_Valid_Objects()
        {
            VigilUser testUser = new VigilUser { UserName = "TestUser" };
            DateTime now = new DateTime(2015, 04, 29, 11, 51, 14, DateTimeKind.Utc);
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
