using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Data.Modeling.Identity
{
    public class VigilUserManager : UserManager<VigilUser, Guid>
    {
        public VigilUserManager(IUserStore<VigilUser, Guid> store) : base(store) { }
        public static VigilUserManager Create(IdentityFactoryOptions<VigilUserManager> options, IOwinContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<VigilUserManager>() != null);

            VigilUserManager manager = new VigilUserManager(new VigilUserStore(context.Get<VigilContext>()));
            manager.UserValidator = new UserValidator<VigilUser, Guid>(manager);
            return manager;
        }
    }
}
