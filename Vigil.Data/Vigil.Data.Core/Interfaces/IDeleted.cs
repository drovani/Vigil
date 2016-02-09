using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IDeletedContract))]
    public interface IDeleted : ICreated
    {
        bool IsDeleted { get; }
        string DeletedBy { get; }
        DateTime? DeletedOn { get; }

        bool MarkDeleted(string deletedBy, DateTime deletedOn);
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
            public bool IsDeleted { get { return DeletedBy != null || DeletedOn.HasValue; } }

            public bool MarkDeleted(string deletedBy, DateTime deletedOn)
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(deletedBy));
                Contract.Requires<ArgumentException>(deletedOn != default(DateTime));
                Contract.Requires<ArgumentException>(deletedOn >= CreatedOn);

                throw new NotImplementedException("Contract Class");
            }

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
