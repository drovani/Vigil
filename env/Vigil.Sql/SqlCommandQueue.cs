using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using Vigil.Domain.Messaging;

namespace Vigil.Sql
{
    public class SqlCommandQueue : ICommandQueue
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<SqlMessageDbContext> _dbFactory;

        public SqlCommandQueue(IServiceProvider serviceProvider, Func<SqlMessageDbContext> dbFactory)
        {
            _serviceProvider = serviceProvider;
            _dbFactory = dbFactory;
        }

        public void Publish<TCommand>(TCommand command) where TCommand : ICommand
        {
            using (SqlMessageDbContext context = _dbFactory())
            {
                var newCmd = new Command()
                {
                    GeneratedBy = command.GeneratedBy,
                    GeneratedOn = command.GeneratedOn,
                    Id = command.Id,
                    SerializedCommand = JsonConvert.SerializeObject(command),
                    CommandType = typeof(TCommand).AssemblyQualifiedName,
                    DispatchedOn = DateTime.UtcNow
                };
                context.Commands.Add(newCmd);
                context.SaveChanges();
            }

            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            handler.Handle(command);

            using (SqlMessageDbContext context = _dbFactory())
            {
                var cmd = context.Commands.Find(command.Id);
                cmd.HandledOn = DateTime.UtcNow;
                context.SaveChanges();
            }
        }
    }
}
