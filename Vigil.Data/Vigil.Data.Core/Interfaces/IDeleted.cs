using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IDeletedContract))]
    public interface IDeleted : ICreated
    {
        IVigilUser DeletedBy { get; }
        DateTime? DeletedOn { get; }
        bool MarkDeleted(IVigilUser deletedBy, DateTime deletedOn);
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IDeleted))]
        internal abstract class IDeletedContract : IDeleted
        {
            public IVigilUser DeletedBy { get; set; }
            public DateTime? DeletedOn { get; set; }

            public bool MarkDeleted(IVigilUser deletedBy, DateTime deletedOn)
            {
                Contract.Requires<ArgumentNullException>(deletedBy != null);
                Contract.Requires<ArgumentOutOfRangeException>(deletedOn >= CreatedOn);
                return default(bool);
            }

            public IVigilUser CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(DeletedOn == null || DeletedOn.Value.Kind == DateTimeKind.Utc);
            }
        }
    }
}
