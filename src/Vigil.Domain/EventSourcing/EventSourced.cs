using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace Vigil.Domain.EventSourcing
{
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type, Action<IVersionedEvent>> handlers = new Dictionary<Type, Action<IVersionedEvent>>();
        private readonly List<IVersionedEvent> events = new List<IVersionedEvent>();

        [Key]
        public Guid Id { get; protected set; }
        public int Version { get; protected set; } = -1;
        [IgnoreDataMember]
        public IEnumerable<IVersionedEvent> Events { get { return events; } }

        [Required]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        protected EventSourced(Guid id)
        {
            Id = id;
        }

        public void Update(VersionedEvent e)
        {
            Version = ++e.Version;
            handlers[e.GetType()].Invoke(e);
            events.Add(e);
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : IVersionedEvent
        {
            handlers.Add(typeof(TEvent), evnt => handler((TEvent)evnt));
        }

        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            var orderedPastEvents = pastEvents.OrderBy(pe => pe.GeneratedOn);
            foreach (var e in orderedPastEvents)
            {
                handlers[e.GetType()].Invoke(e);
                Version = e.Version;
                events.Add(e);
            }
        }
    }
}
