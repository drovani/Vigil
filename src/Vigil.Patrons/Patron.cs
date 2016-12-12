using System;
using System.Collections.Generic;
using Vigil.Domain.EventSourcing;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public class Patron : EventSourced
    {
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string PatronType { get; set; }

        public Patron(Guid patronId, IEnumerable<IVersionedEvent> history)
            : this(patronId)
        {
            LoadFrom(history);
        }

        protected Patron(Guid patronId) : base(patronId)
        {
            Handles<PatronCreated>(OnPatronCreated);
            Handles<PatronHeaderChanged>(OnPatronHeaderChanged);
            Handles<PatronDeleted>(OnPatronDeleted);
        }

        private void OnPatronCreated(PatronCreated evnt)
        {
            DisplayName = evnt.DisplayName;
            IsAnonymous = evnt.IsAnonymous;
            PatronType = evnt.PatronType;
            CreatedBy = evnt.GeneratedBy;
            CreatedOn = evnt.GeneratedOn;
        }

        private void OnPatronHeaderChanged(PatronHeaderChanged evnt)
        {
            DisplayName = evnt.DisplayName ?? DisplayName;
            IsAnonymous = evnt.IsAnonymous ?? IsAnonymous;
            PatronType = evnt.PatronType ?? PatronType;
            ModifiedBy = evnt.GeneratedBy;
            ModifiedOn = evnt.GeneratedOn;
        }

        private void OnPatronDeleted(PatronDeleted evnt)
        {
            ModifiedBy = evnt.GeneratedBy;
            ModifiedOn = evnt.GeneratedOn;
            DeletedBy = evnt.GeneratedBy;
            DeletedOn = evnt.GeneratedOn;
        }
    }
}