using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
using Xunit;

namespace Vigil.Testing.Data.Core.Patrons
{
    public class PersonStateTests
    {
        [Fact]
        public void Create_Method_Uses_Proper_Defaults()
        {
            PatronState patron = PatronState.Create(PatronTypeState.Create("PatronTypeName"), "PatronDisplayName");
            FullName fullName = new FullName(givenName: "FirstName", familyname: "LastName");
            PersonState person = PersonState.Create(patron, PersonTypeState.Create("PersonTypeName"), fullName);

            throw new NotImplementedException();
        }
        [Fact]
        public void Create_Method_Sets_Properties()
        {
            throw new NotImplementedException();
        }
    }
}
