using System;
using System.Data.Entity;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public interface IVigilContext
    {
        VigilUser AffectedBy { get; }
        DateTime Now { get; }

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}