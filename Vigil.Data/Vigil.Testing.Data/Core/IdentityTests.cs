using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class IdentityTests
    {
        private class TestIdentity : Identity
        {
            public TestIdentity() : base() { }
            public TestIdentity(Guid id) : base(id) { }
        }

        [Fact]
        public void Constructor_Generates_NewGuid()
        {
            TestIdentity test = new TestIdentity();

            Assert.NotEqual(Guid.Empty, test.Id);
        }

        [Fact]
        public void Identity_IEquality_Fails_For_Unequal_Ids()
        {
            IEquatable<Identity> a = new TestIdentity(Guid.NewGuid());
            TestIdentity b = new TestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Identity_IEquality_Fails_For_Null_Other()
        {
            IEquatable<Identity> a = new TestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(null));
        }


        [Fact]
        public void Identity_IEquality_Fails_For_Other_Type()
        {
            IEquatable<Identity> a = new TestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(0));
        }

        [Fact]
        public void Identity_IEquality_Passes_For_Equal_Id_Values()
        {
            Guid id = Guid.NewGuid();

            IEquatable<Identity> source = new TestIdentity(id);
            TestIdentity other = new TestIdentity(id);

            Assert.True(source.Equals(other));
        }

        [Fact]
        public void Identity_Equality_Fails_For_Unequal_Ids()
        {
            TestIdentity a = new TestIdentity(Guid.NewGuid());
            TestIdentity b = new TestIdentity(Guid.NewGuid());

            Assert.NotEqual(a.Id, b.Id);
            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Identity_Equality_Fails_For_Null_Other()
        {
            TestIdentity a = new TestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(null));
        }

        [Fact]
        public void Identity_Equality_Passes_For_Equal_Id_Values()
        {
            Guid id = Guid.NewGuid();

            TestIdentity source = new TestIdentity(id);
            TestIdentity other = new TestIdentity(id);

            Assert.True(source.Equals(other));
        }
    }
}
