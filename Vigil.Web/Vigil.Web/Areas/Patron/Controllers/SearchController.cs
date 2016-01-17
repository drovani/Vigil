using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Web.Mvc;
using Vigil.Patrons.Model;
using Vigil.Web.Areas.Patron.Models;
using Vigil.Patrons.Model.Types;

namespace Vigil.Web.Areas.Patron.Controllers
{
    [ContractVerification(false)]
    public class SearchController : Controller
    {
        // GET: Patron/Search
        [HttpGet]
        public ActionResult Index(PatronSearchModel search)
        {
            return View(new PatronSearchResultModel(search));
        }

        // GET: Patron/Search/Create
        [HttpGet]
        public ActionResult Create()
        {
            var patronTypes = new PatronRepository().GetPatronTypes();
            ViewBag.PatronTypes = new SelectList(patronTypes, nameof(PatronTypeModel.TypeName), nameof(PatronTypeModel.TypeName));

            return View(new PatronUpdateModel() { IsAnonymous = false });
        }

        // POST: Patron/Search/Create
        [HttpPost]
        public ActionResult Create(PatronCreateModel model)
        {
            PatronFactory facto = new PatronFactory(User.Identity.Name, DateTime.Now);
            PatronReadModel read = facto.CreatePatron(model);
            if (read == null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Account", new { area = "Patron", accountNumber = read.AccountNumber, culture = CultureInfo.CurrentUICulture });
            }
        }

    }
}