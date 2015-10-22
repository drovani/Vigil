using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Moq;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class VigilRoleManagerTests
    {
        [Fact]
        public void VigilRoleManager_Constructor_Accepts_RoleStore()
        {
            using (VigilRoleManager vrman = new VigilRoleManager(Mock.Of<IRoleStore<VigilRole, Guid>>()))
            {
                Assert.NotNull(vrman);
            }
        }

        [Fact]
        public void VigilRoleManager_Static_Create_Returns_Valid_Manager()
        {
            using (var ivContext = new IdentityVigilContext())
            {
                var context = new Mock<IOwinContext>();
                context.Setup(c => c.Get<IdentityVigilContext>(IdentityGlobalConstant.IdentityKeyPrefix + typeof(IdentityVigilContext).AssemblyQualifiedName))
                       .Returns(ivContext);
                IdentityFactoryOptions<VigilRoleManager> options = new IdentityFactoryOptions<VigilRoleManager>();

                using (VigilRoleManager roleManager = VigilRoleManager.Create(options, context.Object))
                {
                    Assert.NotNull(roleManager);
                }
            }
        }
    }
}
