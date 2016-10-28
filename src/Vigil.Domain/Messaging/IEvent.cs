using System;

namespace Vigil.Domain.Messaging
{
    public interface IEvent
    {
        Guid Id { get; }
        Guid SourceId { get; }
    }
}
