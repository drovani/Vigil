using System;

namespace Vigil.Domain.Messaging
{
    public interface IEvent : IKeyIdentity
    {
        Guid SourceId { get; }
    }
}
