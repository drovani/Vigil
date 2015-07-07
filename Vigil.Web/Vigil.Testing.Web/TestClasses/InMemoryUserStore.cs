using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;

namespace Vigil.Testing.Web.TestClasses
{
    internal class InMemoryUserStore : IQueryableUserStore<VigilUser, Guid>, IUserStore<VigilUser, Guid>
    {
        private readonly Dictionary<Guid, VigilUser> users = new Dictionary<Guid, VigilUser>();
        private readonly IdentityVigilContext identityVigilContext;

        public InMemoryUserStore(IdentityVigilContext identityVigilContext)
        {
            this.identityVigilContext = identityVigilContext;
        }

        public IQueryable<VigilUser> Users
        {
            get { return users.Values.AsQueryable(); }
        }

        public Task CreateAsync(VigilUser user)
        {
            users[user.Id] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task DeleteAsync(VigilUser user)
        {
            if (user == null || !users.ContainsKey(user.Id))
            {
                throw new InvalidOperationException("Unknown user");
            }
            users.Remove(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<VigilUser> FindByIdAsync(Guid userId)
        {
            if (users.ContainsKey(userId))
            {
                return Task.FromResult(users[userId]);
            }
            return Task.FromResult<VigilUser>(null);
        }

        public Task<VigilUser> FindByNameAsync(string userName)
        {
            VigilUser user = Users.SingleOrDefault(u => String.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task UpdateAsync(VigilUser user)
        {
            users[user.Id] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }
    }
}
