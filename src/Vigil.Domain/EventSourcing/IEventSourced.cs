using System;
using System.Collections.Generic;

namespace Vigil.Domain.EventSourcing
{
    public interface IEventSourced : IKeyIdentity
    {
        int Version { get; }
        IEnumerable<IVersionedEvent> Events { get; }
    }
}
