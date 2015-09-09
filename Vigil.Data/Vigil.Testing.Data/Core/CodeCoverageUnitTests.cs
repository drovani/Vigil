using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Attributes;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    [ContractVerification(false)]
    public class CodeCoverageUnitTests
    {
        [Fact]
        public void NotImplementedAttribute_Throws_NotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => new NotImplementedAttribute());
        }
    }
}
