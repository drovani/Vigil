using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IEffectiveContract))]
    public interface IEffective
    {
        /// <summary>The beginning date that this record becomes effective, inclusive.
        /// <remarks>EffectiveOn &lt;= Valid Dates</remarks>
        /// </summary>
        DateTime EffectiveOn { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IEffective))]
        internal abstract class IEffectiveContract : IEffective
        {
            public DateTime EffectiveOn { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(EffectiveOn != default(DateTime));
                Contract.Invariant(EffectiveOn.Kind == DateTimeKind.Utc);
            }

        }
    }
}
