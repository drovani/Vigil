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
            Identity a = new Mock<Identity>(Guid.NewGuid()).Object;
            Identity b = new Mock<Identity>(Guid.NewGuid()).Object;

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

            Identity source = new Mock<Identity>(id).Object;
            Identity other = new Mock<Identity>(id).Object;

            Assert.True(source.Equals(other));
        }
    }
}
