using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    public class VigilRoleStoreTests
    {
        [Fact]
        public void VigilRoleStore_Default_Constructor_Sets_DisposeContext()
        {
            var rstore = new VigilRoleStore();
            Assert.True(rstore.DisposeContext);
        }

        [Fact]
        public void VigilRoleStore_Constructor_Assigns_Context()
        {
            var context = new IdentityVigilContext();
            var rstore = new VigilRoleStore(context);

            Assert.Same(context, rstore.Context);
        }
    }
}
