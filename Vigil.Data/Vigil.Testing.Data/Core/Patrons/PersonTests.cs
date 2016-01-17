using System;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;
using Vigil.Data.Core.ValueObjects;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class PersonTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            Patron patron = Patron.Create(PatronType.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyName: "LastName");
            PersonType personType = PersonType.Create("PersonTypeName");
            Person person = Person.Create(patron, personType, fullName);

            Assert.Same(patron, person.Patron);
            Assert.Same(personType, person.PersonType);
            Assert.Same(fullName, person.FullName);
            Assert.Null(person.DateOfBirth);
            Assert.Null(person.DateOfBirthAccuracy);
        }
        [Fact]
        public void Create_Method_Sets_Properties()
        {
            Patron patron = Patron.Create(PatronType.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyName: "LastName");
            PersonType personType = PersonType.Create("PersonTypeName");

            Person person = Person.Create(patron, personType, fullName, new DateTime(1981, 8, 25, 0, 0, 0, DateTimeKind.Utc), new DateAccuracy('U', 'A', 'E'));

            Assert.Same(patron, person.Patron);
            Assert.Same(personType, person.PersonType);
            Assert.Same(fullName, person.FullName);
            Assert.Equal(new DateTime(1981, 8, 25, 0, 0, 0, DateTimeKind.Utc), person.DateOfBirth);
            Assert.Equal("UAE", person.DateOfBirthAccuracy.ToString());
        }
    }
}
