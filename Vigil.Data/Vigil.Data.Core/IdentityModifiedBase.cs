using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class IdentityModifiedBase : IdentityCreatedBase, IModified
    {
        public string ModifiedBy { get; protected set; }
        public DateTime? ModifiedOn { get; protected set; }

        protected IdentityModifiedBase() : base() { }

        protected IdentityModifiedBase(string createdBy, DateTime createdOn)
            : base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
        }

        public virtual bool MarkModified(string modifiedBy, DateTime modifiedOn)
        {
            IDeleted deletable = this as IDeleted;
            if (deletable == null || !deletable.IsDeleted)
            {
                ModifiedBy = modifiedBy.Trim();
                ModifiedOn = modifiedOn.ToUniversalTime();
                return true;
            }

            return false;
        }
    }
}
