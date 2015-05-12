using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IModifiedContract))]
    public interface IModified : ICreated
    {
        VigilUser ModifiedBy { get; }
        DateTime? ModifiedOn { get; }
        bool MarkModified(VigilUser modifiedBy, DateTime modifiedOn);
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IModified))]
        internal abstract class IModifiedContract : IModified
        {
            public VigilUser ModifiedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }

            public bool MarkModified(VigilUser modifiedBy, DateTime modifiedOn)
            {
                Contract.Requires<ArgumentNullException>(modifiedBy != null);
                Contract.Requires<ArgumentOutOfRangeException>(modifiedOn >= CreatedOn);
                return default(bool);
            }

            public VigilUser CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(ModifiedOn == null || ModifiedOn.Value != default(DateTime));
                Contract.Invariant(ModifiedOn == null || ModifiedOn.Value.Kind == DateTimeKind.Utc);
            }
        }

    }
}
