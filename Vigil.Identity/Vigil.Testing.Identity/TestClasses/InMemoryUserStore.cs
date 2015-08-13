using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Identity.TestClasses
{
    public class InMemoryUserStore : IQueryableUserStore<VigilUser, Guid>,
        IUserStore<VigilUser, Guid>,
        IUserPasswordStore<VigilUser, Guid>,
        IUserLockoutStore<VigilUser, Guid>,
        IUserTwoFactorStore<VigilUser, Guid>
    {
        private readonly Dictionary<Guid, VigilUser> users = new Dictionary<Guid, VigilUser>();
        private readonly Dictionary<VigilUser, string> passwords = new Dictionary<VigilUser, string>();

        public InMemoryUserStore() { }

        public IQueryable<VigilUser> Users
        {
            get { return users.Values.AsQueryable(); }
        }

        public Task CreateAsync(VigilUser user)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Contract.Assume(user != null);

            users[user.Id] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task DeleteAsync(VigilUser user)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id), "Unknown user");

            users.Remove(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<VigilUser> FindByIdAsync(Guid userId)
        {
            Contract.Ensures(Contract.Result<Task<VigilUser>>() != null);

            if (users.ContainsKey(userId))
            {
                return Task.FromResult(users[userId]);
            }
            return Task.FromResult<VigilUser>(null);
        }

        public Task<VigilUser> FindByNameAsync(string userName)
        {
            Contract.Ensures(Contract.Result<Task<VigilUser>>() != null);

            VigilUser user = Users.SingleOrDefault(u => String.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)
                                                     || String.Equals(u.Email, userName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task UpdateAsync(VigilUser user)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Contract.Assume(user != null);

            users[user.Id] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(VigilUser user)
        {
            if (passwords.ContainsKey(user))
            {
                return Task.FromResult(passwords[user]);
            }
            return Task.FromResult<string>(null);
        }

        public Task<bool> HasPasswordAsync(VigilUser user)
        {
            return Task.FromResult(passwords.ContainsKey(user));
        }

        public Task SetPasswordHashAsync(VigilUser user, string passwordHash)
        {
            passwords[user] = passwordHash;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<int> GetAccessFailedCountAsync(VigilUser user)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            return Task.FromResult(users[user.Id].AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(VigilUser user)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            return Task.FromResult(users[user.Id].LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(VigilUser user)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            return Task.FromResult<DateTimeOffset>(new DateTimeOffset(users[user.Id].LockoutEndDateUtc ?? DateTime.MinValue, TimeSpan.Zero));
        }

        public Task<int> IncrementAccessFailedCountAsync(VigilUser user)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            return Task.FromResult(++users[user.Id].AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(VigilUser user)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            users[user.Id].AccessFailedCount = 0;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task SetLockoutEnabledAsync(VigilUser user, bool enabled)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            users[user.Id].LockoutEnabled = enabled;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task SetLockoutEndDateAsync(VigilUser user, DateTimeOffset lockoutEnd)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            users[user.Id].LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<bool> GetTwoFactorEnabledAsync(VigilUser user)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            return Task.FromResult(users[user.Id].TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(VigilUser user, bool enabled)
        {
            Contract.Assume(user != null);
            Contract.Assume(users.ContainsKey(user.Id));

            users[user.Id].TwoFactorEnabled = enabled;
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(users != null);
        }
    }
}
