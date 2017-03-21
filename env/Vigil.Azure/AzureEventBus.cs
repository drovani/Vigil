using System;
using Vigil.Domain.Messaging;

namespace Vigil.Azure
{
    public class AzureEventBus : IEventBus
    {
        public void Publish<TEvent>(TEvent evnt) where TEvent : IEvent
        {
            throw new NotImplementedException();
        }
    }
}
