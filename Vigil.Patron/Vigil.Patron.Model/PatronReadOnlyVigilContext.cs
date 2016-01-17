using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patrons.Model
{
    public class PatronReadOnlyVigilContext : ReadOnlyBaseVigilContext<PatronReadOnlyVigilContext>
    {
        public DbQuery<Patron> Patrons
        {
            get
            {
                return base.Set<Patron>().AsNoTracking();
            }
        }
        public DbQuery<PatronType> PatronTypes
        {
            get
            {
                return Set<PatronType>().AsNoTracking();
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Data.Core.Patrons.Patron>();
            modelBuilder.Entity<PatronType>();
        }

    }
}
