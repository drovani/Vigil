using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vigil.Domain;
using Xunit;

namespace Vigil.MessageQueue.Commands
{
    public class UpdatePatronCommandTest
    {
        [Fact]
        public void Validation_Requires_TargetPatron()
        {
            UpdatePatronCommand command = new UpdatePatronCommand();

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.TargetPatron)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.DisplayName)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.IsAnonymous)));
        }

        [Fact]
        public void Validation_On_DisplayName_Has_Maximum_String_Length()
        {
            UpdatePatronCommand command = new UpdatePatronCommand()
            {
                TargetPatron = KeyIdentity.NewIdentity(),
                DisplayName = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
                PatronType = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
            };

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.DisplayName)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.TargetPatron)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.IsAnonymous)));
        }
    }
}
