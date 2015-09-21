using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class NotImplementedAttributeTests
    {
        [Fact]
        public void NotImplementedAttribute_Constructor_Throw_NotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => new NotImplementedAttribute());
        }
    }
}
