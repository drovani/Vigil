using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.ValueObjects;

namespace Vigil.Data.Core.Patrons
{
    public class Person : IdentityDeletedBase
    {
        [Required]
        public Patron Patron { get; protected set; }
        [Required]
        public PersonType PersonType { get; protected set; }
        [Required]
        public FullName FullName { get; protected set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; protected set; }
        public DateAccuracy DateOfBirthAccuracy { get; protected set; }

        protected Person() : base() { }

        protected Person(string createdBy, DateTime createdOn, Patron patron, PersonType personType, FullName fullName)
            :base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(patron != null);
            Contract.Requires<ArgumentNullException>(personType != null);
            Contract.Requires<ArgumentNullException>(fullName != null);

            Patron = patron;
            PersonType = personType;
            FullName = fullName;
        }

        public static Person Create(string createdBy, DateTime createdOn, Patron patron, PersonType personType, FullName fullName, DateTime? dateOfBirth = null, DateAccuracy dateOfBirthAccuracy = null)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(patron != null);
            Contract.Requires<ArgumentNullException>(personType != null);
            Contract.Requires<ArgumentNullException>(fullName != null);
            Contract.Ensures(Contract.Result<Person>() != null);

            return new Person(createdBy, createdOn, patron, personType, fullName)
            {
                DateOfBirth = dateOfBirth,
                DateOfBirthAccuracy = dateOfBirthAccuracy
            };
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(PersonType != null);
            Contract.Invariant(FullName != null);
        }
    }
}
