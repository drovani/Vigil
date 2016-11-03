using System;

namespace Vigil.Domain.EventSourcing
{
    public abstract class VersionedEvent : IVersionedEvent
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public Guid SourceId { get; set; }
        public int Version { get; set; }
    }
}
