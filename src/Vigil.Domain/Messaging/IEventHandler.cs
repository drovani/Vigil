namespace Vigil.Domain.Messaging
{
    public interface IEventHandler { }

    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : IEvent
    {
        void Handle(TEvent evnt);
    }
}
