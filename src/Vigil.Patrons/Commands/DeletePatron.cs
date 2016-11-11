using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons.Commands
{
    public class DeletePatron : ICommand, IValidatableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PatronId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PatronId == Guid.Empty)
            {
                yield return new ValidationResult("PatronId is a required field.", new string[] { nameof(PatronId) });
            }
        }
    }
}
