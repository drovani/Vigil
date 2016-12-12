namespace Vigil.Domain.Messaging
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent evnt) where TEvent : IEvent;
    }
}
