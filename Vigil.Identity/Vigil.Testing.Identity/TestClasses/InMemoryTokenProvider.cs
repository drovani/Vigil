using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Identity.TestClasses
{
    public class InMemoryTokenProvider : IUserTokenProvider<VigilUser, Guid>
    {
        public async Task<string> GenerateAsync(string purpose, UserManager<VigilUser, Guid> manager, VigilUser user)
        {
            Contract.Assume(manager != null);
            var token = await manager.GenerateUserTokenAsync(purpose, user.Id);
            return token;
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<VigilUser, Guid> manager, VigilUser user)
        {
            return Task.FromResult(true);
        }

        public Task NotifyAsync(string token, UserManager<VigilUser, Guid> manager, VigilUser user)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<VigilUser, Guid> manager, VigilUser user)
        {
            Contract.Assume(manager != null);
            var generatedToken = await manager.GenerateUserTokenAsync(purpose, user.Id);
            return token == generatedToken;
        }
    }
}
