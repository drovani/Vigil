using System.Diagnostics.Contracts;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    [ContractVerification(false)]
    public class VigilRoleManagerTests
    {
        [Fact]
        public void VigilRoleManager_Constructor_Accepts_RoleStore()
        {
            var rolestore = new InMemoryRoleStore();
            VigilRoleManager vrman = new VigilRoleManager(rolestore);

            Assert.NotNull(vrman);
        }

        [Fact(Skip = "Requires an IOwinContext implementation to obtain IdentityVigilContext.")]
        public void VigilRoleManager_Static_Create_Returns_Valid_Manager()
        {
            IdentityFactoryOptions<VigilRoleManager> options = new IdentityFactoryOptions<VigilRoleManager>();
            IOwinContext context = null;

            var roleManager = VigilRoleManager.Create(options, context);

            Assert.NotNull(roleManager);
        }
    }
}
