using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using Vigil.Validation;

namespace Vigil
{
    public abstract class ModelFactory<TCreateModel>
    {
        public abstract ICollection<ValidationResult> ValidationResults { get; }
        public abstract ICollection<IRule<TCreateModel>> DomainRules { get; }

        public virtual bool IsDomainValid(TCreateModel create)
        {
            ValidationResults.Clear();
            if (create == null)
            {
                return false;
            }
            var orderedRules = DomainRules.OrderBy(pr => pr.Ordinal).ThenBy(pr => pr.RuleId);
            foreach (var rule in orderedRules)
            {
                ValidationResult result = rule.Validate(create);
                if (ValidationResult.Success != result)
                {
                    ValidationResults.Add(result);
                }
            }
            return !ValidationResults.Any();
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(ValidationResults != null);
            Contract.Invariant(DomainRules != null);
        }
    }
}
