using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Vigil.Testing.Identity.TestClasses
{
    public class InMemoryAuthenticationHandler : AuthenticationHandler<InMemoryAuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var identity = new ClaimsIdentity(Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Options.UserId.ToString(), null, Options.AuthenticationType));
            identity.AddClaim(new Claim(ClaimTypes.Name, Options.UserName));
            var properties = Options.StateDataFormat.Unprotect(Request.Query["state"]);
            return Task.FromResult(new AuthenticationTicket(identity, properties));
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
                if (challenge != null)
                {
                    var state = challenge.Properties;
                    if (String.IsNullOrEmpty(state.RedirectUri))
                    {
                        state.RedirectUri = Request.Uri.ToString();
                    }

                    var stateString = Options.StateDataFormat.Protect(state);
                    Response.Redirect(WebUtilities.AddQueryString(Options.CallbackPath.Value, "state", stateString));
                }
            }
            return Task.FromResult<Object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                var ticket = await AuthenticateAsync();
                if (ticket != null)
                {
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identity);
                    Response.Redirect(ticket.Properties.RedirectUri);
                    return true;
                }
            }
            return false;
        }
    }
}
