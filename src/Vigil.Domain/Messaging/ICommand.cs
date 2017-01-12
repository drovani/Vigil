using System;

namespace Vigil.Domain.Messaging
{
    public interface ICommand : IKeyIdentity
    {
        string GeneratedBy { get; }
        DateTime GeneratedOn { get; }
    }
}
