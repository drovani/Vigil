using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class CodeCoverageUnitTests
    {
        [Fact]
        public void NotImplementedAttribute_Throws_NotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => new NotImplementedAttribute());
        }
    }
}
