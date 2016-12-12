using System;
using Vigil.Domain.EventSourcing;

namespace Vigil.Patrons.Events
{
    public class PatronCreated : VersionedEvent
    {
        public Guid PatronId { get; set; }
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string PatronType { get; set; }

        public PatronCreated(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }
    }
}
