using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Vigil.Data.Core.System;

namespace Vigil.Application
{
    public interface IApplicationContext : IDisposable
    {
        IDbSet<ApplicationSetting> ApplicationSettings { get; }

        int SaveChanges();
        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
    }
}
