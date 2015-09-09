using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vigil.Web.Models;
using Xunit;

namespace Vigil.Testing.Web.Models
{
    public class AccountViewModelsTests
    {
        [Fact]
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
            Assert.True(isModelStateValid);
            Assert.Equal(0, results.Count);
        }
    }
}
