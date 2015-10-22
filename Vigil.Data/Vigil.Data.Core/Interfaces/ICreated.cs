using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.ICreatedContract))]
    public interface ICreated
    {
        [Required]
        VigilUser CreatedBy { get; }
        DateTime CreatedOn { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(ICreated))]
        internal abstract class ICreatedContract : ICreated
        {
            public VigilUser CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(CreatedOn == default(DateTime) || CreatedOn.Kind == DateTimeKind.Utc);
            }
        }

    }
}
