using System;

namespace Vigil.Data.Core.Identity
{
    public interface IVigilUser
    {
        Guid Id { get; }
        string UserName { get; }
    }
}
