using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public abstract class BaseVigilContext<TContext> : DbContext where TContext : DbContext
    {
        public IVigilUser AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public IDbSet<Comment> Comments { get; protected set; }

        protected BaseVigilContext(IVigilUser affectedBy, DateTime now)
            : base("VigilContextConnection")
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<AggregateException>(now != default(DateTime));

            AffectedBy = affectedBy;
            Now = now.ToUniversalTime();

            Database.SetInitializer<TContext>(new NullDatabaseInitializer<TContext>());
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
