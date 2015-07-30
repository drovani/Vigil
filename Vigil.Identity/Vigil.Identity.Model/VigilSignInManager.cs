using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilSignInManager : SignInManager<VigilUser, Guid>
    {
        public VigilSignInManager(VigilUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        {
            Contract.Requires<ArgumentNullException>(userManager != null);
            Contract.Requires<ArgumentNullException>(authenticationManager != null);
        }

        public static VigilSignInManager Create(IdentityFactoryOptions<VigilSignInManager> options, IOwinContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<VigilSignInManager>() != null);

            VigilUserManager manager = context.GetUserManager<VigilUserManager>();

            Contract.Assume(manager != null);
            Contract.Assume(context.Authentication != null);

            return new VigilSignInManager(manager, context.Authentication);
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(VigilUser user)
        {
            Contract.Ensures(Contract.Result<Task<ClaimsIdentity>>() != null);

            VigilUserManager vum = UserManager as VigilUserManager;
            Task<ClaimsIdentity> claim = vum.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            Contract.Assume(claim != null);

            return claim;
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(UserManager != null);
            Contract.Invariant(UserManager is VigilUserManager);
            Contract.Invariant(AuthenticationManager != null);
        }

    }
}
