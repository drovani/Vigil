using Newtonsoft.Json;
using System;
using Vigil.Domain.Messaging;

namespace Vigil.Sql
{
    public class SqlCommandQueue : ICommandQueue
    {
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

            // TODO - figure out how to call the appropriate CommandHandler
            //throw new NotImplementedException();

            //using (CommandDbContext context = new CommandDbContext())
            //{
            //    var cmd = context.Commands.Find(command.Id);
            //    cmd.HandledOn = DateTime.Now.ToUniversalTime();
            //    context.SaveChanges();
            //}
        }
    }
}
