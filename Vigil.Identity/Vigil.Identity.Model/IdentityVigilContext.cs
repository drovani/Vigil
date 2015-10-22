using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class IdentityVigilContext : IdentityDbContext<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IVigilContext
    {
        public VigilUser AffectedBy { get; private set; }
        public DateTime Now { get; private set; }

        public IdentityVigilContext()
            : base("VigilContextConnection")
        {
            Now = DateTime.UtcNow;
            Database.SetInitializer<IdentityVigilContext>(new NullDatabaseInitializer<IdentityVigilContext>());
        }

        public IdentityVigilContext(VigilUser affectedBy, DateTime now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);

            this.AffectedBy = affectedBy;
            this.Now = now.ToUniversalTime();
        }

        public void SetAffectingUser(VigilUser affectedBy)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);

            this.AffectedBy = affectedBy;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static IdentityVigilContext Create()
        {
            Contract.Ensures(Contract.Result<IdentityVigilContext>() != null);

            return new IdentityVigilContext();
        }

        [System.Diagnostics.Contracts.ContractVerification(false)]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);

            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);

            // Placed after calling IdentityDbContext's OnModelCreating, because it explicitly
            // sets the name of the tables for the users/roles to 'AspNet____'
            // @TODO Find a way to grab all of this information from the VigilContext authoritative, comprehensive class
            modelBuilder.Entity<VigilUser>().ToTable(typeof(VigilUser).Name);
            modelBuilder.Entity<VigilUserRole>().ToTable(typeof(VigilUserRole).Name);
            modelBuilder.Entity<VigilRole>().ToTable(typeof(VigilRole).Name);
            modelBuilder.Entity<VigilUserClaim>().ToTable(typeof(VigilUserClaim).Name);
            modelBuilder.Entity<VigilUserLogin>().ToTable(typeof(VigilUserLogin).Name);
        }
    }
}
