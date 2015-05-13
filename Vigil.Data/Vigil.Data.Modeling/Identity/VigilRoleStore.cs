using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Data.Modeling.Identity
{
    public class VigilRoleStore : RoleStore<VigilRole, Guid, VigilUserRole>, IQueryableRoleStore<VigilRole, Guid>, IRoleStore<VigilRole, Guid>, IDisposable
    {
        public VigilRoleStore()
            : base(new VigilContext())
        {
            base.DisposeContext = true;
        }
        public VigilRoleStore(DbContext context) : base(context) { }
    }
}
