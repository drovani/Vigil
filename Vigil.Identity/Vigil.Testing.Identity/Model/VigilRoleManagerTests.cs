using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Moq;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    [ContractVerification(false)]
    public class VigilRoleManagerTests
    {
        [Fact]
        public void VigilRoleManager_Constructor_Accepts_RoleStore()
        {
            VigilRoleManager vrman = new VigilRoleManager(Mock.Of<IRoleStore<VigilRole, Guid>>());

            Assert.NotNull(vrman);
        }

        [Fact]
        public void VigilRoleManager_Static_Create_Returns_Valid_Manager()
        {
            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Get<IdentityVigilContext>(GlobalConstant.IdentityKeyPrefix + typeof(IdentityVigilContext).AssemblyQualifiedName))
                   .Returns(new IdentityVigilContext());
            IdentityFactoryOptions<VigilRoleManager> options = new IdentityFactoryOptions<VigilRoleManager>();

            VigilRoleManager roleManager = VigilRoleManager.Create(options, context.Object);

            Assert.NotNull(roleManager);
        }
    }
}
