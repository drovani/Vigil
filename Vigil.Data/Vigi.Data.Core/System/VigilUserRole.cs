using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vigil.Data.Core.System
{
    public class VigilUserRole : IdentityUserRole<Guid>
    {
        public Guid VigilUserRoleId { get; protected set; }

        [ForeignKey("Role")]
        public override Guid RoleId { get; set; }
        public virtual VigilRole Role { get; set; }
        [ForeignKey("User")]
        public override Guid UserId { get; set; }
        public virtual VigilUser User { get; set; }
    }
}
