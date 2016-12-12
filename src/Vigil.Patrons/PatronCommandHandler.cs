using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class PatronCommandHandler : ICommandHandler<CreatePatron>, ICommandHandler<UpdatePatronHeader>, ICommandHandler<DeletePatron>
    {
        private readonly IEventBus eventBus;
        private readonly ICommandRepository repo;

        public PatronCommandHandler(IEventBus eventBus, ICommandRepository repo)
        {
            this.eventBus = eventBus;
            this.repo = repo;
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
            repo.Save(command);
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
            repo.Save(command);
        }
        public void Handle(DeletePatron command)
        {
            var evnt = new PatronDeleted(command.GeneratedBy, command.GeneratedOn, command.Id)
            {
                PatronId = command.PatronId
            };
            eventBus.Publish(evnt);
            repo.Save(command);
        }
    }
}
