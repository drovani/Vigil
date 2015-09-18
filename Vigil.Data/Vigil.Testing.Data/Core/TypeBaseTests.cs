using System;
using Moq;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    public class TypeBaseTests
    {
        [Fact]
        public void Constructor_Sets_TypeName_Property()
        {
            TypeBase tb = Mock.Of<TypeBase>();

            Assert.Equal("test type", tb.TypeName);
        }
        [Fact]
        public void SetTypeName_Sets_TypeName_Property()
        {
            TypeBase tb = Mock.Of<TypeBase>();
            tb.SetTypeName("reset type");

            Assert.Equal("reset type", tb.TypeName);
        }
        [Fact]
        public void SetTypeName_To_Empty_Throws_Exception()
        {
            TypeBase tb = Mock.Of<TypeBase>();
            Assert.Throws<ArgumentException>(() => tb.SetTypeName(String.Empty));
        }
        [Fact]
        public void SetTypeName_To_Null_Throws_Exception()
        {
            TypeBase tb = Mock.Of<TypeBase>();
            Assert.Throws<ArgumentNullException>(() => tb.SetTypeName(null));
        }

        [Fact]
        public void MarkDeleted_Sets_Deleted_Properties()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TypeBase tb = Mock.Of<TypeBase>();
            bool marked = tb.MarkDeleted(testUser, now);

            Assert.True(marked);
            Assert.Equal(now, tb.DeletedOn);
            Assert.Equal(testUser, tb.DeletedBy);
        }
        [Fact]
        public void MarkDeleted_Returns_False_If_Deleted_Properties_Are_Already_Set()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TypeBase tb = Mock.Of<TypeBase>();
            bool initialMarked = tb.MarkDeleted(testUser, now);
            bool triedAgain = tb.MarkDeleted(testUser, now.AddDays(1));

            Assert.True(initialMarked);
            Assert.False(triedAgain);
            Assert.Equal(now, tb.DeletedOn);
            Assert.Equal(testUser, tb.DeletedBy);
        }
        [Fact]
        public void MarkModified_Sets_Modified_Properties()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TypeBase tb = Mock.Of<TypeBase>();
            tb.MarkModified(testUser, now);

            Assert.Equal(now, tb.ModifiedOn);
            Assert.Equal(testUser, tb.ModifiedBy);
        }
    }
}
