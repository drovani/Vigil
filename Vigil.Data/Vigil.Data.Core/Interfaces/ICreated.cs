using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.ICreatedContract))]
    public interface ICreated
    {
        [Required]
        IVigilUser CreatedBy { get; }
        DateTime CreatedOn { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(ICreated))]
        internal abstract class ICreatedContract : ICreated
        {
            public IVigilUser CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(CreatedBy != null);
                Contract.Invariant(CreatedBy.Id != Guid.Empty);
                Contract.Invariant(CreatedOn != default(DateTime));
                Contract.Invariant(CreatedOn.Kind == DateTimeKind.Utc);
            }
        }

    }
}
