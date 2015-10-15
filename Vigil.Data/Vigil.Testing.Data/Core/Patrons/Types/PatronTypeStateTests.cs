using Vigil.Data.Core.Patrons.Types;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons.Types
{
    public class PatronTypeStateTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            PatronTypeState patronType = PatronTypeState.Create("PatronTypeName");

            Assert.Equal("PatronTypeName", patronType.TypeName);
            Assert.Null(patronType.Description);
            Assert.Equal(0, patronType.Ordinal);
            Assert.Equal(true, patronType.IsOrganization);
        }

        [Fact]
        public void Create_Method_Sets_Properties()
        {
            PatronTypeState patronType = PatronTypeState.Create("PatronTypeName", "A description.", 1, false);

            Assert.Equal("PatronTypeName", patronType.TypeName);
            Assert.Equal("A description", patronType.Description);
            Assert.Equal(1, patronType.Ordinal);
            Assert.Equal(false, patronType.IsOrganization);
        }
    }
}
