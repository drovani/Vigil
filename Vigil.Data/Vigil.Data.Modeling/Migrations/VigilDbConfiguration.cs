namespace Vigil.Data.Modeling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.Contracts;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Vigil.Data.Core.Identity;

    internal sealed class VigilDbConfiguration : DbMigrationsConfiguration<VigilContext>
    {
        public VigilDbConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        [ContractVerification(false)]
        protected override void Seed(VigilContext context)
        {
            var adminRole = new VigilRole { Name = "System Administrators", RoleType = VigilRoleType.Administrator };
            var adminUser = new VigilUser
            {
                UserName = "vigilAdmin@example.com",
                Email = "vigiladmin@example.com",
                EmailConfirmed = true,
                PasswordHash = HashPassword(context, "vAdmin.01"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            context.Set<VigilRole>().AddOrUpdate(vr => vr.Name,
                adminRole,
                new VigilRole { Name = "Accounting", RoleType = VigilRoleType.PowerRole },
                new VigilRole { Name = "External Vendor", RoleType = VigilRoleType.RestrictedRole },
                new VigilRole { Name = "Standard Users", RoleType = VigilRoleType.DefaultRole }
            );
            context.Set<VigilUser>().AddOrUpdate(vu => vu.UserName,
                adminUser
            );
        }

        private string HashPassword(VigilContext context, string password)
        {
            using (var uman = new UserManager<VigilUser, Guid>(new UserStore<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>(context)))
            {
                Contract.Assume(uman.PasswordHasher != null);
                return uman.PasswordHasher.HashPassword(password);
            }
        }
    }
}
