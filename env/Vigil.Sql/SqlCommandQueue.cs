using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Vigil.Domain.Messaging;
using Vigil.Patrons;
using Vigil.Patrons.Commands;

namespace Vigil.Sql
{
    public class SqlCommandQueue : ICommandQueue
    {
        private readonly Dictionary<Type, ICommandHandler<ICommand>> _commandHandlers;

        public SqlCommandQueue(Dictionary<Type, ICommandHandler<ICommand>> commandHandlers)
        {
            _commandHandlers = commandHandlers;

        }

        public void Publish<TCommand>(TCommand command) where TCommand : ICommand
        {
            using (CommandDbContext context = new CommandDbContext())
            {
                var newCmd = new Command()
                {
                    GeneratedBy = command.GeneratedBy,
                    GeneratedOn = command.GeneratedOn,
                    Id = command.Id,
                    SerializedCommand = JsonConvert.SerializeObject(command)
                };
                context.Commands.Add(newCmd);
                context.SaveChanges();
            }

            using (CommandDbContext context = new CommandDbContext())
            {
                var cmd = context.Commands.Find(command.Id);
                cmd.HandledOn = DateTime.Now.ToUniversalTime();
                context.SaveChanges();
            }
        }
    }
}
