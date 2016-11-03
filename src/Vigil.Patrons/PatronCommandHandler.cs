using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class PatronCommandHandler : ICommandHandler<CreatePatron>, ICommandHandler<UpdatePatronHeader>
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

        public void Handle(UpdatePatronHeader command)
        {
            var headerChanged = new PatronHeaderChanged
            {
                SourceId = command.Id,
                PatronId = command.PatronId,
                DisplayName = command.DisplayName,
                IsAnonymous = command.IsAnonymous,
                PatronType = command.PatronType
            };
            eventBus.Publish(headerChanged);
            repo.Save(command);
        }
    }
}
