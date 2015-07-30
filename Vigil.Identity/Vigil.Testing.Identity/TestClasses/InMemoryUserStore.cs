using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Identity.TestClasses
{
    internal class InMemoryUserStore : IQueryableUserStore<VigilUser, Guid>, IUserStore<VigilUser, Guid>, IUserPasswordStore<VigilUser, Guid>
    {
        private readonly Dictionary<Guid, VigilUser> users = new Dictionary<Guid, VigilUser>();
        private readonly Dictionary<VigilUser, string> passwords = new Dictionary<VigilUser, string>();

        public InMemoryUserStore()
        {
        }

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

            VigilUser user = Users.SingleOrDefault(u => String.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
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
