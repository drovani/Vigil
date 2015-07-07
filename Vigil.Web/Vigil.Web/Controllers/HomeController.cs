using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace Vigil.Web.Controllers
{
    [ContractVerification(false)]
    public class HomeController : Controller
    {
        [HttpGet]
        public ViewResult Index()
        {
            Contract.Ensures(Contract.Result<ViewResult>() != null);

            return View();
        }
    }
}