using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public abstract class ReadOnlyBaseVigilContext<TContext> : BaseVigilContext<TContext> where TContext : DbContext
    {
        protected ReadOnlyBaseVigilContext() : base("ReadOnly", DateTime.UtcNow) { }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);
            modelBuilder.Entity<Comment>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
