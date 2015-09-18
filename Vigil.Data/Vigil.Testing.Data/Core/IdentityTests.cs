using System;
using Moq;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class IdentityTests
    {
        [Fact]
        public void Identity_Equality_Fails_For_Unequal_Ids()
        {
            Identity a = Mock.Of<Identity>();
            Identity b = Mock.Of<Identity>();

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Identity_Equality_Fails_For_Null_Other()
        {
            Identity a = Mock.Of<Identity>();

            Assert.False(a.Equals(null));
        }

        [Fact]
        public void Identity_Equality_Passes_For_Equal_Id_Values()
        {
            Guid id = Guid.NewGuid();
            Identity a = Mock.Of<Identity>();
            Identity b = Mock.Of<Identity>();

            Assert.True(a.Equals(b));
        }
    }
}
