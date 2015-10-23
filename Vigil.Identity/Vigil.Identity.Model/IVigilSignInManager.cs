using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    [ContractClass(typeof(Contracts.IVigilSignInManagerContract))]
    public interface IVigilSignInManager : IDisposable
    {
        // Extracted from Microsoft.AspNet.Identity.Owin.SignInManager<VigilUser, Guid>
        IAuthenticationManager AuthenticationManager { get; }
        string AuthenticationType { get; }
        UserManager<VigilUser, Guid> UserManager { get; }
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
    namespace Contracts
    {
        [ContractClassFor(typeof(IVigilSignInManager))]
        internal abstract class IVigilSignInManagerContract : IVigilSignInManager
        {
            public IAuthenticationManager AuthenticationManager
            {
                get { throw new NotImplementedException(); }
            }

            public string AuthenticationType
            {
                get { throw new NotImplementedException(); }
            }

            public UserManager<VigilUser, Guid> UserManager
            {
                get { throw new NotImplementedException(); }
            }

            public Guid ConvertIdFromString(string id)
            {
                throw new NotImplementedException();
            }

            public string ConvertIdToString(Guid id)
            {
                throw new NotImplementedException();
            }

            public Task<ClaimsIdentity> CreateUserIdentityAsync(VigilUser user)
            {
                throw new NotImplementedException();
            }

            public Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
            {
                throw new NotImplementedException();
            }

            public Task<Guid> GetVerifiedUserIdAsync()
            {
                throw new NotImplementedException();
            }

            public Task<bool> HasBeenVerifiedAsync()
            {
                throw new NotImplementedException();
            }

            public Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
            {
                throw new NotImplementedException();
            }

            public Task<bool> SendTwoFactorCodeAsync(string provider)
            {
                throw new NotImplementedException();
            }

            public Task SignInAsync(VigilUser user, bool isPersistent, bool rememberBrowser)
            {
                throw new NotImplementedException();
            }

            public Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}