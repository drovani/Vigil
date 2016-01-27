using System;
using Vigil.Data.Core.Patrons.Types;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons.Types
{
    public class PatronTypeTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            PatronType patronType = PatronType.Create("TestUser", DateTime.UtcNow, "PatronTypeName");

            Assert.Equal("PatronTypeName", patronType.TypeName);
            Assert.Null(patronType.Description);
            Assert.Equal(0, patronType.Ordinal);
            Assert.Equal(true, patronType.IsOrganization);
        }

        [Fact]
        public void Create_Method_Sets_Properties()
        {
            PatronType patronType = PatronType.Create("TestUser", DateTime.UtcNow, "PatronTypeName", "A description.", 1, false);

            Assert.Equal("PatronTypeName", patronType.TypeName);
            Assert.Equal("A description.", patronType.Description);
            Assert.Equal(1, patronType.Ordinal);
            Assert.Equal(false, patronType.IsOrganization);
        }
    }
}
