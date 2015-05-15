using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Data.Modeling
{
    public class VigilContext : IdentityDbContext<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IVigilContext
    {
        public VigilUser AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public VigilContext()
            : base("VigilDb")
        {
            Database.SetInitializer<VigilContext>(new DropCreateDatabaseAlways<VigilContext>());
        }
        public VigilContext(VigilUser affectedBy, DateTime now)
            : this()
        {
            this.AffectedBy = affectedBy;
            this.Now = now.ToUniversalTime();
        }

        /// <summary>Returns a System.Data.Entity.DbSet&lt;TEntity&gt; instance for access to entities
        /// of the given type in the context and the underlying store.
        /// </summary>
        /// <typeparam name="TEntity">The type entity for which a set should be returned.</typeparam>
        /// <returns>A set for the given entity type.</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        [ContractVerification(false)]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);

            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

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
