using Vigil.Domain.Messaging;

namespace Vigil.Domain.EventSourcing
{
    public interface IVersionedEvent : IEvent
    {
        /// <summary>Gets the version or order of the event in the stream.
        /// </summary>
        int Version { get; }
    }
}
