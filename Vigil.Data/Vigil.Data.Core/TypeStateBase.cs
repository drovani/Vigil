using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class TypeStateBase : KeyIdentity, ICreated, IModified, IOrdered, IDeleted
    {
        [Required]
        public string CreatedBy { get; protected set; }
        public DateTime CreatedOn { get; protected set; }
        public string ModifiedBy { get; protected set; }
        public DateTime? ModifiedOn { get; protected set; }
        public string DeletedBy { get; protected set; }
        public DateTime? DeletedOn { get; protected set; }

        [Required, StringLength(250)]
        public string TypeName { get; protected set; }
        public string Description { get; protected set; }
        public int Ordinal { get; protected set; }

        protected TypeStateBase(string typeName)
            : base()
        {
            Contract.Requires<ArgumentNullException>(typeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(typeName.Trim()));

            TypeName = typeName.Trim();
        }

        public virtual string SetTypeName(string typeName)
        {
            Contract.Requires<ArgumentNullException>(typeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(typeName.Trim()));
            Contract.Ensures(Contract.Result<string>() != null);

            TypeName = typeName.Trim();
            return TypeName;
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(TypeName != null);
            Contract.Invariant(!String.IsNullOrWhiteSpace(TypeName));
        }
    }
}
