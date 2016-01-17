using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IEntityIdContract))]
    public interface IEntityId
    {
        Guid EntityId { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IEntityId))]
        internal abstract class IEntityIdContract : IEntityId
        {
            public Guid EntityId { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(EntityId != Guid.Empty);
            }
        }
    }
}
