using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
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
            VigilUser testUser = new VigilUser { UserName = "TestUser" };

            DateTime now = new DateTime(2015, 4, 23, 13, 33, 12, DateTimeKind.Utc);
            IVigilContext context = new VigilContext(testUser, now);

            Assert.AreEqual(testUser, context.AffectedBy);
            Assert.AreEqual(now, context.Now);
        }

        [TestMethod]
        public void Validate_VigilUser_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUser>(new VigilContext());
            
            Assert.AreEqual("[vigil].[VigilUser]", tableName);
        }

        [TestMethod]
        public void Validate_VigilUserClaim_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUserClaim>(new VigilContext());

            Assert.AreEqual("[vigil].[VigilUserClaim]", tableName);
        }
        
        [TestMethod]
        public void Validate_VigilUserLogin_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUserLogin>(new VigilContext());

            Assert.AreEqual("[vigil].[VigilUserLogin]", tableName);
        }
        
        [TestMethod]
        public void Validate_VigilUserRole_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilUserRole>(new VigilContext());

            Assert.AreEqual("[vigil].[VigilUserRole]", tableName);
        }
        
        [TestMethod]
        public void Validate_VigilRole_TableName_Is_Correct()
        {
            string tableName = GetTableName<VigilRole>(new VigilContext());

            Assert.AreEqual("[vigil].[VigilRole]", tableName);
        }
        
        [TestMethod]
        public void Set_Method_Returns_Valid_Objects()
        {
            VigilUser testUser = new VigilUser { UserName = "TestUser" };
            DateTime now = new DateTime(2015, 04, 29, 11, 51, 14, DateTimeKind.Utc);
            IVigilContext context = new VigilContext(testUser, now);

            Assert.IsInstanceOfType(context.Set<VigilUser>(), typeof(IDbSet<VigilUser>));
        }

        private string GetTableName<TEntity>(DbContext context) where TEntity : class
        {
            ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
            var sql = objContext.CreateObjectSet<TEntity>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            return match.Groups["table"].Value;
        }
    }
}
