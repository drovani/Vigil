using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vigil.Data.Core.System
{
    public class VigilUserLogin : IdentityUserLogin<Guid>
    {
        public Guid VigilUserLoginId { get; protected set; }

        public VigilUserLogin() : base() { }
    }
}
