using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class ExtensionMethodsTests
    {
        private class Modified : IModified
        {
            public string CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public string ModifiedBy { get; protected set; }
            public DateTime? ModifiedOn { get; protected set; }
        }

        private class Deleted : IDeleted
        {
            public string CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public string DeletedBy { get; protected set; }
            public DateTime? DeletedOn { get; protected set; }
        }

        [Fact]
        public void MarkModified_Sets_ModifiedBy_and_ModifiedOn_Properties()
        {
            Modified modded = new Modified() { CreatedBy = "CreateUser", CreatedOn = DateTime.UtcNow.AddDays(-1) };
            DateTime now = DateTime.UtcNow;

            bool changed = InterfaceExtensionMethods.MarkModified(modded, "ModifyingUser", now);

            Assert.True(changed);
            Assert.Equal("ModifyingUser", modded.ModifiedBy);
            Assert.Equal(now, modded.ModifiedOn);
        }

        [Fact]
        public void MarkDeleted_Sets_DeletedBy_and_DeletedOn_Properties()
        {
            Deleted deled = new Deleted() { CreatedBy = "CreateUser", CreatedOn = DateTime.UtcNow.AddDays(-1) };
            DateTime now = DateTime.UtcNow;

            bool changed = InterfaceExtensionMethods.MarkDeleted(deled, "DeletingUser", now);

            Assert.True(changed);
            Assert.Equal("DeletingUser", deled.DeletedBy);
            Assert.Equal(now, deled.DeletedOn);
        }

        [Fact]
        public void MarkDeleted_Second_Time_Makes_No_Changes()
        {
            Deleted deled = new Deleted() { CreatedBy = "CreateUser", CreatedOn = DateTime.UtcNow.AddDays(-1) };
            DateTime now = DateTime.UtcNow;
            bool changed = InterfaceExtensionMethods.MarkDeleted(deled, "DeletingUser", now);
            bool notChanged = InterfaceExtensionMethods.MarkDeleted(deled, "ShouldNotDeleteUser", now.AddDays(1));

            Assert.True(changed);
            Assert.False(notChanged);
            Assert.Equal("DeletingUser", deled.DeletedBy);
            Assert.Equal(now, deled.DeletedOn);
        }
    }
}
