using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IEffectiveRangeContract))]
    public interface IEffectiveRange : IEffective
    {
        /// <summary>The end date that this record is effective between, non-inclusive.
        /// <remarks>EffectiveOn &lt;= Valid Dates &lt; EffectiveUntil</remarks>
        /// </summary>
        DateTime EffectiveUntil { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IEffectiveRange))]
        internal abstract class IEffectiveRangeContract : IEffectiveRange
        {
            public DateTime EffectiveUntil { get; set; }
            public DateTime EffectiveOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(EffectiveUntil != default(DateTime));
                Contract.Invariant(EffectiveOn <= EffectiveUntil);
                Contract.Invariant(EffectiveUntil.Kind == DateTimeKind.Utc);
            }
        }
    }
}
