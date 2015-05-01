using System;

namespace Vigil.Data.Core
{
    public interface IEffectiveRange : IEffective
    {
        /// <summary>The end date that this record is effective between, non-inclusive.
        /// <remarks>EffectiveOn &lt;= Valid Dates &lt; EffectiveUntil</remarks>
        /// </summary>
        DateTime EffectiveUntil { get; }
    }
}
