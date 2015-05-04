using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Testing.Data.TestClasses;

namespace Vigil.Testing.Data.Core.System
{
    [TestClass]
    public class ChangeLogTests
    {
        [TestMethod]
        public void CreateLog_By_Identifier_Sets_Properties_Correctly()
        {
            TestIdentity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog log = ChangeLog.CreateLog<TestIdentity, Guid>(ident.Id, i => i.Id, Guid.Empty, ident.Id);
            Assert.AreEqual(ident.Id, log.SourceId);
            Assert.AreEqual("ChangeLog", log.ModelName);
            Assert.AreEqual("Id", log.PropertyName);
            Assert.AreEqual(Guid.Empty.ToString(), log.OldValue);
            Assert.AreEqual(ident.Id.ToString(), log.NewValue);
        }

        [TestMethod]
        public void CreateLog_By_Entity_Sets_Properties_Correctly()
        {
            TestIdentity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog log = ChangeLog.CreateLog<TestIdentity, Guid>(ident, i => i.Id, Guid.Empty, ident.Id);
            Assert.AreEqual(ident.Id, log.SourceId);
            Assert.AreEqual("ChangeLog", log.ModelName);
            Assert.AreEqual("Id", log.PropertyName);
            Assert.AreEqual(Guid.Empty.ToString(), log.OldValue);
            Assert.AreEqual(ident.Id.ToString(), log.NewValue);
        }
    }
}
