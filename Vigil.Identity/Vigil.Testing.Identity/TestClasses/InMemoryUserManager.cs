using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Identity.Model;

namespace Vigil.Testing.Identity.TestClasses
{
    [ContractVerification(false)]
    internal class InMemoryUserManager 
    {
        internal static VigilUserManager Create(IdentityFactoryOptions<VigilUserManager> options, IOwinContext context)
        {
            Contract.Requires<ArgumentNullException>(options != null);
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<VigilUserManager>() != null);

            var manager = new VigilUserManager(new InMemoryUserStore(context.Get<IdentityVigilContext>()));

            return manager;
        }
    }
}
