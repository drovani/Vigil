using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.Patrons.Types
{
    public class PersonTypeState : TypeStateBase
    {
        [DefaultValue(true)]
        public bool AllowMultiplePerPatron { get; set; }

        protected PersonTypeState(string personTypeName, bool allowMultiplePerPatron = true):base(personTypeName)
        {
            Contract.Requires<ArgumentNullException>(personTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(personTypeName));

            AllowMultiplePerPatron = allowMultiplePerPatron;
        }

        public static PersonTypeState Create(string personTypeName, string description = null, int ordinal = 0, bool allowMultiplePerPatron = true)
        {
            Contract.Requires<ArgumentNullException>(personTypeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(personTypeName));
            Contract.Ensures(Contract.Result<PersonTypeState>() != null);

            return new PersonTypeState(personTypeName, allowMultiplePerPatron)
            {
                Description = description,
                Ordinal = ordinal
            };
        }
    }
}
