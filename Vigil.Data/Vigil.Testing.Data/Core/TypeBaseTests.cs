﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Vigil.Testing.Data.TestClasses;

namespace Vigil.Testing.Data.Core
{
    [TestClass]
    public class TypeBaseTests
    {
        [TestMethod]
        public void Constructor_Sets_TypeName_Property()
        {
            TypeBase tb = TestTypeBase.CreateType("test type");

            Assert.AreEqual("test type", tb.TypeName);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Passing_Empty_TypeName_Throws_Exception()
        {
            TestTypeBase.CreateType(String.Empty);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Passing_Null_TypeName_Throws_Exception()
        {
            TestTypeBase.CreateType(null);
        }
        [TestMethod]
        public void MarkDeleted_Sets_Deleted_Properties()
        {
            VigilUser testUser = new VigilUser{ UserName = "Test User"};
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TypeBase tb = TestTypeBase.CreateType("test type");
            bool marked = tb.MarkDeleted(testUser, now);

            Assert.IsTrue(marked);
            Assert.AreEqual(now, tb.DeletedOn);
            Assert.AreEqual(testUser, tb.DeletedBy);
        }
        [TestMethod]
        public void MarkDeleted_Returns_False_If_Deleted_Properties_Are_Already_Set()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TypeBase tb = TestTypeBase.CreateType("test type");
            bool initialMarked = tb.MarkDeleted(testUser, now);
            bool triedAgain = tb.MarkDeleted(testUser, now.AddDays(1));

            Assert.IsTrue(initialMarked);
            Assert.IsFalse(triedAgain);
            Assert.AreEqual(now, tb.DeletedOn);
            Assert.AreEqual(testUser, tb.DeletedBy);
        }
        [TestMethod]
        public void MarkModified_Sets_Modified_Properties()
        {
            VigilUser testUser = new VigilUser { UserName = "Test User" };
            DateTime now = new DateTime(2015, 5, 12, 12, 17, 00, DateTimeKind.Utc);
            TypeBase tb = TestTypeBase.CreateType("test type");
            tb.MarkModified(testUser, now);

            Assert.AreEqual(now, tb.ModifiedOn);
            Assert.AreEqual(testUser, tb.ModifiedBy);
        }
    }
}
