using System;
using Moq;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Xunit;

namespace Vigil.Testing.Data.Core.System
{
    public class ChangeLogTests
    {
        [Fact]
        public void CreateLog_With_Invalid_Expression_Throws_Exception()
        {
            Identity mockedIdentity = Mock.Of<Identity>();
            Guid newGuid = Guid.NewGuid();
            Mock.Get(mockedIdentity).SetupGet(mi => mi.Id).Returns(newGuid);

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<Identity, Guid>(mockedIdentity.Id, i => new Guid(), Guid.Empty, mockedIdentity.Id));
        }
        [Fact]
        public void CreateLog_With_Invalid_MemberExpression_Throws_Exception()
        {
            Identity mockedIdentity = Mock.Of<Identity>();
            Guid newGuid = Guid.NewGuid();
            Mock.Get(mockedIdentity).SetupGet(mi => mi.Id).Returns(newGuid);

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<Identity, Guid>(mockedIdentity.Id, i => Guid.Empty, Guid.Empty, mockedIdentity.Id));
        }
        [Fact]
        public void Passing_Null_Values_Saves_Null_Values()
        {
            TypeBase mockedTypeBase = Mock.Of<TypeBase>();
            Mock.Get(mockedTypeBase).SetupGet(tb => tb.TypeName).Returns("Test Type");
            ChangeLog log = ChangeLog.CreateLog<TypeBase, DateTime?>(mockedTypeBase, t => t.DeletedOn, null, null);

            Assert.Null(log.OldValue);
            Assert.Null(log.NewValue);
        }
        [Fact]
        public void CreateLog_By_Identifier_Sets_Properties_Correctly()
        {
            Identity ident = Mock.Of<Identity>();
            Guid newGuid = Guid.NewGuid();
            Mock.Get(ident).SetupGet(mi => mi.Id).Returns(newGuid);

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(ident.Id, i => i.Id, Guid.Empty, ident.Id);
            Assert.Equal(ident.Id, log.SourceId);
            Assert.Equal("Identity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(newGuid.ToString(), log.NewValue);
        }

        [Fact]
        public void CreateLog_By_Entity_Sets_Properties_Correctly()
        {
            Identity ident = Mock.Of<Identity>();
            Guid newGuid = Guid.NewGuid();
            Mock.Get(ident).SetupGet(mi => mi.Id).Returns(newGuid);

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(ident, i => i.Id, Guid.Empty, ident.Id);
            Assert.Equal(ident.Id, log.SourceId);
            Assert.Equal("Identity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(ident.Id.ToString(), log.NewValue);
        }
    }
}
