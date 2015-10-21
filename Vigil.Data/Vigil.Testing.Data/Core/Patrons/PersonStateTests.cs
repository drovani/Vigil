using System;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;
using Vigil.Data.Core.ValueObjects;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class PersonStateTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            PatronState patron = PatronState.Create(PatronTypeState.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyname: "LastName");
            PersonTypeState personType = PersonTypeState.Create("PersonTypeName");
            PersonState person = PersonState.Create(patron, personType, fullName);

            Assert.Same(patron, person.Patron);
            Assert.Same(personType, person.PersonType);
            Assert.Same(fullName, person.FullName);
            Assert.Null(person.DateOfBirth);
            Assert.Null(person.DateOfBirthAccuracy);
        }
        [Fact]
        public void Create_Method_Sets_Properties()
        {
            PatronState patron = PatronState.Create(PatronTypeState.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyname: "LastName");
            PersonTypeState personType = PersonTypeState.Create("PersonTypeName");

            PersonState person = PersonState.Create(patron, personType, fullName, new DateTime(1981, 8, 25, 0, 0, 0, DateTimeKind.Utc), new DateAccuracy('U', 'A', 'E'));

            Assert.Same(patron, person.Patron);
            Assert.Same(personType, person.PersonType);
            Assert.Same(fullName, person.FullName);
            Assert.Equal(new DateTime(1981, 8, 25, 0, 0, 0, DateTimeKind.Utc), person.DateOfBirth);
            Assert.Equal("UAE", person.DateOfBirthAccuracy.ToString());
        }

        [Fact]
        public void MarkDeleted_Sets_Delete_Properties()
        {
            PatronState patron = PatronState.Create(PatronTypeState.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyname: "LastName");
            PersonTypeState personType = PersonTypeState.Create("PersonTypeName");
            PersonState person = PersonState.Create(patron, personType, fullName);

            DateTime now = DateTime.UtcNow;
            VigilUser newUser = new VigilUser();
            person.MarkDeleted(newUser, now);

            Assert.Same(newUser, person.DeletedBy);
            Assert.Equal(now, person.DeletedOn);
        }

        [Fact]
        public void MarkModified_Sets_Delete_Properties()
        {
            PatronState patron = PatronState.Create(PatronTypeState.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyname: "LastName");
            PersonTypeState personType = PersonTypeState.Create("PersonTypeName");
            PersonState person = PersonState.Create(patron, personType, fullName);

            DateTime now = DateTime.UtcNow;
            VigilUser newUser = new VigilUser();
            person.MarkModified(newUser, now);

            Assert.Same(newUser, person.ModifiedBy);
            Assert.Equal(now, person.ModifiedOn);
        }
    }
}
