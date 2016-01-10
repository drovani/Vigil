using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;
using Vigil.Data.Core.System;
using Vigil.Validation;

namespace Vigil.Patron.Model
{
    public class PatronFactory : ModelFactory<PatronCreateModel>
    {
        protected readonly PatronVigilContext context;

        [Import("AccountNumberGenerator", typeof(IValueGenerator<>))]
        protected IValueGenerator<string> AccountNumberGenerator { get; set; }
        [ImportMany("DomainRules", typeof(IRule<PatronCreateModel>))]
        public override ICollection<IRule<PatronCreateModel>> DomainRules { get; } = new List<IRule<PatronCreateModel>>();
        public override ICollection<ValidationResult> ValidationResults { get; } = new List<ValidationResult>();


        public PatronFactory(string affectedBy, DateTime now)
            : base()
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(affectedBy));
            Contract.Requires<ArgumentException>(now != default(DateTime));

            context = new PatronVigilContext(affectedBy, now);
        }

        public PatronReadModel CreatePatron(PatronCreateModel createPatron)
        {
            Contract.Requires<ArgumentNullException>(createPatron != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(createPatron.DisplayName));

            if (!IsDomainValid(createPatron))
            {
                return null;
            }

            PatronTypeState patronType = context.PatronTypes.SingleOrDefault(pt => pt.TypeName == createPatron.PatronType);
            if (patronType == null)
            {
                ValidationResults.Add(new ValidationResult("InvalidPatronType", new string[] { nameof(PatronCreateModel.PatronType) }));
                return null;
            }
            PatronState newPatron = PatronState.Create(patronType: patronType,
                displayName: createPatron.DisplayName,
                accountNumber: AccountNumberGenerator.GetNextValue(context.Now),
                isAnonymous: createPatron.IsAnonymous);
            //context.Patrons.Add(newPatron);

            int savedChanges = ValidateAndSave(newPatron);
            if (savedChanges >= 0)
            {
                PatronRepository repo = new PatronRepository();
                return repo.Get(newPatron);
            }
            else
            {
                return null;
            }
        }
        public PatronReadModel UpdatePatron(PatronUpdateModel updatePatron, string accountNumber)
        {
            Contract.Requires<ArgumentNullException>(updatePatron != null);

            PatronState patron = context.Patrons.SingleOrDefault(p => p.AccountNumber == accountNumber);
            if (patron != null)
            {
                patron.DisplayName = updatePatron.DisplayName ?? patron.DisplayName;
                patron.IsAnonymous = updatePatron.IsAnonymous.HasValue ? updatePatron.IsAnonymous.Value : patron.IsAnonymous;
                if (updatePatron.PatronType != null && patron.PatronType.TypeName != updatePatron.PatronType)
                {
                    patron.PatronType = context.PatronTypes.Single(pt => pt.TypeName == updatePatron.PatronType);
                }

                if (context.Entry(patron).State == EntityState.Modified)
                {
                    patron.MarkModified(context.AffectedBy, context.Now);
                    int savedChanges = ValidateAndSave(patron);
                    if (savedChanges >= 0)
                    {
                        PatronRepository repo = new PatronRepository();
                        return repo.Get(patron);
                    }
                }
            }
            return null;
        }

        [ContractVerification(false)]
        private int ValidateAndSave(object entity)
        {
            var validationResult = context.Entry(entity).GetValidationResult();
            ValidationResults.Clear();
            if (validationResult.IsValid)
            {
                return context.SaveChanges();
            }
            else
            {
                foreach (var ve in validationResult.ValidationErrors)
                {
                    ValidationResults.Add(new ValidationResult(ve.ErrorMessage, new string[1] { ve.PropertyName }));
                }
                return -1;
            }
        }

        public bool DeletePatron(string accountNumber, string reason)
        {
            var patron = context.Patrons.SingleOrDefault(pt => pt.AccountNumber == accountNumber.Trim());
            if (patron != null)
            {
                patron.MarkDeleted(context.AffectedBy, context.Now);
                context.Comments.Add(Comment.Create(patron.Id, reason, context.AffectedBy, context.Now));
                int numChanges = context.SaveChanges();
                return numChanges > 0;
            }
            else
            {
                return false;
            }
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(context != null);
            Contract.Invariant(ValidationResults != null);
        }
    }
}
