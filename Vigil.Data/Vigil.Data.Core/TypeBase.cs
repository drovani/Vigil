using System;
using System.ComponentModel.DataAnnotations;
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
            Contract.Requires<ArgumentNullException>(typeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(typeName.Trim()));

            this.TypeName = typeName.Trim();
        }

        public virtual string SetTypeName(string typeName)
        {
            Contract.Requires<ArgumentNullException>(typeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(typeName.Trim()));
            Contract.Ensures(Contract.Result<string>() != null);

            TypeName = typeName.Trim();
            return TypeName;
        }

        public virtual bool MarkDeleted(VigilUser deletedBy, DateTime deletedOn)
        {
            if (DeletedBy == null && DeletedOn == null)
            {
                DeletedBy = deletedBy;
                DeletedOn = deletedOn.ToUniversalTime();
                return true;
            }
            return false;
        }


        public virtual bool MarkModified(VigilUser modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = modifiedOn.ToUniversalTime();
            return true;
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(!String.IsNullOrWhiteSpace(TypeName));
        }
    }
}
