using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons.Commands
{
    public abstract class PatronCommand : Command, IValidatableObject
    {
        public Guid PatronId { get; set; } = Guid.NewGuid();

        public PatronCommand(string generatedBy, DateTime generatedOn) : base(generatedBy, generatedOn) { }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PatronId == Guid.Empty)
            {
                yield return new ValidationResult("PatronId is a required field.", new string[] { nameof(PatronId) });
            }
        }
    }
}
