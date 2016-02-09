using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class IdentityCreatedBase : KeyIdentity, ICreated
    {
        [Required]
        public string CreatedBy { get; protected set; }
        public DateTime CreatedOn { get; protected set; }

        protected IdentityCreatedBase() : base() { }

        protected IdentityCreatedBase(string createdBy, DateTime createdOn)
            : base()
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));

            CreatedBy = createdBy.Trim();
            CreatedOn = createdOn.ToUniversalTime();
        }
    }
}
