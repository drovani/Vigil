using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
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
    }
}
