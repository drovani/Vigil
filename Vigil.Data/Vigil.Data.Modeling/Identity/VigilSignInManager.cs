using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Vigil.Data.Core.System;

namespace Vigil.Data.Modeling.Identity
{
    public class VigilSignInManager : SignInManager<VigilUser, Guid>
    {
        public VigilSignInManager(VigilUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        {
            Contract.Requires<ArgumentNullException>(userManager != null);
            Contract.Requires<ArgumentNullException>(authenticationManager != null);
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(VigilUser user)
        {
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Ensures(Contract.Result<Task<ClaimsIdentity>>() != null);

            return ((VigilUserManager)UserManager).CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public static VigilSignInManager Create(IdentityFactoryOptions<VigilSignInManager> options, IOwinContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<VigilSignInManager>() != null);

            return new VigilSignInManager(context.GetUserManager<VigilUserManager>(), context.Authentication);
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(UserManager != null);
            Contract.Invariant(AuthenticationManager != null);
        }

    }
}
