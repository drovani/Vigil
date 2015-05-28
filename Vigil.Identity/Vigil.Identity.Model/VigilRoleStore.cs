using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilRoleStore : RoleStore<VigilRole, Guid, VigilUserRole>, IQueryableRoleStore<VigilRole, Guid>, IRoleStore<VigilRole, Guid>
    {
        public VigilRoleStore()
            : base(new IdentityVigilContext())
        {
            base.DisposeContext = true;
        }
        public VigilRoleStore(DbContext context) : base(context) { }
    }
}
