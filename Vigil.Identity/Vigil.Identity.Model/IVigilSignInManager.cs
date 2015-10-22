using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public interface IVigilSignInManager : IDisposable
    {
        // Extracted from Microsoft.AspNet.Identity.Owin.SignInManager<VigilUser, Guid>
        IAuthenticationManager AuthenticationManager { get; set; }
        string AuthenticationType { get; set; }
        UserManager<VigilUser, Guid> UserManager { get; set; }
        Guid ConvertIdFromString(string id);
        string ConvertIdToString(Guid id);
        Task<ClaimsIdentity> CreateUserIdentityAsync(VigilUser user);
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);
        Task<Guid> GetVerifiedUserIdAsync();
        Task<bool> HasBeenVerifiedAsync();
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
        Task<bool> SendTwoFactorCodeAsync(string provider);
        Task SignInAsync(VigilUser user, bool isPersistent, bool rememberBrowser);
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
    }
}