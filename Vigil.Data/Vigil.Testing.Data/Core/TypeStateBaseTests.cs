using System;
using Vigil.Data.Core;
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

        [Fact]
        public void MarkDeleted_Sets_Deleted_Properties()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            bool marked = typeBase.MarkDeleted(testUser, now);

            Assert.True(marked);
            Assert.Equal(now, typeBase.DeletedOn);
            Assert.Equal(testUser, typeBase.DeletedBy);
        }
        [Fact]
        public void MarkDeleted_Returns_False_If_Deleted_Properties_Are_Already_Set()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            bool initialMarked = typeBase.MarkDeleted(testUser, now);
            bool triedAgain = typeBase.MarkDeleted(testUser, now.AddDays(1));

            Assert.True(initialMarked);
            Assert.False(triedAgain);
            Assert.Equal(now, typeBase.DeletedOn);
            Assert.Equal(testUser, typeBase.DeletedBy);
        }
        [Fact]
        public void MarkModified_Sets_Modified_Properties()
        {
            TestTypeStateBase typeBase = new TestTypeStateBase("test type");
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            typeBase.MarkModified(testUser, now);

            Assert.Equal(now, typeBase.ModifiedOn);
            Assert.Equal(testUser, typeBase.ModifiedBy);
        }
    }
}
