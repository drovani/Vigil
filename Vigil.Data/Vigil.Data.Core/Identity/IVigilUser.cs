using System;

namespace Vigil.Data.Core.Identity
{
    public interface IVigilUser : IKeyIdentity
    {
        string UserName { get; }
    }
}
