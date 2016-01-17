using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.ValueObjects;

namespace Vigil.Data.Core.Patrons
{
    public class Person : KeyIdentity, ICreated, IModified, IDeleted
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

        protected Person(Patron patron, PersonType personType, FullName fullName)
        {
            Contract.Requires<ArgumentNullException>(patron != null);
            Contract.Requires<ArgumentNullException>(personType != null);
            Contract.Requires<ArgumentNullException>(fullName != null);

            Patron = patron;
            PersonType = personType;
            FullName = fullName;
        }

        public static Person Create(Patron patron, PersonType personType, FullName fullName, DateTime? dateOfBirth = null, DateAccuracy dateOfBirthAccuracy = null)
        {
            Contract.Requires<ArgumentNullException>(patron != null);
            Contract.Requires<ArgumentNullException>(personType != null);
            Contract.Requires<ArgumentNullException>(fullName != null);
            Contract.Ensures(Contract.Result<Person>() != null);

            return new Person(patron, personType, fullName)
            {
                DateOfBirth = dateOfBirth,
                DateOfBirthAccuracy = dateOfBirthAccuracy
            };
        }

        #region ICreated, IModified, IDeleted Implementation
        [Required]
        public string CreatedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedOn { get; protected set; }
        public string ModifiedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? ModifiedOn { get; protected set; }
        public string DeletedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DeletedOn { get; protected set; }

        public bool MarkModified(string modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = modifiedOn;
            return true;
        }

        public bool MarkDeleted(string deletedBy, DateTime deletedOn)
        {
            if (DeletedBy == null && DeletedOn == null)
            {
                DeletedBy = deletedBy;
                DeletedOn = deletedOn;
                return true;
            }
            return false;
        }
        #endregion

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(FullName != null);
        }
    }
}
