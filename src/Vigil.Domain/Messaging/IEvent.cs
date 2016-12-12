using System;

namespace Vigil.Domain.Messaging
{
    public interface IEvent : IKeyIdentity
    {
        string GeneratedBy { get; }
        DateTime GeneratedOn { get; }
        /// <summary>The unique identifier of the command or event that caused this event to occur.
        /// </summary>
        Guid SourceId { get; }
    }
}
