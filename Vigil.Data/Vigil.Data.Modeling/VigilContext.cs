using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public class VigilContext : IdentityDbContext, IVigilContext
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

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
