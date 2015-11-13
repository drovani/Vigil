using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patron.Model
{
    public class PatronFactory
    {
        public IReadOnlyList<KeyValuePair<string, string>> PropertyValidationErrors { get; protected set; }
        protected readonly PatronVigilContext context;

        public PatronFactory(VigilUser affectedBy, DateTime now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<AggregateException>(now != default(DateTime));

            context = new PatronVigilContext(affectedBy, now);
            PropertyValidationErrors = new List<KeyValuePair<string, string>>();
        }

        public PatronReadModel CreatePatron(PatronCreateModel createPatron, IAccountNumberGenerator accountNumberGenerator)
        {
            Contract.Requires<ArgumentNullException>(createPatron != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(createPatron.DisplayName));

            Contract.Assert(context.Patrons != null);
            Contract.Assert(context.PatronTypes != null);

            PatronTypeState patronType = context.PatronTypes.SingleOrDefault(pt => pt.TypeName == createPatron.PatronType);
            if (patronType == null)
            {
                return null;
            }
            PatronState newPatron = PatronState.Create(patronType: patronType,
                displayName: createPatron.DisplayName,
                accountNumber: accountNumberGenerator.GetAccountNumber(),
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
            if (validationResult.IsValid)
            {
                PropertyValidationErrors = new List<KeyValuePair<string, string>>();
                return context.SaveChanges();
            }
            else
            {
                PropertyValidationErrors = validationResult.ValidationErrors.Select(ve => new KeyValuePair<string, string>(ve.PropertyName, ve.ErrorMessage)).ToList();
                return -1;
            }
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(context != null);
            Contract.Invariant(PropertyValidationErrors != null);
        }
    }
}
