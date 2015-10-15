using System.ComponentModel.DataAnnotations.Schema;

namespace Vigil.Data.Core
{
    [ComplexType]
    public class FullName : ValueObject<FullName>
    {
        public string GivenName { get; private set; }
        public string MiddleName { get; private set; }
        public string FamilyName { get; private set; }

        public FullName(string familyname, string givenName)
        {
            GivenName = givenName;
            FamilyName = familyname;
        }
        public FullName(string familyname, string givenName, string middleName)
            : this(familyname, givenName)
        {
            MiddleName = middleName;
        }
        public FullName(FullName fullName)
            : this(fullName.FamilyName, fullName.GivenName, fullName.MiddleName)
        { }
        internal FullName() { }
    }
}
