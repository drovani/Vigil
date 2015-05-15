using System;
using System.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Vigil.Testing.Data.TestClasses;

namespace Vigil.Testing.Data.Core.System
{
    [TestClass]
    [ContractVerification(false)]
    public class ChangeLogTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateLog_With_Invalid_Expression_Throws_Exception()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog.CreateLog<TestIdentity, Guid>(ident.Id, i => new Guid(), Guid.Empty, ident.Id);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateLog_With_Invalid_MemberExpression_Throws_Exception()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog.CreateLog<TestIdentity, Guid>(ident.Id, i => Guid.Empty, Guid.Empty, ident.Id);
        }
        [TestMethod]
        public void Passing_Null_Values_Saves_Null_Values()
        {
            TypeBase tb = TestTypeBase.CreateType("test type");
            ChangeLog log = ChangeLog.CreateLog<TypeBase, DateTime?>(tb, t => t.DeletedOn, null, null);

            Assert.IsNull(log.OldValue);
            Assert.IsNull(log.NewValue);
        }
        [TestMethod]
        public void CreateLog_By_Identifier_Sets_Properties_Correctly()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(ident.Id, i => i.Id, Guid.Empty, ident.Id);
            Assert.AreEqual(ident.Id, log.SourceId);
            Assert.AreEqual("Identity", log.ModelName);
            Assert.AreEqual("Id", log.PropertyName);
            Assert.AreEqual(Guid.Empty.ToString(), log.OldValue);
            Assert.AreEqual(ident.Id.ToString(), log.NewValue);
        }

        [TestMethod]
        public void CreateLog_By_Entity_Sets_Properties_Correctly()
        {
            Identity ident = TestIdentity.CreateTestIdentity(Guid.NewGuid());

            ChangeLog log = ChangeLog.CreateLog<Identity, Guid>(ident, i => i.Id, Guid.Empty, ident.Id);
            Assert.AreEqual(ident.Id, log.SourceId);
            Assert.AreEqual("Identity", log.ModelName);
            Assert.AreEqual("Id", log.PropertyName);
            Assert.AreEqual(Guid.Empty.ToString(), log.OldValue);
            Assert.AreEqual(ident.Id.ToString(), log.NewValue);
        }
    }
}
