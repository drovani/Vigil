using Moq;
using System;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons
{
    public static class TestHelper
    {
        public static readonly DateTime Now = new DateTime(1981, 8, 26, 1, 17, 00, DateTimeKind.Utc);
        public static readonly DateTime Later = new DateTime(1982, 4, 22, 3, 13, 00, DateTimeKind.Utc);

        public static void VerifyPublish<TEvent>(this Mock<IEventBus> eventBus, Times times)
            where TEvent : IEvent
        {
            eventBus.Verify(bus => bus.Publish(It.IsAny<TEvent>()), times);
        }
    }
}
