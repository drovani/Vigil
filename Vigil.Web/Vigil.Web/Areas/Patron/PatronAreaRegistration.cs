using System.Diagnostics.Contracts;
using System.Globalization;
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
                name: "Patron_without_AccountNumber",
                url: "{culture}/Patron/{controller}/{action}",
                defaults: new { culture = CultureInfo.CurrentCulture.Name, action = "Index" },
                namespaces: new string[] { "Vigil.Web.Area.Patron.Controllers" }
            );
            context.MapRoute(
                name: "Patron_with_AccountNumber",
                url: "{culture}/Patron/{accountNumber}/{controller}/{action}/{id}",
                defaults: new { culture = CultureInfo.CurrentCulture.Name, action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Vigil.Web.Area.Patron.Controllers" }
            );
        }
    }
}