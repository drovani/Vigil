using System;
using Vigil.Domain.Messaging;

namespace Vigil.Azure
{
    internal class Command
    {
        public Guid Id { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string CommandType { get; set; }
        public ICommand PublishedCommand { get; set; }

        public DateTime DispatchedOn { get; set; }
        public DateTime? HandledOn { get; set; }

        public Command() { }
        public Command(ICommand command)
        {
            GeneratedBy = command.GeneratedBy;
            GeneratedOn = command.GeneratedOn;
            Id = command.Id;
            PublishedCommand = command;
            CommandType = command.GetType().AssemblyQualifiedName;
        }
    }
}