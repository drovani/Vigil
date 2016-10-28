namespace Vigil.Domain.Messaging
{
    public interface IEventBus
    {
        void Publish(IEvent @event);
    }
}
