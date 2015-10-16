using Vigil.Data.Core.Patrons.Types;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons.Types
{
    public class PersonTypeStateTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            PersonTypeState personType = PersonTypeState.Create("PersonTypeState");

            Assert.Equal("PersonTypeState", personType.TypeName);
            Assert.Null(personType.Description);
            Assert.Equal(0, personType.Ordinal);
            Assert.Equal(true, personType.AllowMultiplePerPatron);
        }

        [Fact]
        public void Create_Method_Sets_Properties()
        {
            PersonTypeState personType = PersonTypeState.Create("PersonTypeState", "A description.", 1, false);

            Assert.Equal("PersonTypeState", personType.TypeName);
            Assert.Equal("A description.", personType.Description);
            Assert.Equal(1, personType.Ordinal);
            Assert.Equal(false, personType.AllowMultiplePerPatron);
        }
    }
}
