using System;
using System.Collections.Generic;
using Vigil.Domain.Messaging;

namespace Vigil.Domain.EventSourcing
{
    public interface IEventSourced : IKeyIdentity
    {
        string CreatedBy { get; }
        DateTime CreatedOn { get; }
        string ModifiedBy { get; }
        DateTime? ModifiedOn { get; }
        string DeletedBy { get; }
        DateTime? DeletedOn { get; }
        int Version { get; }
        IEnumerable<IVersionedEvent> Events { get; }
    }
}
