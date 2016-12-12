using System;
using Vigil.Domain.EventSourcing;

namespace Vigil.Patrons.Events
{
    public class PatronDeleted : VersionedEvent
    {
        public Guid PatronId { get; set; }

        public PatronDeleted(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }
    }
}