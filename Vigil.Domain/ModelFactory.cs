using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using Vigil.Validation;

namespace Vigil
{
    public abstract class ModelFactory<TCreateModel, TReadModel, TUpdateModel>
    {
        public ICollection<ValidationResult> ValidationResults { get; protected set; }
        public ICollection<IRule<TCreateModel>> PersistenceRules { get; protected set; }
        public ICollection<IRule<TCreateModel>> DomainRules { get; protected set; }

        public ModelFactory()
        {
            ValidationResults = new Collection<ValidationResult>();
            PersistenceRules = new Collection<IRule<TCreateModel>>();
            DomainRules = new Collection<IRule<TCreateModel>>();
        }

        public bool IsPersistenceValid(TCreateModel create)
        {
            ValidationResults.Clear();
            if (create == null)
            {
                return false;
            }

            foreach (var rule in PersistenceRules)
            {
                ValidationResult result = rule.Validate(create);
                if (ValidationResult.Success != result)
                {
                    ValidationResults.Add(result);
                }
            }
            return !ValidationResults.Any();
        }
        public bool IsDomainValid(TCreateModel create)
        {
            ValidationResults.Clear();
            if (create == null)
            {
                return false;
            }
            if (IsPersistenceValid(create))
            {
                foreach (var rule in DomainRules)
                {
                    ValidationResult result = rule.Validate(create);
                    if (ValidationResult.Success != result)
                    {
                        ValidationResults.Add(result);
                    }
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
            Contract.Invariant(PersistenceRules != null);
            Contract.Invariant(DomainRules != null);
        }
    }
}
