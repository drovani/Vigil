using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.Patrons;

namespace Vigil.Patron.Model
{
    public class PatronRepository : IRepository<PatronReadModel>
    {
        protected readonly PatronVigilContext context;

        public PatronRepository(string affectedBy, DateTime now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(affectedBy.Trim() != string.Empty);
            Contract.Requires<ArgumentException>(now != default(DateTime));

            context = new PatronVigilContext(affectedBy, now);
        }

        public PatronReadModel Get(IKeyIdentity id)
        {
            return GetPatronReadModel(ps => ps.Id == id.Id);
        }
        public PatronReadModel GetByAccountNumber(string accountNumber)
        {
            if (accountNumber == null)
            {
                return null;
            }
            return GetPatronReadModel(ps => ps.AccountNumber == accountNumber.Trim());
        }

        public IReadOnlyCollection<PatronReadModel> GetPatronsByType(string patronType, bool includeDeleted = false)
        {
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Ensures(Contract.Result<IReadOnlyCollection<PatronReadModel>>() != null);

            Contract.Assume(context.Patrons != null);

            var query = context.Patrons
                    .Include(p => p.PatronType)
                    .Include(p => p.CreatedBy)
                    .Include(p => p.ModifiedBy)
                    .Include(p => p.DeletedBy)
                    .AsNoTracking()
                    .Where(p => p.PatronType.TypeName == patronType.Trim())
                    .Where(p => includeDeleted || p.DeletedOn == null)
                    .Select(patronState => new PatronReadModel
                    {
                        Id = patronState.Id,
                        PatronType = patronState.PatronType.TypeName,
                        AccountNumber = patronState.AccountNumber,
                        DisplayName = patronState.DisplayName,
                        IsAnonymous = patronState.IsAnonymous,
                        CreatedBy = patronState.CreatedBy,
                        CreatedOn = patronState.CreatedOn,
                        ModifiedBy = patronState.ModifiedBy,
                        ModifiedOn = patronState.ModifiedOn,
                        DeletedBy = patronState.DeletedBy,
                        DeletedOn = patronState.DeletedOn
                    });
            query.Load();
            return query.ToList();
        }

        private PatronReadModel GetPatronReadModel(Expression<Func<PatronState, bool>> predicate)
        {
            Contract.Assume(context.Patrons != null);

            PatronState patronState = context.Patrons
                    .Include(p => p.PatronType)
                    .Include(p => p.CreatedBy)
                    .Include(p => p.ModifiedBy)
                    .Include(p => p.DeletedBy)
                    .AsNoTracking()
                    .Where(predicate)
                    .SingleOrDefault();
            if (patronState == null)
            {
                return null;
            }
            else
            {
                return new PatronReadModel
                {
                    Id = patronState.Id,
                    PatronType = patronState.PatronType.TypeName,
                    AccountNumber = patronState.AccountNumber,
                    DisplayName = patronState.DisplayName,
                    IsAnonymous = patronState.IsAnonymous,
                    CreatedBy = patronState.CreatedBy,
                    CreatedOn = patronState.CreatedOn,
                    ModifiedBy = patronState.ModifiedBy,
                    ModifiedOn = patronState.ModifiedOn,
                    DeletedBy = patronState.DeletedBy,
                    DeletedOn = patronState.DeletedOn
                };
            }
        }
    }
}
