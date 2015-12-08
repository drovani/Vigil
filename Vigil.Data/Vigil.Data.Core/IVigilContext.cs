using System;

namespace Vigil.Data.Core
{
    public interface IVigilContext
    {
        IKeyIdentity AffectedById { get; }
        DateTime Now { get; }

        int SaveChanges();
    }
}