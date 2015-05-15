using System;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilUserManager : UserManager<VigilUser, Guid>
    {
        public VigilUserManager(IUserStore<VigilUser, Guid> store) : base(store) { }
    }
}
