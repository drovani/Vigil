using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Vigil.Patrons.Commands
{
    public class CreatePatronTest
    {
        [Fact]
        public void CreatePatronCommand_Defaults_IsAnonymous()
        {
            CreatePatron command = new CreatePatron();

            Assert.False(command.IsAnonymous, "Default Value of CreatePatronCommand.IsAnonymous changed from 'false'.");
            Assert.Null(command.DisplayName);
            Assert.Null(command.PatronType);
        }

        [Fact]
        public void Validation_Requires_DisplayName_and_PatronType()
        {
            CreatePatron command = new CreatePatron();

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatron.DisplayName)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatron.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatron.IsAnonymous)));
        }

        [Fact]
        public void Validation_On_DisplayName_Has_Maximum_String_Length()
        {
            CreatePatron command = new CreatePatron()
            {
                DisplayName = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
                PatronType = "Invalid Type"
            };

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatron.DisplayName)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatron.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatron.IsAnonymous)));
        }
    }
}
