using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Identity.Model;

namespace Vigil.Testing.Identity.Model
{
    [TestClass]
    public class VigilRoleStoreTests
    {
        [TestMethod]
        public void VigilRoleStore_Default_Constructor_Sets_DisposeContext()
        {
            var rstore = new VigilRoleStore();
            Assert.IsTrue(rstore.DisposeContext);
        }

        [TestMethod]
        public void VigilRoleStore_Constructor_Assigns_Context()
        {
            var context = new IdentityVigilContext();
            var rstore = new VigilRoleStore(context);

            Assert.AreSame(context, rstore.Context);
        }
    }
}
