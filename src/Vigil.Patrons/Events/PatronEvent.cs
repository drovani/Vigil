using System;
using Vigil.Domain.EventSourcing;

namespace Vigil.Patrons.Events
{
    public abstract class PatronEvent : VersionedEvent
    {
        public PatronEvent(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }

        public Guid PatronId { get; set; }
    }
}
