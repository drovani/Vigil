using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class TypeBase : IdentityDeletedBase
    {
        [Required, StringLength(250)]
        public string TypeName { get; protected set; }
        public string Description { get; protected set; }
        public int Ordinal { get; protected set; }

        protected TypeBase() : base() { }

        protected TypeBase(string createdBy, DateTime createdOn, string typeName)
            : base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(typeName));

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
