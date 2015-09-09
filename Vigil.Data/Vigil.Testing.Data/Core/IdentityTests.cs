using System;
using Vigil.Testing.Data.TestClasses;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class IdentityTests
    {
        [Fact]
        public void Identity_Equality_Fails_For_Unequal_Ids()
        {
            var a = TestIdentity.CreateTestIdentity(Guid.NewGuid());
            var b = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Identity_Equality_Fails_For_Null_Other()
        {
            var a = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(null));
        }

        [Fact]
        public void Identity_Equality_Passes_For_Equal_Id_Values()
        {
            Guid id = Guid.NewGuid();
            var a = TestIdentity.CreateTestIdentity(id);
            var b = TestIdentity.CreateTestIdentity(id);

            Assert.True(a.Equals(b));
        }
    }
}
