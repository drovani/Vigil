using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.Patrons.Types
{
    public class PatronType : TypeBase
    {
        [DefaultValue(true)]
        public bool IsOrganization { get; protected set; }

        protected PatronType() { }

        protected PatronType(string createdBy, DateTime createdOn, string patronTypeName, bool isOrganization = true)
            : base(createdBy, createdOn, patronTypeName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(patronTypeName != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(patronTypeName));

            IsOrganization = isOrganization;
        }

        public static PatronType Create(string createdBy, DateTime createdOn, string patronTypeName, string description = null, int ordinal = 0, bool isOrganization = true)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(patronTypeName));
            Contract.Ensures(Contract.Result<PatronType>() != null);

            return new PatronType(createdBy, createdOn, patronTypeName, isOrganization)
            {
                Description = description,
                Ordinal = ordinal
            };
        }
    }
}
