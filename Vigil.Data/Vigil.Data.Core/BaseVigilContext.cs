using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public abstract class BaseVigilContext<TContext> : DbContext where TContext : DbContext
    {
        public string AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public IDbSet<Comment> Comments { get; protected set; }

        static BaseVigilContext()
        {
            Database.SetInitializer<TContext>(null);
        }

        protected BaseVigilContext(string affectedBy, DateTime now)
            : base("VigilContextConnection")
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(affectedBy));
            Contract.Requires<AggregateException>(now != default(DateTime));

            AffectedBy = affectedBy;
            Now = now.ToUniversalTime();
            Configuration.ProxyCreationEnabled = false;
        }

        [ContractVerification(false)]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Contract.Assume(modelBuilder != null);

            modelBuilder.HasDefaultSchema("vigil");
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
