using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class PatronEventHandler :
        IEventHandler<PatronCreated>,
        IEventHandler<PatronHeaderChanged>,
        IEventHandler<PatronDeleted>
    {
        private Func<IPatronContext> contextFactory;

        public PatronEventHandler(Func<IPatronContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void Handle(PatronCreated evnt)
        {
            var context = contextFactory.Invoke();
            var patron = new Patron(evnt.PatronId, new[] { evnt });
            context.Patrons.Add(patron);
            context.SaveChanges();
        }

        public void Handle(PatronHeaderChanged evnt)
        {
            using (var context = contextFactory.Invoke())
            {
                var patron = context.Patrons.Find(evnt.PatronId);

                if (patron != null)
                {
                    patron.Update(evnt);
                    context.SaveChanges();
                }
            }
        }

        public void Handle(PatronDeleted evnt)
        {
            using (var context = contextFactory.Invoke())
            {
                var patron = context.Patrons.Find(evnt.PatronId);
                if (patron != null)
                {
                    patron.Update(evnt);
                    context.SaveChanges();
                }
            }
        }
    }
}
