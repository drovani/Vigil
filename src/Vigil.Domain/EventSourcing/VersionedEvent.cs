using System;
using Vigil.Domain.Messaging;

namespace Vigil.Domain.EventSourcing
{
    public abstract class VersionedEvent : Event, IVersionedEvent
    {
        public int Version { get; set; } = -1;

        public VersionedEvent(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }
    }
}
