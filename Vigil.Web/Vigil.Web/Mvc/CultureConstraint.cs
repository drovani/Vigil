using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Vigil.Web.Mvc
{
    public class CultureConstraint : IRouteConstraint
    {
        private readonly string DefaultCulture;
        private readonly string Pattern;

        public CultureConstraint(string defaultCulture, string pattern)
        {
            DefaultCulture = defaultCulture;
            Pattern = pattern;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration && DefaultCulture.Equals(values[parameterName]))
            {
                return false;
            }
            else
            {
                return Regex.IsMatch((string)values[parameterName], "^" + Pattern + "$");
            }
        }
    }
}