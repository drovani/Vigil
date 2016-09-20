using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Vigil.Domain
{
    public class FactoryResult
    {
        public ICollection<ValidationResult> ValidationResults { get; protected set; }
        public IKeyIdentity AffectedEntity { get; protected set; }

        public FactoryResult(ICollection<ValidationResult> validationResults)
        {
            ValidationResults = validationResults ?? new Collection<ValidationResult>();
        }
        public FactoryResult(IKeyIdentity affectedEntity)
        {
            AffectedEntity = affectedEntity;
            ValidationResults = new Collection<ValidationResult>();
        }
    }
}
