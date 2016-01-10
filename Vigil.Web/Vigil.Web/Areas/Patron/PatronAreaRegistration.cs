using System.Diagnostics.Contracts;
using System.Globalization;
using System.Web.Mvc;
using Vigil.Web.Mvc;

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
                defaults: new { controller = "Search", action = "Index", area = AreaName, culture = "en-US" },
                namespaces: new string[] { "Vigil.Web.Areas.Patron.Controllers" },
                constraints: new { controller = @"(Search)" }
            );
            context.MapRoute(
                name: "Patron_with_AccountNumber",
                url: "{culture}/Patron/{accountNumber}/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional, area = AreaName },
                namespaces: new string[] { "Vigil.Web.Areas.Patron.Controllers" },
                constraints: new { controller = @"(Account)" }
            );
        }
    }
}