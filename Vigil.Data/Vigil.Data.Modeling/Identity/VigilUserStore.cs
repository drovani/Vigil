using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Data.Modeling.Identity
{
    public class VigilUserStore : UserStore<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IUserStore<VigilUser, Guid>, IDisposable
    {
        public VigilUserStore()
            : this(new VigilContext())
        {
            base.DisposeContext = true;
        }
        public VigilUserStore(DbContext context) : base(context) { }
    }
}
