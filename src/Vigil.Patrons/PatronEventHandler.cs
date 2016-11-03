using System;
using System.Linq;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class PatronEventHandler :
        IEventHandler<PatronCreated>,
        IEventHandler<PatronHeaderChanged>
    {
        private Func<IPatronContext> contextFactory;

        public PatronEventHandler(Func<IPatronContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void Handle(PatronCreated @event)
        {
            using (var context = contextFactory.Invoke())
            {
                context.Patrons.Add(new Patron(@event.PatronId, new []{ @event }));
                context.SaveChanges();
            }
        }

        public void Handle(PatronHeaderChanged @event)
        {
            using (var context = contextFactory.Invoke())
            {
                var patron = context.Patrons.FirstOrDefault(p => p.Id == @event.PatronId);
                if (patron != null)
                {
                    patron.DisplayName = @event.DisplayName ?? patron.DisplayName;
                    patron.IsAnonymous = @event.IsAnonymous ?? patron.IsAnonymous;
                    patron.PatronType = @event.PatronType ?? patron.PatronType;
                    context.SaveChanges();
                }
            }
        }
    }
}
