using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class TypeBase : KeyIdentity, ICreated, IModified, IOrdered, IDeleted
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

        protected TypeBase() { }

        protected TypeBase(string createdBy, DateTime createdOn, string typeName)
            : base()
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(typeName));

            CreatedBy = createdBy;
            CreatedOn = createdOn.ToUniversalTime();
            TypeName = typeName.Trim();
        }

        public virtual string SetTypeName(string typeName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(typeName));
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
        }
    }
}
