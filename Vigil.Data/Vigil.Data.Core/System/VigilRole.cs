using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vigil.Data.Core.System
{
    public class VigilRole : IdentityRole<Guid, VigilUserRole>, IRole<Guid>
    {
        public VigilRole()
            : base()
        {
            Id = Guid.NewGuid();
        }

        public string Description { get; set; }
        public virtual VigilRoleType RoleType { get; set; }
    }

    public enum VigilRoleType
    {
        Administrator = 0,
        PowerRole,
        DefaultRole,
        RestrictedRole
    }
}
