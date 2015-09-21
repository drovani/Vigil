using System;
using Moq;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Xunit;

namespace Vigil.Testing.Data.Core.System
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class ChangeLogTests
    {
        [Fact]
        public void CreateLog_With_Invalid_Expression_Throws_Exception()
        {
            Identity testIdentity = new Mock<Identity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<Identity, Guid>(testIdentity.Id, i => new Guid(), Guid.Empty, testIdentity.Id));
        }
        [Fact]
        public void CreateLog_With_Invalid_MemberExpression_Throws_Exception()
        {
            Identity testIdentity = new Mock<Identity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<Identity, Guid>(testIdentity.Id, i => Guid.Empty, Guid.Empty, testIdentity.Id));
        }
        [Fact]
        public void Passing_Null_Values_Saves_Null_Values()
        {
            TypeBase testTypeBase = new Mock<TypeBase>("Test Type")
            {
                CallBase = true
            }.Object;
            ChangeLog log = ChangeLog.CreateLog<TypeBase, DateTime?>(testTypeBase, t => t.DeletedOn, null, null);

            Assert.Null(log.OldValue);
            Assert.Null(log.NewValue);
        }
        [Fact]
        public void CreateLog_By_Identifier_Sets_Properties_Correctly()
        {
            Identity testIdentity = new Mock<Identity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(testIdentity.Id, i => i.Id, Guid.Empty, testIdentity.Id);
            Assert.Equal(testIdentity.Id, log.SourceId);
            Assert.Equal("Identity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(testIdentity.Id.ToString(), log.NewValue);
        }

        [Fact]
        public void CreateLog_By_Entity_Sets_Properties_Correctly()
        {
            Identity testIdentity = new Mock<Identity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(testIdentity, i => i.Id, Guid.Empty, testIdentity.Id);
            Assert.Equal(testIdentity.Id, log.SourceId);
            Assert.Equal("Identity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(testIdentity.Id.ToString(), log.NewValue);
        }
    }
}
