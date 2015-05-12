using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [ContractClass(typeof(Contracts.IOrderedContract))]
    public interface IOrdered
    {
        int Ordinal { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IOrdered))]
        internal abstract class IOrderedContract : IOrdered
        {
            public int Ordinal { get; set; }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
            }

        }
    }
}
