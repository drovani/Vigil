using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Data.TestClasses
{
    public class TestUserStore : IUserStore<VigilUser, Guid>
    {
        private readonly List<VigilUser> users = new List<VigilUser>();

        public Task CreateAsync(VigilUser user)
        {
            users.Add(user);
            return Task.FromResult<VigilUser>(user);
        }

        public Task DeleteAsync(VigilUser user)
        {
            var todelete = users.Find(vu => vu.Id == user.Id);
            users.Remove(todelete);
            return Task.FromResult<VigilUser>(todelete);
        }

        public Task<VigilUser> FindByIdAsync(Guid userId)
        {
            return Task.FromResult<VigilUser>(users.SingleOrDefault(u => u.Id == userId));
        }

        public Task<VigilUser> FindByNameAsync(string userName)
        {
            return Task.FromResult<VigilUser>(users.SingleOrDefault(u => String.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)));
        }

        public Task UpdateAsync(VigilUser user)
        {
            var toupdate = users.Find(vu => vu.Id == user.Id);
            return Task.FromResult<VigilUser>(toupdate);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
