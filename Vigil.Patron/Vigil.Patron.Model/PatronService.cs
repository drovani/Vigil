using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vigil.Data.Core.Identity;

namespace Vigil.Patron.Model
{
    public class PatronService
    {
        protected readonly PatronVigilContext context;

        public PatronService(VigilUser affectedBy, DateTime now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<AggregateException>(now != default(DateTime));

            context = new PatronVigilContext(affectedBy, now);
        }

        public IEnumerable<Patron> GetPatronsByType(string patronType)
        {
            return context.Patrons.Where(p => p.PatronType.TypeName == patronType)
                    .Select(p => new Patron(p))
                    .AsEnumerable();
        }
    }
}
