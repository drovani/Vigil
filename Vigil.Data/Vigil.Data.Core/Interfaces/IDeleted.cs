using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IDeletedContract))]
    public interface IDeleted : ICreated
    {
        string DeletedBy { get; }
        DateTime? DeletedOn { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IDeleted))]
        internal abstract class IDeletedContract : IDeleted
        {
            public abstract string CreatedBy { get; }
            public abstract DateTime CreatedOn { get; }
            public string DeletedBy { get; set; }
            public DateTime? DeletedOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(DeletedBy == null || !string.IsNullOrWhiteSpace(DeletedBy));
                Contract.Invariant(DeletedOn == null || DeletedOn.Value != default(DateTime));
                Contract.Invariant(DeletedOn == null || DeletedOn.Value.Kind == DateTimeKind.Utc);
            }
        }
    }
}
