using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Vigil.Patrons.Commands
{
    public class UpdatePatronCommandTest
    {
        [Fact]
        public void Validation_Requires_PatronId()
        {
            UpdatePatronCommand command = new UpdatePatronCommand();

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.PatronId)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.DisplayName)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.IsAnonymous)));
        }

        [Fact]
        public void Validation_On_DisplayName_And_PatronType_Has_Maximum_String_Length()
        {
            UpdatePatronCommand command = new UpdatePatronCommand()
            {
                PatronId = Guid.NewGuid(),
                DisplayName = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
                PatronType = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
            };

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.DisplayName)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.PatronId)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronCommand.IsAnonymous)));
        }
    }
}
