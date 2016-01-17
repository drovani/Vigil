using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.Patrons.Types
{
    public class PatronType : TypeBase
    {
        [DefaultValue(true)]
        public bool IsOrganization { get; protected set; }

        protected PatronType(string patronTypeName, bool isOrganization = true)
            : base(patronTypeName)
        {
            Contract.Requires<ArgumentNullException>(patronTypeName != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(patronTypeName));

            IsOrganization = isOrganization;
        }

        public static PatronType Create(string patronTypeName, string description = null, int ordinal = 0, bool isOrganization = true)
        {
            Contract.Requires<ArgumentNullException>(patronTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(patronTypeName));
            Contract.Ensures(Contract.Result<PatronType>() != null);

            return new PatronType(patronTypeName, isOrganization)
            {
                Description = description,
                Ordinal = ordinal
            };
        }
    }
}
