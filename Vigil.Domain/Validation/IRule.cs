using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Validation
{
    /// <summary>Validation Rule is used for simple state checks that only look at fields isolated to one specific object to test.
    /// </summary>
    [ContractClass(typeof(Contracts.IRuleContract<>))]
    public interface IRule<TModel>
    {
        /// <summary>Unique identifier for this particular rule.
        /// <remarks>This aids in validation caching, i18n text, and possibly other on-the-file identification needs.</remarks>
        /// </summary>
        Guid ValidationRuleId { get; }
        /// <summary>Order that the validation rules will be processed. Equal ordinals will be processed in an unsorted order.
        /// </summary>
        [DefaultValue(0)]
        int Ordinal { get; }
        /// <summary>Resource constant used to identify what the text of the validation message should be.
        /// <remarks>Parameter formatting uses the ParticipatingFields.</remarks>
        /// </summary>
        string ErrorToken { get; }
        /// <summary>The fields that are checked by the affecting rule.
        /// <remarks>This is to aid in revalidation, only checking rules that affect dirty fields.</remarks>
        /// </summary>
        string[] ParticipatingFields { get; }

        /// <summary>Checks the target against the validation rules, resetting the ValidationResults collection upon completion.
        /// </summary>
        /// <param name="target">Model that requires validation.</param>
        /// <returns>True, if the target's state passes validation; otherwise, false.</returns>
        ValidationResult Validate(TModel target);
    }
    namespace Contracts
    {
        [ContractClassFor(typeof(IRule<>))]
        internal abstract class IRuleContract<TModel> : IRule<TModel>
        {
            public Guid ValidationRuleId { get; set; }
            public int Ordinal { get; set; }
            public string ErrorToken { get; set; }
            public string[] ParticipatingFields { get; set; }
            public ValidationResult Validate(TModel target)
            {
                Contract.Requires<ArgumentNullException>(target != null);
                throw new NotImplementedException();
            }

            [ContractInvariantMethod]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(ErrorToken != null);
                Contract.Invariant(ParticipatingFields != null);
            }
        }
    }
}
