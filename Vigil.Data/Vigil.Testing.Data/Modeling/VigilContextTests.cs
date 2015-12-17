using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;
using Vigil.Data.Modeling;
using Xunit;

namespace Vigil.Testing.Data.Modeling
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class VigilContextTests
    {
        private readonly VigilUser testUser;
        private readonly DateTime now;

        public VigilContextTests()
        {
            testUser = new VigilUser { UserName = "TestUser" };
            now = new DateTime(2015, 4, 23, 13, 33, 12, DateTimeKind.Utc);
        }

        [Fact]
        public void Initialize_And_Create_Database_Cause_Database_To_Exists()
        {
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                Database.SetInitializer<VigilContext>(new DropCreateDatabaseAlways<VigilContext>());
                context.Database.Initialize(true);

                Assert.True(context.Database.Exists());
            }
        }

        [Fact]
        public void Explicit_Constructor_Sets_AffectedBy_And_Now()
        {
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                Assert.Equal(testUser.UserName, context.AffectedBy);
                Assert.Equal(now, context.Now);
            }
        }

        [Fact]
        public void Validate_VigilUser_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                tableName = GetTableName<VigilUser>(context);
            }
            Assert.Equal("[vigil].[VigilUser]", tableName);
        }

        [Fact]
        public void Validate_VigilUserClaim_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                tableName = GetTableName<VigilUserClaim>(context);
            }
            Assert.Equal("[vigil].[VigilUserClaim]", tableName);
        }

        public void Validate_VigilUserLogin_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                tableName = GetTableName<VigilUserLogin>(context);
            }
            Assert.Equal("[vigil].[VigilUserLogin]", tableName);
        }

        [Fact]
        public void Validate_VigilUserRole_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                tableName = GetTableName<VigilUserRole>(context);
            }
            Assert.Equal("[vigil].[VigilUserRole]", tableName);
        }

        [Fact]
        public void Validate_VigilRole_TableName_Is_Correct()
        {
            string tableName;
            using (VigilContext context = new VigilContext(testUser.UserName, now))
            {
                tableName = GetTableName<VigilRole>(context);
            }
            Assert.Equal("[vigil].[VigilRole]", tableName);
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
