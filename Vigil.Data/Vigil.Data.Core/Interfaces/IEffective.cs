using System;

namespace Vigil.Data.Core
{
    public interface IEffective
    {
        /// <summary>The beginning date that this record becomes effective, inclusive.
        /// <remarks>EffectiveOn &lt;= Valid Dates</remarks>
        /// </summary>
        DateTime EffectiveOn { get; }
    }
}
