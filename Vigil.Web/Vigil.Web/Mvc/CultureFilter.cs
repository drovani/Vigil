using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace Vigil.Web.Mvc
{
    public class CultureFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var values = filterContext.RouteData.Values;
            string culture = (string)values["culture"] ?? "en-US";
            CultureInfo ci = new CultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);
        }
    }
}