using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Migrations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Data.Modeling.Migrations
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal sealed class VigilDbConfiguration : DbMigrationsConfiguration<VigilContext>
    {
        public VigilDbConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        [ContractVerification(false)]
        protected override void Seed(VigilContext context)
        {
            var adminUser = new VigilUser
            {
                Id = Guid.NewGuid(),
                UserName = "vigilAdmin@example.com",
                Email = "vigiladmin@example.com",
                EmailConfirmed = true,
                PasswordHash = HashPassword(context, "vAdmin.01"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var testUser = new VigilUser
            {
                Id = Guid.NewGuid(),
                UserName = "Test User",
                Email = "test@example.com",
                EmailConfirmed = true,
                PasswordHash = HashPassword(context, "123qweasd"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            context.Set<VigilRole>().AddOrUpdate(vr => vr.Name,
                new VigilRole { Name = "System Administrators", RoleType = VigilRoleType.Administrator },
                new VigilRole { Name = "Accounting", RoleType = VigilRoleType.PowerRole },
                new VigilRole { Name = "External Vendor", RoleType = VigilRoleType.RestrictedRole },
                new VigilRole { Name = "Standard Users", RoleType = VigilRoleType.DefaultRole }
            );
            context.Set<VigilUser>().AddOrUpdate(vu => vu.UserName,
                adminUser,
                testUser
            );
            context.SaveChanges();
            using (var ustore = new UserStore<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>(context))
            using (var uman = new UserManager<VigilUser, Guid>(ustore))
            {
                uman.AddToRole(adminUser.Id, "System Administrators");
                uman.AddToRole(testUser.Id, "Standard Users");
            }

            base.Seed(context);
        }

        private static string HashPassword(VigilContext context, string password)
        {
            string hashedPassword;
            using (var ustore = new UserStore<VigilUser, VigilRole, Guid, VigilUserLogin, VigilUserRole, VigilUserClaim>(context))
            using (var uman = new UserManager<VigilUser, Guid>(ustore))
            {
                Contract.Assume(uman.PasswordHasher != null);
                hashedPassword = uman.PasswordHasher.HashPassword(password);
            }
            return hashedPassword;
        }
    }
}
