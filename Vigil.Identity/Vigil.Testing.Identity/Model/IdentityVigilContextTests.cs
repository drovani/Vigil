using System;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    public class IdentityVigilContextTests
    {
        [Fact]
        public void IdentityVigilContext_Default_Constructor_Sets_Now()
        {
            var context = new IdentityVigilContext();
            Assert.NotEqual(DateTime.MinValue, context.Now);
        }

        [Fact]
        public void IdentityVigilContext_Explicit_Contructor_Sets_Now_and_AffectedBy()
        {
            VigilUser affectedBy = new VigilUser(){ Id = Guid.NewGuid(), UserName = "TestUser" };
            DateTime now = new DateTime(2015, 7, 30, 15, 10, 12, DateTimeKind.Utc);
            var context = new IdentityVigilContext(affectedBy, now);
            Assert.Equal(affectedBy, context.AffectedBy);
            Assert.Equal(now, context.Now);
        }

        [Fact]
        public void SetAffectingUser_Sets_AffectedBy()
        {
            var context = new IdentityVigilContext();
            Assert.Null(context.AffectedBy);

            VigilUser affectedBy = new VigilUser() { Id = Guid.NewGuid(), UserName = "TestUser" };
            context.SetAffectingUser(affectedBy);
            Assert.Equal(affectedBy, context.AffectedBy);
        }

        [Fact]
        public void Create_Static_Method_Returns_New_IdentityVigilContext()
        {
            var context = IdentityVigilContext.Create();

            Assert.Null(context.AffectedBy);
            Assert.NotEqual(DateTime.MinValue, context.Now);
        }
    }
}
