using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core.Patrons
{
    public class Patron : IdentityDeletedBase
    {
        [Required]
        public PatronType PatronType { get; set; }
        public string AccountNumber { get; set; }
        [Required]
        [StringLength(250)]
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; set; }

        public virtual ICollection<Comment> Comments { get; }

        protected Patron() : base() { }

        protected Patron(string createdBy, DateTime createdOn, PatronType patronType, string displayName)
            : base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(displayName));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(displayName.Trim()));

            PatronType = patronType;
            DisplayName = displayName.Trim();
        }

        public static Patron Create(string createdBy, DateTime createdOn, PatronType patronType, string displayName, string accountNumber = null, bool isAnonymous = false)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(displayName));
            Contract.Ensures(Contract.Result<Patron>() != null);

            return new Patron(createdBy, createdOn, patronType, displayName)
            {
                AccountNumber = string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim(),
                IsAnonymous = isAnonymous
            };
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(DisplayName));
            Contract.Invariant(PatronType != null);
        }
    }
}
