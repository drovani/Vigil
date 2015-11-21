using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patron.Model
{
    public class PatronFactory : ModelFactory<PatronCreateModel, PatronReadModel, PatronUpdateModel>
    {
        protected readonly PatronVigilContext context;

        [Import("AccountNumberGenerator", typeof(IValueGenerator<>))]
        protected IValueGenerator<string> accountNumberGenerator { get; set; }

        public PatronFactory(VigilUser affectedBy, DateTime now)
            : base()
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(now != default(DateTime));

            context = new PatronVigilContext(affectedBy, now);
        }

        public PatronReadModel CreatePatron(PatronCreateModel createPatron)
        {
            Contract.Requires<ArgumentNullException>(createPatron != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(createPatron.DisplayName));

            Contract.Assume(context.Patrons != null);
            Contract.Assume(context.PatronTypes != null);

            PatronTypeState patronType = context.PatronTypes.SingleOrDefault(pt => pt.TypeName == createPatron.PatronType);
            if (patronType == null)
            {
                return null;
            }
            PatronState newPatron = PatronState.Create(patronType: patronType,
                displayName: createPatron.DisplayName,
                accountNumber: accountNumberGenerator.GetNextValue(),
                isAnonymous: createPatron.IsAnonymous);
            context.Patrons.Add(newPatron);

            int savedChanges = ValidateAndSave(newPatron);
            if (savedChanges >= 0)
            {
                PatronRepository repo = new PatronRepository(context.AffectedBy, context.Now);
                return repo.Get(newPatron.Id);
            }
            else
            {
                return null;
            }
        }
        public PatronReadModel UpdatePatron(PatronUpdateModel updatePatron)
        {
            Contract.Requires<ArgumentNullException>(updatePatron != null);

            PatronState patron = context.Patrons.SingleOrDefault(p => p.Id == updatePatron.Id);
            if (patron != null)
            {
                patron.AccountNumber = updatePatron.AccountNumber ?? patron.AccountNumber;
                patron.DisplayName = updatePatron.DisplayName ?? patron.DisplayName;
                patron.IsAnonymous = updatePatron.IsAnonymous.HasValue ? updatePatron.IsAnonymous.Value : patron.IsAnonymous;
                if (updatePatron.PatronType != null && patron.PatronType.TypeName != updatePatron.PatronType)
                {
                    patron.PatronType = context.PatronTypes.Single(pt => pt.TypeName == updatePatron.PatronType);
                }

                if (context.Entry<PatronState>(patron).State == EntityState.Modified)
                {
                    patron.MarkModified(context.AffectedBy, context.Now);
                    int savedChanges = ValidateAndSave(patron);
                    if (savedChanges >= 0)
                    {
                        PatronRepository repo = new PatronRepository(context.AffectedBy, context.Now);
                        return repo.Get(updatePatron.Id);
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

        public bool DeletePatron(string accountNumber)
        {
            var patron = context.Patrons.SingleOrDefault(pt => pt.AccountNumber == accountNumber.Trim());
            if (patron != null)
            {
                patron.MarkDeleted(context.AffectedBy, context.Now);
                return true;
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
