using System;
using Vigil.Domain.Messaging;

namespace Vigil.Azure
{
    internal class Event
    {
        public Guid Id { get; set; }
        public Guid SourceId { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string EventType { get; set; }
        public IEvent PublishedEvent { get; set; }

        public DateTime DispatchedOn { get; set; }
        public DateTime? HandledOn { get; set; }

        public Event() { }
        public Event(IEvent evnt)
        {

            GeneratedBy = evnt.GeneratedBy;
            GeneratedOn = evnt.GeneratedOn;
            Id = evnt.Id;
            SourceId = evnt.SourceId;
            EventType = evnt.GetType().AssemblyQualifiedName;
            PublishedEvent = evnt;
        }
    }
}
