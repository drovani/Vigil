using System;

namespace Vigil.Data.Core
{
    public interface IVigilContext
    {
        string AffectedBy { get; }
        DateTime Now { get; }

        int SaveChanges();
    }
}