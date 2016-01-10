using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patron.Model
{
    public class PatronReadOnlyVigilContext : ReadOnlyBaseVigilContext<PatronReadOnlyVigilContext>
    {
        public DbQuery<PatronState> Patrons
        {
            get
            {
                return Set<PatronState>().AsNoTracking();
            }
        }
        public DbQuery<PatronTypeState> PatronTypes
        {
            get
            {
                return Set<PatronTypeState>().AsNoTracking();
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PatronState>();
            modelBuilder.Entity<PatronTypeState>();
        }

    }
}
