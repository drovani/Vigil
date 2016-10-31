namespace Vigil.Domain.Messaging
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
