using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;
using Vigil.Data.Core.ValueObjects;

namespace Vigil.Data.Core.Patrons
{
    public class PersonState : Identity, ICreated, IModified, IDeleted
    {
        [Required]
        public virtual PatronState Patron { get; protected set; }
        [Required]
        public virtual PersonTypeState PersonType { get; protected set; }
        [Required]
        public FullName FullName { get; protected set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; protected set; }
        public DateAccuracy DateOfBirthAccuracy { get; protected set; }

        protected PersonState(PatronState patron, PersonTypeState personType, FullName fullName, DateTime? dateOfBirth = null)
        {
            Contract.Requires<ArgumentNullException>(patron != null);
            Contract.Requires<ArgumentNullException>(personType != null);
            Contract.Requires<ArgumentNullException>(fullName != null);

            Patron = patron;
            PersonType = personType;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
        }

        public static PersonState Create(PatronState patron, PersonTypeState personType, FullName fullName, DateTime? dateOfBirth = null)
        {
            Contract.Requires<ArgumentNullException>(patron != null);
            Contract.Requires<ArgumentNullException>(personType != null);
            Contract.Requires<ArgumentNullException>(fullName != null);

            return new PersonState(patron, personType, fullName, dateOfBirth);
        }

        #region ICreated, IModified, IDeleted Implementation
        [Required]
        public VigilUser CreatedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedOn { get; protected set; }
        public VigilUser ModifiedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? ModifiedOn { get; protected set; }
        public VigilUser DeletedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DeletedOn { get; protected set; }

        public bool MarkModified(VigilUser modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = modifiedOn;
            return true;
        }

        public bool MarkDeleted(VigilUser deletedBy, DateTime deletedOn)
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
