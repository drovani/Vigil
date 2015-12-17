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

            InterfaceExtensionMethods.MarkModified(modded, "ModifyingUser", now);

            Assert.Equal("ModifyingUser", modded.ModifiedBy);
            Assert.Equal(now, modded.ModifiedOn);
        }

        [Fact]
        public void MarkDeleted_Sets_DeletedBy_and_DeletedOn_Properties()
        {
            Deleted deled = new Deleted() { CreatedBy = "CreateUser", CreatedOn = DateTime.UtcNow.AddDays(-1) };
            DateTime now = DateTime.UtcNow;

            InterfaceExtensionMethods.MarkDeleted(deled, "DeletingUser", now);

            Assert.Equal("DeletingUser", deled.DeletedBy);
            Assert.Equal(now, deled.DeletedOn);
        }
    }
}
