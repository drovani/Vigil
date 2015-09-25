using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilRoleStore : RoleStore<VigilRole, Guid, VigilUserRole>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public VigilRoleStore()
            : base(new IdentityVigilContext())
        {
            base.DisposeContext = true;
        }
        public VigilRoleStore(DbContext context) : base(context) { }
    }
}
