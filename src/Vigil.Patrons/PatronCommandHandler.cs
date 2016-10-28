using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class PatronCommandHandler : ICommandHandler<CreatePatronCommand>
    {
        private readonly IEventBus eventBus;
        private readonly ICommandRepository repo;

        public PatronCommandHandler(IEventBus eventBus, ICommandRepository repo)
        {
            this.eventBus = eventBus;
            this.repo = repo;
        }

        public void Handle(CreatePatronCommand command)
        {
            var patronCreated = new PatronCreated
            {
                SourceId = command.Id,
                PatronId = Guid.NewGuid(),
                DisplayName = command.DisplayName,
                IsAnonymous = command.IsAnonymous,
                PatronType = command.PatronType
            };
            eventBus.Publish(patronCreated);
            repo.Save(command);
        }

        public void Handle(UpdatePatronCommand command)
        {
            var patronUpdated = new PatronUpdated
            {
                SourceId = command.Id,
                PatronId = command.PatronId,
                DisplayName = command.DisplayName,
                IsAnonymous = command.IsAnonymous,
                PatronType = command.PatronType
            };
            eventBus.Publish(patronUpdated);
            repo.Save(command);
        }
    }
}
