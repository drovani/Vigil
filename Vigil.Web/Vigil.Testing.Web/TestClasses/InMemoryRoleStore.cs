using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Web.TestClasses
{
    internal class InMemoryRoleStore : IQueryableRoleStore<VigilRole, Guid>, IRoleStore<VigilRole, Guid>
    {
        private readonly Dictionary<Guid, VigilRole> roles = new Dictionary<Guid, VigilRole>();

        public IQueryable<VigilRole> Roles
        {
            get { return roles.Values.AsQueryable(); }
        }

        public Task CreateAsync(VigilRole role)
        {
            roles[role.Id] = role;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task DeleteAsync(VigilRole role)
        {
            if (role == null || !roles.ContainsKey(role.Id))
            {
                throw new InvalidOperationException("Unknown role");
            }
            roles.Remove(role.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<VigilRole> FindByIdAsync(Guid roleId)
        {
            if (roles.ContainsKey(roleId))
            {
                return Task.FromResult(roles[roleId]);
            }
            return Task.FromResult<VigilRole>(null);
        }

        public Task<VigilRole> FindByNameAsync(string roleName)
        {
            VigilRole role = Roles.SingleOrDefault(r => String.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(role);
        }

        public Task UpdateAsync(VigilRole role)
        {
            roles[role.Id] = role;
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }
    }
}
