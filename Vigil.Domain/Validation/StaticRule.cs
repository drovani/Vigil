using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Validation
{
    public class StaticRule<TModel> : ValidationRule<TModel>
    {
        protected readonly Predicate<TModel> rulePredicate;

        public StaticRule(Predicate<TModel> predicate, string errorToken, params string[] properties)
            : base(errorToken: errorToken,
                participatingFields: properties)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);

            rulePredicate = predicate;
        }

        public override ValidationResult Validate(TModel target)
        {
            if (rulePredicate(target))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorToken, ParticipatingFields);
            }
        }
    }
}
