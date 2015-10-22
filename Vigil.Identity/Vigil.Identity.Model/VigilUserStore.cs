using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilUserStore : UserStore<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public VigilUserStore()
            : this(new IdentityVigilContext())
        {
            base.DisposeContext = true;
        }
        public VigilUserStore(DbContext context) : base(context) { }
    }
}
