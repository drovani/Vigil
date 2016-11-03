using System;
using System.Collections.Generic;
using Vigil.Domain.Messaging;

namespace Vigil.Domain.EventSourcing
{
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type, Action<IVersionedEvent>> handlers = new Dictionary<Type, Action<IVersionedEvent>>();
        private readonly List<IVersionedEvent> events = new List<IVersionedEvent>();
        private readonly Guid id;

        public Guid Id { get { return id; } }
        public int Version { get; private set; } = -1;
        public IEnumerable<IVersionedEvent> Events { get { return events; } }

        protected EventSourced(Guid id)
        {
            this.id = id;
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent
        {
            handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }
        protected void AppendEvent(VersionedEvent e)
        {
            e.SourceId = Id;
            e.Version = Version + 1;

            handlers[e.GetType()].Invoke(e);
            Version = e.Version;
            events.Add(e);
        }
        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                handlers[e.GetType()].Invoke(e);
                Version = e.Version;
            }
        }
    }
}
