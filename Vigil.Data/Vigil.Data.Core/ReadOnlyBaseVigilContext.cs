using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public abstract class ReadOnlyBaseVigilContext<TContext> : DbContext, IVigilContext where TContext : DbContext
    {
        public string AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public DbQuery<Comment> Comments { get; protected set; }

        protected ReadOnlyBaseVigilContext() : base("VigilContextConnection")
        {
            AffectedBy = "ReadOnly";
            Now = DateTime.UtcNow;
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);

            modelBuilder.Entity<Comment>();

            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
