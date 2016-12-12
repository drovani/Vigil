﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        protected EventSourced(Guid id)
        {
            this.id = id;
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : IVersionedEvent
        {
            handlers.Add(typeof(TEvent), evnt => handler((TEvent)evnt));
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
