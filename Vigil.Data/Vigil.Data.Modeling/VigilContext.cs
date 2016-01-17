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
        public string AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public DbSet<ChangeLog> ChangeLogs { get; protected set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; protected set; }

        public DbSet<PatronType> PatronTypes { get; protected set; }
        public DbSet<PersonType> PersonTypes { get; protected set; }
        
        public DbSet<Patron> Patrons { get; protected set; }
        public DbSet<Person> Persons { get; protected set; }

        public VigilContext()
            : base("VigilContextConnection")
        {
            Database.SetInitializer<VigilContext>(new NullDatabaseInitializer<VigilContext>());
        }
        public VigilContext(string affectedBy, DateTime now)
            : this()
        {
            AffectedBy = affectedBy;
            Now = now.ToUniversalTime();
        }

        [ContractVerification(false)]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);

            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

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
