using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class IdentityVigilContext : IdentityDbContext<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IVigilContext
    {
        public VigilUser AffectedBy { get; set; }
        public DateTime Now { get; set; }

        /// <summary>Returns a System.Data.Entity.DbSet&lt;TEntity&gt; instance for access to entities
        /// of the given type in the context and the underlying store.
        /// </summary>
        /// <typeparam name="TEntity">The type entity for which a set should be returned.</typeparam>
        /// <returns>A set for the given entity type.</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
