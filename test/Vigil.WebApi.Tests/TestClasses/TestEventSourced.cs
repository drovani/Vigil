using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vigil.Domain.EventSourcing;

namespace Vigil.WebApi
{
    public class TestEventSourced : EventSourced
    {
        [NotMapped]
        public bool OnTestVersionedEventCalled { get; protected set; } = false;

        protected TestEventSourced() : base(Guid.Empty) { }

        public TestEventSourced(Guid id) : base(id)
        {
            Handles<TestVersionedEvent>(OnTestVersionedEvent);
        }

        protected virtual void OnTestVersionedEvent(TestVersionedEvent e)
        {
            OnTestVersionedEventCalled = true;
        }
    }

    public class TestVersionedEvent : VersionedEvent
    {
        public TestVersionedEvent(string generatedBy, DateTime generatedOn, Guid sourceId)
            : base(generatedBy, generatedOn, sourceId) { }
    }
}
