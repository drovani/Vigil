using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;

namespace Vigil.Data.Modeling
{
    public class VigilContext : IdentityDbContext<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IVigilContext
    {
        public VigilUser AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public DbSet<ChangeLog> ChangeLogs { get; protected set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; protected set; }

        public DbSet<PatronTypeState> PatronTypes { get; protected set; }
        public DbSet<PersonTypeState> PersonTypes { get; protected set; }
        
        public DbSet<PatronState> Patrons { get; protected set; }
        public DbSet<PersonState> Persons { get; protected set; }

        public VigilContext()
            : base("VigilContextConnection")
        {
            Database.SetInitializer<VigilContext>(new NullDatabaseInitializer<VigilContext>());
        }
        public VigilContext(VigilUser affectedBy, DateTime now)
            : this()
        {
            this.AffectedBy = affectedBy;
            this.Now = now.ToUniversalTime();
        }

        [System.Diagnostics.Contracts.ContractVerification(false)]
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
