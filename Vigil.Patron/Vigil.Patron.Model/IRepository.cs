using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using Vigil.Data.Core;

namespace Vigil.Patron.Model
{
    [ContractClass(typeof(Contracts.IRepositoryContract<,>))]
    public interface IRepository<TState, TReadModel>
    {
        TReadModel Get(IKeyIdentity id);
        Expression<Func<TState, TReadModel>> ToReadModel { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IRepository<,>))]
        internal abstract class IRepositoryContract<TState, TReadModel> : IRepository<TState, TReadModel>
        {
            public Expression<Func<TState, TReadModel>> ToReadModel
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
