using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patron.Model
{
    public class PatronVigilContext : BaseVigilContext<PatronVigilContext>, IVigilContext
    {
        public IDbSet<PatronState> Patrons { get; protected set; }
        public IDbSet<PatronTypeState> PatronTypes { get; protected set; }

        public PatronVigilContext(IKeyIdentity affectedById, DateTime now)
            : base(affectedById, now)
        {
            Contract.Requires<ArgumentNullException>(affectedById != null);
            Contract.Requires<ArgumentException>(affectedById.Id != Guid.Empty);
            Contract.Requires<ArgumentException>(now != default(DateTime));
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
        }
    }
}
