using System;

namespace Vigil.Sql
{
    internal class Event
    {
        public Guid Id { get; set; }
        public Guid SourceId { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string EventType { get; set; }
        public string SerializedEvent { get; set; }

        public DateTime DispatchedOn { get; set; }
        public DateTime? HandledOn { get; set; }
    }
}
