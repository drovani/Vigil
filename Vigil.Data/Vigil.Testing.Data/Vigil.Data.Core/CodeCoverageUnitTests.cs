using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.Attributes;

namespace Vigil.Testing.Data.Vigil.Data.Core
{
    [TestClass]
    public class CodeCoverageUnitTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Vigil.Data.Core.Attributes.NotImplementedAttribute"), TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void NotImplementedAttribute_Throws_NotImplementedException()
        {
            new NotImplementedAttribute();

            Assert.Fail("Constructor should have throw Not Implemented Exception.");
        }
    }
}
