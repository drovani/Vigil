using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Identity.Model;

namespace Vigil.Testing.Web.TestClasses
{
    internal class InMemoryUserManager 
    {
        internal static VigilUserManager Create(IdentityFactoryOptions<VigilUserManager> options, IOwinContext context)
        {
            Contract.Ensures(Contract.Result<VigilUserManager>() != null);

            var manager = new VigilUserManager(new InMemoryUserStore(context.Get<IdentityVigilContext>()));

            return manager;
        }
    }
}
