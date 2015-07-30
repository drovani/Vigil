using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class IdentityVigilContext : IdentityDbContext<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IVigilContext
    {
        public VigilUser AffectedBy { get; private set; }
        public DateTime Now { get; private set; }

        public IdentityVigilContext()
        {
            Now = DateTime.UtcNow;
        }

        public IdentityVigilContext(VigilUser affectedBy, DateTime now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);

            this.AffectedBy = affectedBy;
            this.Now = now.ToUniversalTime();
        }

        public void SetAffectingUser(VigilUser affectedBy)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);

            this.AffectedBy = affectedBy;
        }

        public static IdentityVigilContext Create()
        {
            Contract.Ensures(Contract.Result<IdentityVigilContext>() != null);

            return new IdentityVigilContext();
        }
    }
}
