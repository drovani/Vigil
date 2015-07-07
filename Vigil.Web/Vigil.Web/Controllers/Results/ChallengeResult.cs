using System;
using System.Diagnostics.Contracts;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Vigil.Web.Controllers.Results
{
    public class ChallengeResult : HttpUnauthorizedResult
    {
        public static readonly string XsrfKey = "XsrfId";

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }
        public Guid UserId { get; set; }

        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, Guid.Empty)
        {
        }

        public ChallengeResult(string provider, string redirectUri, Guid userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            Contract.Assume(context != null);

            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId.ToString();
            }
            IOwinContext owinContext = context.HttpContext.GetOwinContext();
            Contract.Assume(owinContext != null);
            Contract.Assume(owinContext.Authentication != null);

            owinContext.Authentication.Challenge(properties, LoginProvider);
        }
    }
}