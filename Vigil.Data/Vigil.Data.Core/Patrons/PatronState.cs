using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core.Patrons
{
    public class PatronState : Identity, ICreated, IModified, IDeleted
    {
        [Required]
        public virtual PatronTypeState PatronType { get; protected set; }
        public string AccountNumber { get; protected set; }
        [Required]
        [StringLength(250)]
        public string DisplayName { get; protected set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; protected set; }

        protected PatronState() { }

        public static PatronState Create(PatronTypeState patronType, string displayName, string accountNumber = null, bool isAnonymous = false)
        {
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(displayName.Trim()));
            Contract.Requires<ArgumentNullException>(patronType != null);

            return new PatronState
            {
                DisplayName = displayName.Trim(),
                PatronType = patronType,
                AccountNumber = String.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim(),
                IsAnonymous = isAnonymous
            };
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
            Contract.Invariant(DisplayName != null);
            Contract.Invariant(!String.IsNullOrWhiteSpace(DisplayName));
            Contract.Invariant(PatronType != null);
        }
    }
}
