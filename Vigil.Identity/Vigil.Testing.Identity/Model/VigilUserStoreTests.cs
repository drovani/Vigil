using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    public class VigilUserStoreTests
    {
        [Fact]
        public void VigilUserStore_Default_Constructor_Sets_DisposeContext()
        {
            using (var ustore = new VigilUserStore())
            {
                Assert.True(ustore.DisposeContext);
            }
        }

        [Fact]
        public void VigilUserStore_Constructor_Assigns_Context()
        {
            using(var context = new IdentityVigilContext())
            using (var ustore = new VigilUserStore(context))
            {
                Assert.Same(context, ustore.Context);
            }
        }
    }
}
