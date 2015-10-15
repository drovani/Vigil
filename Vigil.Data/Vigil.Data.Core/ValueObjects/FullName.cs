using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ComplexType]
    public class FullName : ValueObject<FullName>
    {
        [StringLength(250)]
        public string Title { get; protected set; }
        [StringLength(250)]
        public string GivenName { get; protected set; }
        [StringLength(250)]
        public string MiddleName { get; protected set; }
        [StringLength(250)]
        public string FamilyName { get; protected set; }
        [StringLength(250)]
        public string Suffix { get; protected set; }

        public FullName(string title = null, string givenName = null, string middleName = null, string familyname = null, string suffix = null)
        {
            Title = title;
            GivenName = givenName;
            MiddleName = middleName;
            FamilyName = familyname;
            Suffix = suffix;
        }
        public FullName(FullName fullName)
            : this(fullName.Title, fullName.GivenName, fullName.MiddleName, fullName.FamilyName, fullName.Suffix)
        {
            Contract.Requires<ArgumentNullException>(fullName != null);
        }
        internal FullName() { }
    }
}
