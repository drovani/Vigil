using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.Patrons.Types
{
    public class PersonType : TypeBase
    {
        [DefaultValue(true)]
        public bool AllowMultiplePerPatron { get; set; }

        protected PersonType(string createdBy, DateTime createdOn, string personTypeName, bool allowMultiplePerPatron = true)
            : base(createdBy, createdOn, personTypeName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(personTypeName));

            AllowMultiplePerPatron = allowMultiplePerPatron;
        }

        public static PersonType Create(string createdBy, DateTime createdOn, string personTypeName, string description = null, int ordinal = 0, bool allowMultiplePerPatron = true)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(personTypeName));
            Contract.Ensures(Contract.Result<PersonType>() != null);

            return new PersonType(createdBy, createdOn, personTypeName, allowMultiplePerPatron)
            {
                Description = description,
                Ordinal = ordinal
            };
        }
    }
}
