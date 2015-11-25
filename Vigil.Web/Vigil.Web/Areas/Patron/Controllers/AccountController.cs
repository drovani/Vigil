using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using Vigil.Data.Core.Identity;
using Vigil.Patron.Model;

namespace Vigil.Web.Areas.Patron.Controllers
{
    [ContractVerification(false)]
    public class AccountController : Controller
    {
        // Get: Patron/Account
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Patron/850827/Account
        [HttpGet]
        public ActionResult Index(string accountNumber)
        {
            PatronRepository repo = new PatronRepository(User as VigilUser, DateTime.Now);
            PatronReadModel read = repo.GetByAccountNumber(accountNumber);
            if (read == null)
            {
                return RedirectToAction("Index", "Account");
            }
            return View("Account", read);
        }

        // GET: Patron/Account/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patron/Account/Create
        [HttpPost]
        public ActionResult Create(PatronUpdateModel model)
        {
            PatronFactory facto = new PatronFactory(User as VigilUser, DateTime.Now);
            PatronReadModel read = facto.UpdatePatron(model);
            if (read == null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Account", new { area = "Patron", accountNumber = read.AccountNumber });
            }
        }

        // GET: Patron/850827/Account/Edit
        [HttpGet]
        public ActionResult Edit(string accountNumber)
        {
            return View();
        }

        // POST: Patron/850827/Account/Edit
        [HttpPut]
        public ActionResult Edit(string accountNumber, PatronUpdateModel model)
        {
            PatronFactory factory = new PatronFactory(User as VigilUser, DateTime.Now);
            PatronReadModel readModel = factory.UpdatePatron(model);
            if (readModel != null)
            {
                return RedirectToAction("Index", "Account", new { accountNumber = readModel.AccountNumber });
            }
            else
            {
                return View(model);
            }
        }

        // GET: Patron/850827/Account/Delete
        [HttpGet]
        public ActionResult Delete(string accountNumber)
        {
            return View();
        }

        // DELETE: Patron/850827/Account/Delete
        [HttpDelete]
        public ActionResult Delete(string accountNumber, string reason)
        {
            PatronFactory facto = new PatronFactory(User as VigilUser, DateTime.Now);
            if (facto.DeletePatron(accountNumber, reason))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }
    }
}
