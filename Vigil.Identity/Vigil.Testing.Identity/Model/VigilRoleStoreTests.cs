using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    public class VigilRoleStoreTests
    {
        [Fact]
        public void VigilRoleStore_Default_Constructor_Sets_DisposeContext()
        {
            using (var rstore = new VigilRoleStore())
            {
                Assert.True(rstore.DisposeContext);
            }
        }

        [Fact]
        public void VigilRoleStore_Constructor_Assigns_Context()
        {
            using(var context = new IdentityVigilContext())
            using (var rstore = new VigilRoleStore(context))
            {
                Assert.Same(context, rstore.Context);
            }
        }
    }
}
