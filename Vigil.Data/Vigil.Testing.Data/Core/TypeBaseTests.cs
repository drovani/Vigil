using System;
using Moq;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class TypeBaseTests
    {
        private TypeBase TestTypeBase;

        public TypeBaseTests()
        {
            TestTypeBase = new Mock<TypeBase>(" test type ")
            {
                CallBase = true
            }.Object;
        }

        [Fact]
        public void Constructor_Sets_TypeName_Property()
        {
            Assert.Equal("test type", TestTypeBase.TypeName);
        }
        [Fact]
        public void SetTypeName_Sets_TypeName_Property()
        {
            Assert.Equal("test type", TestTypeBase.TypeName);

            TestTypeBase.SetTypeName("reset type");

            Assert.Equal("reset type", TestTypeBase.TypeName);
        }
        [Fact]
        public void SetTypeName_To_Empty_Throws_Exception()
        {
            Assert.Throws<ArgumentException>(() => TestTypeBase.SetTypeName(String.Empty));
        }
        [Fact]
        public void SetTypeName_To_Null_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => TestTypeBase.SetTypeName(null));
        }

        [Fact]
        public void MarkDeleted_Sets_Deleted_Properties()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            bool marked = TestTypeBase.MarkDeleted(testUser, now);

            Assert.True(marked);
            Assert.Equal(now, TestTypeBase.DeletedOn);
            Assert.Equal(testUser, TestTypeBase.DeletedBy);
        }
        [Fact]
        public void MarkDeleted_Returns_False_If_Deleted_Properties_Are_Already_Set()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            bool initialMarked = TestTypeBase.MarkDeleted(testUser, now);
            bool triedAgain = TestTypeBase.MarkDeleted(testUser, now.AddDays(1));

            Assert.True(initialMarked);
            Assert.False(triedAgain);
            Assert.Equal(now, TestTypeBase.DeletedOn);
            Assert.Equal(testUser, TestTypeBase.DeletedBy);
        }
        [Fact]
        public void MarkModified_Sets_Modified_Properties()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TestTypeBase.MarkModified(testUser, now);

            Assert.Equal(now, TestTypeBase.ModifiedOn);
            Assert.Equal(testUser, TestTypeBase.ModifiedBy);
        }
    }
}
