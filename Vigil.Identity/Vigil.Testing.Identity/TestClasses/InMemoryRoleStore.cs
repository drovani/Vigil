using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Testing.Identity.TestClasses
{
    [ContractVerification(false)]
    internal class InMemoryRoleStore : IQueryableRoleStore<VigilRole, Guid>, IRoleStore<VigilRole, Guid>
    {
        private readonly Dictionary<Guid, VigilRole> roles = new Dictionary<Guid, VigilRole>();

        public IQueryable<VigilRole> Roles
        {
            get { return roles.Values.AsQueryable(); }
        }

        public Task CreateAsync(VigilRole role)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Contract.Assume(role != null);

            roles[role.Id] = role;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task DeleteAsync(VigilRole role)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Contract.Assume(role != null);
            Contract.Assume(roles.ContainsKey(role.Id), "Unknown role");

            roles.Remove(role.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<VigilRole> FindByIdAsync(Guid roleId)
        {
            Contract.Ensures(Contract.Result<Task<VigilRole>>() != null);

            if (roles.ContainsKey(roleId))
            {
                return Task.FromResult(roles[roleId]);
            }
            return Task.FromResult<VigilRole>(null);
        }

        public Task<VigilRole> FindByNameAsync(string roleName)
        {
            Contract.Ensures(Contract.Result<Task<VigilRole>>() != null);

            VigilRole role = Roles.SingleOrDefault(r => String.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(role);
        }

        public Task UpdateAsync(VigilRole role)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Contract.Assume(role != null);

            roles[role.Id] = role;
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
            Contract.Invariant(roles != null);
        }
    }
}
