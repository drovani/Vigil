using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using Vigil.Data.Core;

namespace Vigil.Patrons.Model
{
    [ContractClass(typeof(Contracts.IRepositoryContract<,>))]
    public interface IRepository<TEntity, TReadModel>
    {
        TReadModel Get(IKeyIdentity id);
        Expression<Func<TEntity, TReadModel>> ToReadModel { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IRepository<,>))]
        internal abstract class IRepositoryContract<TEntity, TReadModel> : IRepository<TEntity, TReadModel>
        {
            public Expression<Func<TEntity, TReadModel>> ToReadModel
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public TReadModel Get(IKeyIdentity id)
            {
                Contract.Requires<ArgumentNullException>(id != null);
                throw new NotImplementedException();
            }
        }
    }
}
