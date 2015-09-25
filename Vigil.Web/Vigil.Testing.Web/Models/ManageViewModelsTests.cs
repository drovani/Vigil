using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vigil.Web.Models;
using Xunit;

namespace Vigil.Testing.Web.Models
{
    public class ManageViewModelsTests
    {
        [Fact]
        public void IndexViewModel_Default_Constructor()
        {
            var ivm = new IndexViewModel();

            Assert.NotNull(ivm);
        }

        [Fact]
        public void ManageLoginsViewModel_Default_Constructor()
        {
            var mlvm = new ManageLoginsViewModel();

            Assert.NotNull(mlvm);
        }
    }
}
