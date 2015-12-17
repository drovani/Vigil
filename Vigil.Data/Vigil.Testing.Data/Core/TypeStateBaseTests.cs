using System;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class TypeStateBaseTests
    {
        private class TestTypeStateBase : TypeStateBase
        {
            public TestTypeStateBase(string typeName)
                : base(typeName)
            {
            }
        }

        [Fact]
        public void Constructor_Sets_TypeName_Property()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            Assert.Equal("test type", typeBase.TypeName);
        }
        [Fact]
        public void SetTypeName_Sets_TypeName_Property()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            Assert.Equal("test type", typeBase.TypeName);

            typeBase.SetTypeName("reset type");

            Assert.Equal("reset type", typeBase.TypeName);
        }
        [Fact]
        public void SetTypeName_To_Empty_Throws_Exception()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            Assert.Throws<ArgumentException>(() => typeBase.SetTypeName(String.Empty));
        }
        [Fact]
        public void SetTypeName_To_Null_Throws_Exception()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            Assert.Throws<ArgumentNullException>(() => typeBase.SetTypeName(null));
        }
    }
}
