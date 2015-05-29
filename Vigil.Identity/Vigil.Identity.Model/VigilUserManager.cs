using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilUserManager : UserManager<VigilUser, Guid>
    {
        public VigilUserManager(IUserStore<VigilUser, Guid> store) : base(store) { }

        public static VigilUserManager Create(IdentityFactoryOptions<VigilUserManager> options, IOwinContext context)
        {
            var manager = new VigilUserManager(new VigilUserStore(context.Get<IdentityVigilContext>()));

            return manager;
        }
    }
}
