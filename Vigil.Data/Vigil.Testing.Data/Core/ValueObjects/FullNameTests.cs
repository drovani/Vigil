using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core.ValueObjects
{
    public class FullNameTests
    {
        [Fact]
        public void Constructor_Sets_Properties()
        {
            FullName fullname = new FullName(title: "Title", givenName: "GivenName", middleName: "MiddleName", familyname: "FamilyName", suffix: "Suffix");

            Assert.Equal("Title", fullname.Title);
            Assert.Equal("GivenName", fullname.GivenName);
            Assert.Equal("MiddleName", fullname.MiddleName);
            Assert.Equal("FamilyName", fullname.FamilyName);
            Assert.Equal("Suffix", fullname.Suffix);
        }

        [Fact]
        public void CopyConstructor_Sets_Properties()
        {
            FullName originalname = new FullName(title: "Title", givenName: "GivenName", middleName: "MiddleName", familyname: "FamilyName", suffix: "Suffix");
            FullName fullname = new FullName(originalname);

            Assert.Equal("Title", fullname.Title);
            Assert.Equal("GivenName", fullname.GivenName);
            Assert.Equal("MiddleName", fullname.MiddleName);
            Assert.Equal("FamilyName", fullname.FamilyName);
            Assert.Equal("Suffix", fullname.Suffix);
        }
    }
}
