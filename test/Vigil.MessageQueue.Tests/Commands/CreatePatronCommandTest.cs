using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Vigil.MessageQueue.Commands
{
    public class CreatePatronCommandTest
    {
        [Fact]
        public void CreatePatronCommand_Defaults_IsAnonymous()
        {
            CreatePatronCommand command = new CreatePatronCommand();

            Assert.False(command.IsAnonymous, "Default Value of CreatePatronCommand.IsAnonymous changed from 'false'.");
            Assert.Null(command.DisplayName);
            Assert.Null(command.PatronType);
        }

        [Fact]
        public void Validation_Requires_DisplayName_and_PatronType()
        {
            CreatePatronCommand command = new CreatePatronCommand();

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatronCommand.DisplayName)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatronCommand.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatronCommand.IsAnonymous)));
        }

        [Fact]
        public void Validation_On_DisplayName_Has_Maximum_String_Length()
        {
            CreatePatronCommand command = new CreatePatronCommand()
            {
                DisplayName = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
                PatronType = "Invalid Type"
            };

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatronCommand.DisplayName)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatronCommand.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(CreatePatronCommand.IsAnonymous)));
        }
    }
}
