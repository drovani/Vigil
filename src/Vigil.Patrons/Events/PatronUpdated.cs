using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons.Events
{
    public class PatronUpdated : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SourceId { get; set; }

        public Guid PatronId { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool? IsAnonymous { get; internal set; }
        public string PatronType { get; internal set; }
    }
}
