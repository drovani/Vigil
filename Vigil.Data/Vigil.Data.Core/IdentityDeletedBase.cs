using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class IdentityDeletedBase : IdentityModifiedBase, IDeleted
    {
        public string DeletedBy { get; protected set; }
        public DateTime? DeletedOn { get; protected set; }
        public bool IsDeleted { get { return DeletedBy != null || DeletedOn.HasValue; } }

        protected IdentityDeletedBase() : base() { }

        protected IdentityDeletedBase(string createdBy, DateTime createdOn)
            : base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
        }

        public bool MarkDeleted(string deletedBy, DateTime deletedOn)
        {
            if (!IsDeleted)
            {
                DeletedBy = deletedBy.Trim();
                DeletedOn = deletedOn.ToUniversalTime();
                return true;
            }

            return false;
        }
    }
}
