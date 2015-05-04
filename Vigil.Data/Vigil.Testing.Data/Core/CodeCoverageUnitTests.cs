using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.Attributes;

namespace Vigil.Testing.Data.Core
{
    [TestClass]
    public class CodeCoverageUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Vigil.Data.Core.Attributes.NotImplementedAttribute")]
        public void NotImplementedAttribute_Throws_NotImplementedException()
        {
            new NotImplementedAttribute();

            Assert.Fail("Constructor should have throw Not Implemented Exception.");
        }
    }
}
