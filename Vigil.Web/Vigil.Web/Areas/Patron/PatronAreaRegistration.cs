using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace Vigil.Web.Areas.Patron
{
    public class PatronAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Patron";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            Contract.Assume(context != null);

            context.MapRoute(
                name: "Patron_with_AccountNumber",
                url: "Patron/{accountNumber}/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Patron_default",
                "Patron/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}