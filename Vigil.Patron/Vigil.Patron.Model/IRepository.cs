using Vigil.Data.Core;

namespace Vigil.Patron.Model
{
    public interface IRepository<T>
    {
        T Get(IKeyIdentity id);
    }
}
