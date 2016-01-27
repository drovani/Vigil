using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Patrons.Model.Types;

namespace Vigil.Patrons.Model
{
    public class PatronRepository : IRepository<Patron, PatronReadModel>, IDisposable
    {
        private PatronReadOnlyVigilContext context;
        public Expression<Func<Patron, PatronReadModel>> ToReadModel
        {
            get
            {
                return patronState => new PatronReadModel
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

        public PatronRepository()
        {
            context = new PatronReadOnlyVigilContext();
        }

        public IEnumerable<PatronTypeModel> GetPatronTypes()
        {
            Contract.Ensures(Contract.Result<IEnumerable<PatronTypeModel>>() != null);

            var query = from pt in context.PatronTypes
                        where pt.DeletedOn == null
                        orderby pt.Ordinal, pt.TypeName
                        select new
                        {
                            pt.TypeName,
                            pt.Description,
                            pt.IsOrganization
                        };
            return query.ToList().Select(pt => new PatronTypeModel(pt.TypeName, pt.Description, pt.IsOrganization));
        }

        public PatronReadModel Find(IKeyIdentity id)
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

        public IReadOnlyCollection<PatronReadModel> GetPatronsBySearch(PatronSearchModel search, out int totalRecords)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Ensures(Contract.Result<IReadOnlyCollection<PatronReadModel>>() != null);

            Contract.Assume(context.Patrons != null);

            var predicates = GetSearchPredicates(search);

            var query = context.Patrons.Include(p => p.PatronType);
            predicates.ForEach(pred => query = query.Where(pred));

            totalRecords = query.Count();

            var result = query.OrderBy(q => q.AccountNumber).Take(search.ResultLimit)
                .Select(ToReadModel);
            return result.ToList();
        }

        private PatronReadModel GetPatronReadModel(Expression<Func<Patron, bool>> predicate)
        {
            Contract.Assume(context.Patrons != null);

            Patron patronState = context.Patrons
                    .Include(p => p.PatronType)
                    .Where(predicate)
                    .SingleOrDefault();
            if (patronState == null)
            {
                return null;
            }
            else
            {
                return ToReadModel.Compile().Invoke(patronState);
            }
        }
        private static List<Expression<Func<Patron, bool>>> GetSearchPredicates(PatronSearchModel search)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Ensures(Contract.Result<IList<Expression<Func<Patron, bool>>>>() != null);

            List<Expression<Func<Patron, bool>>> predicates = new List<Expression<Func<Patron, bool>>>();

            if (!string.IsNullOrWhiteSpace(search.AccountNumber))
            {
                predicates.Add(q => q.AccountNumber == search.AccountNumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(search.CreatedBy))
            {
                predicates.Add(q => q.CreatedBy.StartsWith(search.CreatedBy));
            }
            if (search.CreatedOnAfter.HasValue)
            {
                predicates.Add(q => q.CreatedOn >= search.CreatedOnAfter.Value);
            }
            if (search.CreatedOnBefore.HasValue)
            {
                predicates.Add(q => q.CreatedOn <= search.CreatedOnBefore.Value);
            }
            if (!string.IsNullOrWhiteSpace(search.DisplayName))
            {
                predicates.Add(q => q.DisplayName == search.DisplayName);
            }
            if (search.HasBeenDeleted.HasValue)
            {
                predicates.Add(q => q.DeletedOn.HasValue == search.HasBeenDeleted.Value);
            }
            if (search.IsAnonymous.HasValue)
            {
                predicates.Add(q => q.IsAnonymous == search.IsAnonymous.Value);
            }
            if (!string.IsNullOrWhiteSpace(search.PatronType))
            {
                predicates.Add(q => q.PatronType.TypeName == search.PatronType);
            }

            return predicates;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
