using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Identity.Model;

namespace Vigil.Testing.Identity.Model
{
    [TestClass]
    public class VigilUserStoreTests
    {
        [TestMethod]
        public void VigilUserStore_Default_Constructor_Sets_DisposeContext()
        {
            var ustore = new VigilUserStore();
            Assert.IsTrue(ustore.DisposeContext);
        }

        [TestMethod]
        public void VigilUserStore_Constructor_Assigns_Context()
        {
            var context = new IdentityVigilContext();
            var ustore = new VigilUserStore(context);

            Assert.AreSame(context, ustore.Context);
        }
    }
}
