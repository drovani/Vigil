﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Vigil.Patrons.Commands
{
    public class DeletePatronTest
    {
        [Fact]
        public void Validation_Requires_PatronId()
        {
            DeletePatron command = new DeletePatron("Delete User", TestHelper.Now)
            {
                PatronId = Guid.Empty
            };

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            Assert.Contains(validationResults, vr => vr.MemberNames.Any(mn => mn == nameof(DeletePatron.PatronId)));
        }
    }
}
