using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class PatronCommandHandler : ICommandHandler<CreatePatron>, ICommandHandler<UpdatePatronHeader>, ICommandHandler<DeletePatron>
    {
        private readonly IEventBus eventBus;

        public PatronCommandHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Handle(CreatePatron command)
        {
            var evnt = new PatronCreated(command.GeneratedBy, command.GeneratedOn, command.Id)
            {
                PatronId = Guid.NewGuid(),
                DisplayName = command.DisplayName,
                IsAnonymous = command.IsAnonymous,
                PatronType = command.PatronType
            };
            eventBus.Publish(evnt);
        }
        public void Handle(UpdatePatronHeader command)
        {
            var evnt = new PatronHeaderChanged(command.GeneratedBy, command.GeneratedOn, command.Id)
            {
                PatronId = command.PatronId,
                DisplayName = command.DisplayName,
                IsAnonymous = command.IsAnonymous,
                PatronType = command.PatronType
            };
            eventBus.Publish(evnt);
        }
        public void Handle(DeletePatron command)
        {
            var evnt = new PatronDeleted(command.GeneratedBy, command.GeneratedOn, command.Id)
            {
                PatronId = command.PatronId
            };
            eventBus.Publish(evnt);
        }
    }
}
