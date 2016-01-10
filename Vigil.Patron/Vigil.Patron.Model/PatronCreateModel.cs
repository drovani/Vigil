using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Patron.Model
{
    public class PatronCreateModel : IValidatableObject
    {
        [Required]
        public string PatronType { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; set; }

        public PatronCreateModel()
        {
            IsAnonymous = false;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
        }
    }
}
