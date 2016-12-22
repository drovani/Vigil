using System;
using System.Linq;
using Vigil.Domain.EventSourcing;
using Xunit;

namespace Vigil.Domain
{
    public class EventSourcedTest
    {
        [Fact]
        public void Update_Invokes_Handler_and_Appends_To_Events()
        {
            var test = new TestEventSourced(Guid.NewGuid());
            test.Update(new TestVersionedEvent("Test User", TestHelper.Now, Guid.NewGuid()));

            TestVersionedEvent evnt = test.Events.Single() as TestVersionedEvent;
            Assert.NotNull(evnt);
            Assert.True(test.OnTestVersionedEventCalled);
            Assert.Equal(0, evnt.Version);
            Assert.Equal(0, test.Version);
        }


        internal class TestEventSourced : EventSourced
        {
            public bool OnTestVersionedEventCalled { get; protected set; } = false;

            public TestEventSourced(Guid id) : base(id)
            {
                Handles<TestVersionedEvent>(OnTestVersionedEvent);
            }

            protected virtual void OnTestVersionedEvent(TestVersionedEvent e)
            {
                OnTestVersionedEventCalled = true;
            }
        }

        internal class TestVersionedEvent : VersionedEvent
        {
            public TestVersionedEvent(string generatedBy, DateTime generatedOn, Guid sourceId)
                : base(generatedBy, generatedOn, sourceId) { }
        }
    }
}
