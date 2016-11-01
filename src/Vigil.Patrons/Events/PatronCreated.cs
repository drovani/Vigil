using System;
using Vigil.Domain.EventSourcing;

namespace Vigil.Patrons.Events
{
    public class PatronCreated : IVersionedEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Version { get; private set; } = 0;
        public Guid SourceId { get; set; }
        public Guid PatronId { get; set; }
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string PatronType { get; set; }
    }
}
