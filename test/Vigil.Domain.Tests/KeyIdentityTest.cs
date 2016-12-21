using System;
using Xunit;

namespace Vigil.Domain
{
    public class KeyIdentityTest
    {
        protected class TestKeyIdentity : KeyIdentity {
            public TestKeyIdentity(Guid id)
            {
                Id = id;
            }
        }

        [Fact]
        public void ToString_Is_Guid()
        {
            TestKeyIdentity left = new TestKeyIdentity(Guid.NewGuid());

            Assert.Equal(left.Id.ToString(), left.ToString());
        }

        [Fact]
        public void Equal_Self()
        {
            TestKeyIdentity left = new TestKeyIdentity(Guid.NewGuid());

            Assert_Equal(left, left);
        }

        [Fact]
        public void Equal_Same_Ids()
        {
            Guid newGuid = Guid.NewGuid();
            TestKeyIdentity left = new TestKeyIdentity(newGuid);
            TestKeyIdentity right = new TestKeyIdentity(newGuid);

            Assert_Equal(left, right);
        }

        [Fact]
        public void NotEqual_Different_Ids()
        {
            TestKeyIdentity left = new TestKeyIdentity(Guid.NewGuid());
            TestKeyIdentity right = new TestKeyIdentity(Guid.NewGuid());

            Assert_NotEqual(left, right);
        }

        [Fact]
        public void NotEqual_Different_Type()
        {
            TestKeyIdentity left = new TestKeyIdentity(Guid.NewGuid());

            Assert.False(left.Equals(new object()));
        }

        [Fact]
        public void NotEqual_CompareWithNull()
        {
            TestKeyIdentity left = new TestKeyIdentity(Guid.NewGuid());

            Assert_NotEqual(left, null);
        }

        private void Assert_Equal(KeyIdentity left, KeyIdentity right)
        {
            Assert.Equal(left, right);
            Assert.True(left == right);
            Assert.False(left != right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
            Assert.True(left.Equals(right));
            Assert.True(left.Equals((object)right));
        }

        private void Assert_NotEqual(KeyIdentity left, KeyIdentity right)
        {
            Assert.NotEqual(left, right);

            Assert.False(left == right);
            Assert.True(left != right);

            Assert.False(left.Equals(right));
            Assert.False(left.Equals((object)right));

            if (right != null)
                Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }
    }
}
