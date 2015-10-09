namespace Vigil.Data.Modeling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Vigil.Data.Core.System;
    using Vigil.Identity.Model;

    internal sealed class VigilDbConfiguration : DbMigrationsConfiguration<VigilContext>
    {
        public VigilDbConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

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
            using (VigilUserManager uman = new VigilUserManager(new VigilUserStore(context)))
            {
                return uman.PasswordHasher.HashPassword(password);
            }
        }
    }
}
