using System;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class PatronStateTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            PatronTypeState patronType = PatronTypeState.Create("PatronTypeName");
            PatronState patron = PatronState.Create(patronType, "PatronDisplayName");

            Assert.Same(patronType, patron.PatronType);
            Assert.Equal("PatronDisplayName", patron.DisplayName);
            Assert.Null(patron.AccountNumber);
            Assert.False(patron.IsAnonymous);
        }

        [Fact]
        public void Create_Method_Sets_Properties()
        {
            PatronTypeState patronType = PatronTypeState.Create("PatronTypeName");
            PatronState patron = PatronState.Create(patronType, "PatronDisplayName", "0123456", true);

            Assert.Same(patronType, patron.PatronType);
            Assert.Equal("PatronDisplayName", patron.DisplayName);
            Assert.Equal("0123456", patron.AccountNumber);
            Assert.True(patron.IsAnonymous);
        }

        [Fact]
        public void MarkDeleted_Sets_Delete_Properties()
        {
            PatronTypeState patronType = PatronTypeState.Create("PatronTypeName");
            PatronState patron = PatronState.Create(patronType, "PatronDisplayName", "0123456", true);
            DateTime now = DateTime.UtcNow;
            VigilUser newUser = new VigilUser();
            patron.MarkDeleted(newUser, now);

            Assert.Same(newUser, patron.DeletedBy);
            Assert.Equal(now, patron.DeletedOn);
        }

        [Fact]
        public void MarkModified_Sets_Delete_Properties()
        {
            PatronTypeState patronType = PatronTypeState.Create("PatronTypeName");
            PatronState patron = PatronState.Create(patronType, "PatronDisplayName", "0123456", true);
            DateTime now = DateTime.UtcNow;
            VigilUser newUser = new VigilUser();
            patron.MarkModified(newUser, now);

            Assert.Same(newUser, patron.ModifiedBy);
            Assert.Equal(now, patron.ModifiedOn);
        }
    }
}
