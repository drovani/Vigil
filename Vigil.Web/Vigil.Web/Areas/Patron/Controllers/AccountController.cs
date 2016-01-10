using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using Vigil.Patron.Model;

namespace Vigil.Web.Areas.Patron.Controllers
{
    [ContractVerification(false)]
    public class AccountController : Controller
    {
        // GET: Patron/850827/Account
        [HttpGet]
        public ActionResult Index(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return View();
            }
            PatronRepository repo = new PatronRepository();
            PatronReadModel read = repo.GetByAccountNumber(accountNumber);
            if (read == null)
            {
                return RedirectToAction("Index", "Account");
            }
            return View("Account", read);
        }

        // GET: Patron/850827/Account/Edit
        [HttpGet]
        public ActionResult Edit(string accountNumber)
        {
            PatronReadModel model = new PatronRepository().GetByAccountNumber(accountNumber);
            return View(model);
        }

        // PUT: Patron/850827/Account/Edit
        [HttpPut]
        public ActionResult Edit(string accountNumber, PatronUpdateModel model)
        {
            PatronFactory factory = new PatronFactory(User.Identity.Name, DateTime.Now);
            PatronReadModel readModel = factory.UpdatePatron(model, accountNumber);
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
            PatronRepository repo = new PatronRepository();
            PatronReadModel readModel = repo.GetByAccountNumber(accountNumber);
            return View(readModel);
        }

        // DELETE: Patron/850827/Account/Delete
        [HttpDelete]
        public ActionResult Delete(string accountNumber, string reason)
        {
            PatronFactory facto = new PatronFactory(User.Identity.Name, DateTime.Now);
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
