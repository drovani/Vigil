using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IModifiedContract))]
    public interface IModified : ICreated
    {
        string ModifiedBy { get; }
        DateTime? ModifiedOn { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IModified))]
        internal abstract class IModifiedContract : IModified
        {
            public abstract string CreatedBy { get; }
            public abstract DateTime CreatedOn { get; }

            public string ModifiedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(ModifiedBy == null || ModifiedBy.Trim() != string.Empty);
                Contract.Invariant(ModifiedOn == null || ModifiedOn.Value != default(DateTime));
                Contract.Invariant(ModifiedOn == null || ModifiedOn.Value.Kind == DateTimeKind.Utc);
            }
        }

    }
}
