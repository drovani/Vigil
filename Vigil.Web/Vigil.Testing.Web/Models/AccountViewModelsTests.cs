using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Web.Models;

namespace Vigil.Testing.Web.Models
{
    [TestClass]
    public class AccountViewModelsTests
    {
        [TestMethod]
        public void VerifyCodeViewModel_Required_Fields_Validates_Positive()
        {
            var model = new VerifyCodeViewModel
            {
                Code = "a1b2c3d4",
                Provider = "InMemory"
            };
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);
            Assert.IsTrue(isModelStateValid);
            Assert.AreEqual(0, results.Count);
        }
    }
}
