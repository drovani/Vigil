using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vigil.Data.Core.System
{
    public class VigilUser : IdentityUser<Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>, IUser<Guid>
    {
    }
}
