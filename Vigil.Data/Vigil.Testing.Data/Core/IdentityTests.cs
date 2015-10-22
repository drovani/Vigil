using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class IdentityTests
    {
        private class TestIdentity : KeyIdentity
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
            IEquatable<KeyIdentity> a = new TestIdentity(Guid.NewGuid());
            TestIdentity b = new TestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Identity_IEquality_Fails_For_Null_Other()
        {
            IEquatable<KeyIdentity> a = new TestIdentity(Guid.NewGuid());
            string test = null;
            Assert.False(a.Equals(test));
        }

        [Fact]
        public void Identity_IEquality_Passes_For_Reflective_Equality()
        {
            IEquatable<KeyIdentity> test = new TestIdentity(Guid.NewGuid());

            Assert.True(test.Equals(test));
        }
        
        [Fact]
        public void Identity_IEquality_Passes_For_Equal_Id_Values()
        {
            Guid id = Guid.NewGuid();

            IEquatable<KeyIdentity> source = new TestIdentity(id);
            IEquatable<KeyIdentity> other = new TestIdentity(id);

            Assert.True(source.Equals(other));
        }
        
        [Fact]
        public void Identity_IEquality_Fails_For_Other_Type()
        {
            IEquatable<KeyIdentity> a = new TestIdentity(Guid.NewGuid());

            Assert.False(a.Equals(0));
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

        [Fact]
        public void Identity_HashCode_Matches_HashCode_Of_Id()
        {
            TestIdentity test = new TestIdentity();

            Assert.Equal(test.Id.GetHashCode(), test.GetHashCode());
        }

        [Fact]
        public void Identity_ToString_Matches_ToString_Of_Id()
        {
            TestIdentity test = new TestIdentity();

            Assert.Equal(test.Id.ToString(), test.ToString());
        }
    }
}
