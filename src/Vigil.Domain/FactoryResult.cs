using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Domain
{
    public class FactoryResult
    {
        public ICollection<ValidationResult> ValidationResults { get; protected set; }
        public IKeyIdentity AffectedEntity { get; protected set; }

        public FactoryResult(ICollection<ValidationResult> validationResults)
        {
            ValidationResults = validationResults;
        }
        public FactoryResult(IKeyIdentity affectedEntity)
        {
            AffectedEntity = affectedEntity;
        }
    }
}
