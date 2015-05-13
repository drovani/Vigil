using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Vigil.Data.Core;
using Vigil.Data.Core.System;
using Vigil.Data.Modeling.Identity;

namespace Vigil.Data.Modeling
{
    public class VigilDbInitializer : DropCreateDatabaseIfModelChanges<VigilContext>
    {
        protected override void Seed(VigilContext context)
        {
            InitializeIdentity(context);
            base.Seed(context);
        }

        private static void InitializeIdentity(VigilContext context)
        {
            VigilUserManager userManager = new VigilUserManager(new VigilUserStore(context));
            VigilRoleManager roleManager = new VigilRoleManager(new VigilRoleStore(context));
            const string username = "Administrator";
            const string email = "admin@example.com";
            const string password = "admin@12345";
            const string rolename = "Admin";

            VigilRole role = roleManager.FindByNameAsync(rolename).Result;
            if (role == null)
            {
                role = new VigilRole()
                {
                    Description = "Default Adminstrator Role",
                    RoleType = VigilRoleType.Administrator,
                    Name = rolename
                };
                roleManager.CreateAsync(role).Wait();
            }

            VigilUser user = userManager.FindByNameAsync(username).Result;
            if (user == null)
            {
                user = new VigilUser()
                {
                    UserName = username,
                    Email = email
                };
                IdentityResult result = userManager.CreateAsync(user, password).Result;
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                userManager.AddToRole(user.Id, role.Name);
            }
        }
    }
}
