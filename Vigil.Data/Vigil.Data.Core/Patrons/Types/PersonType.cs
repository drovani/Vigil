using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.Patrons.Types
{
    public class PersonType : TypeBase
    {
        [DefaultValue(true)]
        public bool AllowMultiplePerPatron { get; set; }

        protected PersonType(string personTypeName, bool allowMultiplePerPatron = true):base(personTypeName)
        {
            Contract.Requires<ArgumentNullException>(personTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(personTypeName));

            AllowMultiplePerPatron = allowMultiplePerPatron;
        }

        public static PersonType Create(string personTypeName, string description = null, int ordinal = 0, bool allowMultiplePerPatron = true)
        {
            Contract.Requires<ArgumentNullException>(personTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(personTypeName));
            Contract.Ensures(Contract.Result<PersonType>() != null);

            return new PersonType(personTypeName, allowMultiplePerPatron)
            {
                Description = description,
                Ordinal = ordinal
            };
        }
    }
}
