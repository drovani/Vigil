using System;
using Vigil.Data.Core.Identity;

namespace Vigil.Data.Core
{
    public interface IVigilContext
    {
        IVigilUser AffectedBy { get; }
        DateTime Now { get; }

        int SaveChanges();
    }
}