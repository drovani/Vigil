using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons.Commands
{
    public class UpdatePatronCommand : ICommand, IValidatableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PatronId { get; set; }

        [StringLength(250)]
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
        [StringLength(250)]
        public string PatronType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PatronId == Guid.Empty)
            {
                yield return new ValidationResult("PatronId is a required field.", new string[] { nameof(PatronId) });
            }
        }
    }
}
