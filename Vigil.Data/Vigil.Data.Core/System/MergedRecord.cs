using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.System
{
    public class MergedRecord : KeyIdentity, IEntityId
    {
        /// <summary>The Id of the record being moved from one PatronId to another.
        /// </summary>
        public Guid EntityId { get; protected set; }
        /// <summary>The original PatronId with which this record was associated.
        /// </summary>
        public Guid OriginalPatronId { get; protected set; }
        /// <summary>The target PatronId into which this record was merged.
        /// </summary>
        public Guid TargetPatronId { get; protected set; }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(EntityId != Guid.Empty);
            Contract.Invariant(OriginalPatronId != Guid.Empty);
            Contract.Invariant(TargetPatronId != Guid.Empty);
        }
    }
}
