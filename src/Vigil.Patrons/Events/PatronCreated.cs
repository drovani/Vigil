using System;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons.Events
{
    public class PatronCreated : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SourceId { get; set; }
        public Guid PatronId { get; set; }
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string PatronType { get; set; }
    }
}
