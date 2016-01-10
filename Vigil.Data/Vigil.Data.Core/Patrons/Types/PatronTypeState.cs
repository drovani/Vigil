using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.Patrons.Types
{
    public class PatronTypeState : TypeStateBase
    {
        [DefaultValue(true)]
        public bool IsOrganization { get; protected set; }

        protected PatronTypeState(string patronTypeName, bool isOrganization = true)
            : base(patronTypeName)
        {
            Contract.Requires<ArgumentNullException>(patronTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(patronTypeName));

            IsOrganization = isOrganization;
        }

        public static PatronTypeState Create(string patronTypeName, string description = null, int ordinal = 0, bool isOrganization = true)
        {
            Contract.Requires<ArgumentNullException>(patronTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(patronTypeName));
            Contract.Ensures(Contract.Result<PatronTypeState>() != null);

            return new PatronTypeState(patronTypeName, isOrganization)
            {
                Description = description,
                Ordinal = ordinal
            };
        }
    }
}
