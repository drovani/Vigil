using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.Attributes;

namespace Vigil.Testing.Data.Vigil.Data.Core
{
    [TestClass]
    public class CodeCoverageUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void NotImplementedAttribute_Throws_NotImplementedException()
        {
            var nia = new NotImplementedAttribute();

            Assert.Fail("Constructor should have throw Not Implemented Exception.");
        }
    }
}
