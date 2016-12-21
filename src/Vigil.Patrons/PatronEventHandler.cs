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
                var patron = context.Patrons.FirstOrDefault(p => p.Id == evnt.PatronId);
                if (patron != null)
                {
                    patron.ModifiedBy = evnt.GeneratedBy;
                    patron.ModifiedOn = evnt.GeneratedOn;
                    patron.DisplayName = evnt.DisplayName ?? patron.DisplayName;
                    patron.IsAnonymous = evnt.IsAnonymous ?? patron.IsAnonymous;
                    patron.PatronType = evnt.PatronType ?? patron.PatronType;
                    context.SaveChanges();
                }
            }
        }

        public void Handle(PatronDeleted evnt)
        {
            using (var context = contextFactory.Invoke())
            {
                var patron = context.Patrons.FirstOrDefault(p => p.Id == evnt.PatronId);
                if (patron != null)
                {
                    patron.ModifiedBy = evnt.GeneratedBy;
                    patron.ModifiedOn = evnt.GeneratedOn;
                    patron.DeletedBy = evnt.GeneratedBy;
                    patron.DeletedOn = evnt.GeneratedOn;
                }
            }
        }
    }
}
