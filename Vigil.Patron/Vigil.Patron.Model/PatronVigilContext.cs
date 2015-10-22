using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Patron.Model
{
    public class PatronVigilContext : BaseVigilContext<PatronVigilContext>, IVigilContext
    {
        public VigilUser AffectedBy { get; protected set; }
        public DateTime Now { get; protected set; }

        public DbSet<Vigil.Data.Core.Patrons.PatronState> Patrons { get; protected set; }

        public PatronVigilContext(VigilUser affectedBy, DateTime now)
            : base()
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<AggregateException>(now != default(DateTime));

            AffectedBy = affectedBy;
            Now = now.ToUniversalTime();
        }
    }
}
