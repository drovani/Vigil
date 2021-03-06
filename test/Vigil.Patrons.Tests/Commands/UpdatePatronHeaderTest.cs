﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Vigil.Patrons.Commands
{
    public class UpdatePatronHeaderTest
    {
        [Fact]
        public void Validation_Requires_PatronId()
        {
            UpdatePatronHeader command = new UpdatePatronHeader("Update User", TestHelper.Now);

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.PatronId)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.DisplayName)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.IsAnonymous)));
        }

        [Fact]
        public void Validation_On_DisplayName_And_PatronType_Has_Maximum_String_Length()
        {
            UpdatePatronHeader command = new UpdatePatronHeader("Update User", TestHelper.Now)
            {
                PatronId = Guid.NewGuid(),
                DisplayName = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
                PatronType = "This is a string with lots of letters appended.".PadRight(1000, 'A'),
                IsAnonymous = true
            };

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.DisplayName)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.PatronType)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.PatronId)));
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(UpdatePatronHeader.IsAnonymous)));
        }
    }
}
