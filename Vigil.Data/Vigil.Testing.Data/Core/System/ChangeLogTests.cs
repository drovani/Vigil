using System;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Vigil.Testing.Data.TestClasses;
using Xunit;

namespace Vigil.Testing.Data.Core.System
{
    public class ChangeLogTests
    {
        [Fact]
        public void CreateLog_With_Invalid_Expression_Throws_Exception()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<TestIdentity, Guid>(ident.Id, i => new Guid(), Guid.Empty, ident.Id));
        }
        [Fact]
        public void CreateLog_With_Invalid_MemberExpression_Throws_Exception()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<TestIdentity, Guid>(ident.Id, i => Guid.Empty, Guid.Empty, ident.Id));
        }
        [Fact]
        public void Passing_Null_Values_Saves_Null_Values()
        {
            TypeBase tb = TestTypeBase.CreateType("test type");
            ChangeLog log = ChangeLog.CreateLog<TypeBase, DateTime?>(tb, t => t.DeletedOn, null, null);

            Assert.Null(log.OldValue);
            Assert.Null(log.NewValue);
        }
        [Fact]
        public void CreateLog_By_Identifier_Sets_Properties_Correctly()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(ident.Id, i => i.Id, Guid.Empty, ident.Id);
            Assert.Equal(ident.Id, log.SourceId);
            Assert.Equal("Identity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(ident.Id.ToString(), log.NewValue);
        }

        [Fact]
        public void CreateLog_By_Entity_Sets_Properties_Correctly()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(ident, i => i.Id, Guid.Empty, ident.Id);
            Assert.Equal(ident.Id, log.SourceId);
            Assert.Equal("Identity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(ident.Id.ToString(), log.NewValue);
        }
    }
}
