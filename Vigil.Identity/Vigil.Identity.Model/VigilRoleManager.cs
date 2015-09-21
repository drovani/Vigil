﻿using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilRoleManager : RoleManager<VigilRole, Guid>
    {
        public VigilRoleManager(IRoleStore<VigilRole, Guid> roleStore)
            : base(roleStore)
        {
            Contract.Requires<ArgumentNullException>(roleStore != null);
        }

        public static VigilRoleManager Create(IdentityFactoryOptions<VigilRoleManager> options, IOwinContext context)
        {
            Contract.Requires<ArgumentNullException>(options != null);
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<VigilRoleManager>() != null);

            return new VigilRoleManager(new VigilRoleStore(context.Get<IdentityVigilContext>()));
        }
    }
}
