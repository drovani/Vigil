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
        public DateTime Now { get; } = DateTime.UtcNow;

        [Fact]
        public void CreateLog_With_Invalid_Expression_Throws_Exception()
        {
            KeyIdentity testIdentity = new Mock<KeyIdentity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<KeyIdentity, Guid>(testIdentity.Id, i => new Guid(), Guid.Empty, testIdentity.Id, "TestChange", Now));
        }
        [Fact]
        public void CreateLog_With_Invalid_MemberExpression_Throws_Exception()
        {
            KeyIdentity testIdentity = new Mock<KeyIdentity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            Assert.Throws<ArgumentException>(() => ChangeLog.CreateLog<KeyIdentity, Guid>(testIdentity.Id, i => Guid.Empty, Guid.Empty, testIdentity.Id, "TestChange", Now));
        }
        [Fact]
        public void Passing_Null_Values_Saves_Null_Values()
        {
            TypeBase testTypeBase = new Mock<TypeBase>("Test Type")
            {
                CallBase = true
            }.Object;
            ChangeLog log = ChangeLog.CreateLog(testTypeBase, t => t.DeletedOn, null, null, "TestChanger", Now);

            Assert.Null(log.OldValue);
            Assert.Null(log.NewValue);
            Assert.Equal("TestChanger", log.CreatedBy);
            Assert.Equal(Now, log.CreatedOn);
        }
        [Fact]
        public void CreateLog_By_Identifier_Sets_Properties_Correctly()
        {
            KeyIdentity testIdentity = new Mock<KeyIdentity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            ChangeLog log = ChangeLog.CreateLog<KeyIdentity, Guid>(testIdentity.Id, i => i.Id, Guid.Empty, testIdentity.Id, "TestChanger", Now);
            Assert.Equal(testIdentity.Id, log.EntityId);
            Assert.Equal("KeyIdentity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(testIdentity.Id.ToString(), log.NewValue);
            Assert.Equal("TestChanger", log.CreatedBy);
            Assert.Equal(Now, log.CreatedOn);
        }

        [Fact]
        public void CreateLog_By_Entity_Sets_Properties_Correctly()
        {
            KeyIdentity testIdentity = new Mock<KeyIdentity>(Guid.NewGuid())
            {
                CallBase = true
            }.Object;

            ChangeLog log = ChangeLog.CreateLog(testIdentity, i => i.Id, Guid.Empty, testIdentity.Id, "TestChanger", Now);
            Assert.Equal(testIdentity.Id, log.EntityId);
            Assert.Equal("KeyIdentity", log.ModelName);
            Assert.Equal("Id", log.PropertyName);
            Assert.Equal(Guid.Empty.ToString(), log.OldValue);
            Assert.Equal(testIdentity.Id.ToString(), log.NewValue);
            Assert.Equal("TestChanger", log.CreatedBy);
            Assert.Equal(Now, log.CreatedOn);
        }
    }
}
