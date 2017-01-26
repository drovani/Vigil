using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using Vigil.Domain.Messaging;

namespace Vigil.Sql
{
    public class SqlEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<SqlMessageDbContext> _dbFactory;


        public SqlEventBus(IServiceProvider serviceProvider, Func<SqlMessageDbContext> dbFactory)
        {
            _serviceProvider = serviceProvider;
            _dbFactory = dbFactory;
        }

        public void Publish<TEvent>(TEvent evnt) where TEvent : IEvent
        {
            using (SqlMessageDbContext context = _dbFactory())
            {
                var newEvnt = new Event()
                {
                    GeneratedBy = evnt.GeneratedBy,
                    GeneratedOn = evnt.GeneratedOn,
                    Id = evnt.Id,
                    SourceId = evnt.SourceId,
                    EventType = typeof(TEvent).AssemblyQualifiedName,
                    SerializedEvent = JsonConvert.SerializeObject(evnt),
                    DispatchedOn = DateTime.UtcNow
                };
                context.Events.Add(newEvnt);
                context.SaveChanges();
            }

            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach(IEventHandler<TEvent> handler in handlers)
            {
                handler.Handle(evnt);
            }

            using (SqlMessageDbContext context = _dbFactory())
            {
                var handled = context.Events.Find(evnt.Id);
                handled.HandledOn = DateTime.UtcNow;
                context.SaveChanges();
            }
        }
    }
}
