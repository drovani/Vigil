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
        public DbSet<PatronState> Patrons { get; protected set; }
        public DbSet<PatronTypeState> PatronTypes { get; protected set; }

        public PatronVigilContext(string affectedBy, DateTime now)
            : base(affectedBy, now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(affectedBy));
            Contract.Requires<ArgumentException>(now != default(DateTime));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatronState>();
            modelBuilder.Entity<PatronTypeState>();

            base.OnModelCreating(modelBuilder);
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
