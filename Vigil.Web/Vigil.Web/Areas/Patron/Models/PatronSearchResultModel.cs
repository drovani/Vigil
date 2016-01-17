using System.Collections.Generic;
using System.Linq;
using Vigil.Patrons.Model;

namespace Vigil.Web.Areas.Patron.Models
{
    public class PatronSearchResultModel
    {
        public PatronSearchModel SearchTerms { get; protected set; }
        public IEnumerable<PatronReadModel> Patrons { get; protected set; }
        public int TotalRecords { get; protected set; }

        public PatronSearchResultModel(PatronSearchModel searchTerms)
        {
            SearchTerms = searchTerms;
            if (searchTerms == null)
            {
                Patrons = Enumerable.Empty<PatronReadModel>();
            }
            else
            {
                PatronRepository repo = new PatronRepository();
                int totalRows;
                Patrons = repo.GetPatronsBySearch(searchTerms, out totalRows);

            }
        }
    }
}