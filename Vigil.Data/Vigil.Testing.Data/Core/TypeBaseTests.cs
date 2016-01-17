using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class TypeBaseTests
    {
        private class TestTypeBase : TypeBase
        {
            public TestTypeBase(string typeName)
                : base(typeName)
            {
            }
        }

        [Fact]
        public void Constructor_Sets_TypeName_Property()
        {
            TestTypeBase typeBase = new TestTypeBase("test type");
            Assert.Equal("test type", typeBase.TypeName);
        }
        [Fact]
        public void SetTypeName_Sets_TypeName_Property()
        {
            TestTypeBase typeBase = new TestTypeBase("test type");
            Assert.Equal("test type", typeBase.TypeName);

            typeBase.SetTypeName("reset type");

            Assert.Equal("reset type", typeBase.TypeName);
        }
        [Fact]
        public void SetTypeName_To_Empty_Throws_Exception()
        {
            TestTypeBase typeBase = new TestTypeBase("test type");
            Assert.Throws<ArgumentException>(() => typeBase.SetTypeName(String.Empty));
        }
        [Fact]
        public void SetTypeName_To_Null_Throws_Exception()
        {
            TestTypeBase typeBase = new TestTypeBase("test type");
            Assert.Throws<ArgumentNullException>(() => typeBase.SetTypeName(null));
        }
    }
}
