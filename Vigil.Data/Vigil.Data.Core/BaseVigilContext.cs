using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons;

namespace Vigil.Data.Core
{
    public class BaseVigilContext<TContext> : DbContext where TContext : DbContext
    {
        static BaseVigilContext()
        {
            Database.SetInitializer<TContext>(new NullDatabaseInitializer<TContext>());
        }

        protected BaseVigilContext()
            : base("VigilContextConnection")
        {
        }

        [ContractVerification(false)]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);

            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Remove the name "State" from the end of all of the ClrTypes.
            modelBuilder.Types().Where(t => t.Name.EndsWith("State"))
                .Configure(convention => convention.ToTable(convention.ClrType.Name.Remove(convention.ClrType.Name.Length - "State".Length)));

            base.OnModelCreating(modelBuilder);

            // Placed after calling IdentityDbContext's OnModelCreating, because it explicitly
            // sets the name of the tables for the users/roles to 'AspNet____'
            modelBuilder.Entity<VigilUser>().ToTable(typeof(VigilUser).Name);
            modelBuilder.Entity<VigilUserRole>().ToTable(typeof(VigilUserRole).Name);
            modelBuilder.Entity<VigilRole>().ToTable(typeof(VigilRole).Name);
            modelBuilder.Entity<VigilUserClaim>().ToTable(typeof(VigilUserClaim).Name);
            modelBuilder.Entity<VigilUserLogin>().ToTable(typeof(VigilUserLogin).Name);
        }
    }
}
