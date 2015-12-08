using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core.Patrons
{
    public class PatronState : KeyIdentity, ICreated, IModified, IDeleted
    {
        [Required]
        public virtual PatronTypeState PatronType { get; set; }
        public string AccountNumber { get; set; }
        [Required]
        [StringLength(250)]
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; set; }

        public virtual ICollection<Comment> Comments { get; protected set; }

        protected PatronState(PatronTypeState patronType, string displayName)
        {
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(displayName.Trim()));

            PatronType = patronType;
            DisplayName = displayName.Trim();
            Comments = new HashSet<Comment>();
        }

        public static PatronState Create(PatronTypeState patronType, string displayName, string accountNumber = null, bool isAnonymous = false)
        {
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(displayName.Trim()));
            Contract.Ensures(Contract.Result<PatronState>() != null);

            return new PatronState(patronType, displayName)
            {
                AccountNumber = String.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim(),
                IsAnonymous = isAnonymous
            };
        }

        #region ICreated, IModified, IDeleted Implementation
        [Required]
        public IVigilUser CreatedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedOn { get; protected set; }
        public IVigilUser ModifiedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? ModifiedOn { get; protected set; }
        public IVigilUser DeletedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DeletedOn { get; protected set; }

        public bool MarkModified(IKeyIdentity modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = new VigilUser() { Id = modifiedBy.Id, UserName = modifiedBy.Id.ToString() };
            ModifiedOn = modifiedOn;
            return true;
        }

        public bool MarkDeleted(IKeyIdentity deletedBy, DateTime deletedOn)
        {
            if (DeletedBy == null && DeletedOn == null)
            {
                DeletedBy = new VigilUser() { Id = deletedBy.Id, UserName = deletedBy.Id.ToString() };
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
