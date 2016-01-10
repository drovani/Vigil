using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core.Patrons
{
    public class PatronState : KeyIdentity, ICreated, IModified, IDeleted
    {
        [Required]
        public PatronTypeState PatronType { get; set; }
        public string AccountNumber { get; set; }
        [Required]
        [StringLength(250)]
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; set; }

        public virtual ICollection<Comment> Comments { get; }

        protected PatronState(PatronTypeState patronType, string displayName)
        {
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(displayName));

            PatronType = patronType;
            DisplayName = displayName.Trim();
        }

        public static PatronState Create(PatronTypeState patronType, string displayName, string accountNumber = null, bool isAnonymous = false)
        {
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(displayName));
            Contract.Ensures(Contract.Result<PatronState>() != null);

            return new PatronState(patronType, displayName)
            {
                AccountNumber = string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim(),
                IsAnonymous = isAnonymous
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
        #endregion

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(DisplayName != null);
            Contract.Invariant(!string.IsNullOrWhiteSpace(DisplayName));
            Contract.Invariant(PatronType != null);
        }
    }
}
