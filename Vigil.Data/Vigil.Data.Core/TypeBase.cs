using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public abstract class TypeBase : Identity, ICreated, IModified, IOrdered, IDeleted
    {
        [Required]
        public VigilUser CreatedBy { get; protected set; }
        public DateTime CreatedOn { get; protected set; }
        public VigilUser ModifiedBy { get; protected set; }
        public DateTime? ModifiedOn { get; protected set; }
        public VigilUser DeletedBy { get; protected set; }
        public DateTime? DeletedOn { get; protected set; }

        [Required, StringLength(250)]
        public string TypeName { get; protected set; }
        public string Description { get; protected set; }
        public int Ordinal { get; protected set; }

        protected TypeBase(string typeName)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(typeName));

            this.TypeName = typeName;
        }

        public bool MarkDeleted(VigilUser deletedBy, DateTime deletedOn)
        {
            if (DeletedBy == null && DeletedOn == null)
            {
                DeletedBy = deletedBy;
                DeletedOn = deletedOn.ToUniversalTime();
                return true;
            }
            return false;
        }


        public bool MarkModified(VigilUser modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = modifiedOn.ToUniversalTime();
            return true;
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(!String.IsNullOrWhiteSpace(TypeName));
        }
    }
}
