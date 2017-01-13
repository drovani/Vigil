using Newtonsoft.Json;
using System;
using Vigil.Domain.Messaging;

namespace Vigil.Sql
{
    public class SqlEventBus : IEventBus
    {
        public void Publish<TEvent>(TEvent evnt) where TEvent : IEvent
        {
            using (CommandDbContext context = new CommandDbContext())
            {
                var newEvnt = new Event()
                {
                    GeneratedBy = evnt.GeneratedBy,
                    GeneratedOn = evnt.GeneratedOn,
                    Id = evnt.Id,
                    SourceId = evnt.SourceId,
                    SerializedEvent = JsonConvert.SerializeObject(evnt)
                };
                context.Events.Add(newEvnt);
                context.SaveChanges();
            }

            // TODO - figure out how to call the appropriate EventHandlers
            //throw new NotImplementedException();

            //using (CommandDbContext context = new CommandDbContext())
            //{
            //    var handled = context.Events.Find(evnt.Id);
            //    handled.HandledOn = DateTime.Now.ToUniversalTime();
            //    context.SaveChanges();
            //}
        }
    }
}
