using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Data.Core.System;

namespace Vigil.Identity.Model
{
    public class VigilUserManager : UserManager<VigilUser, Guid>
    {
        public VigilUserManager(IUserStore<VigilUser, Guid> store) : base(store) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static VigilUserManager Create(IOwinContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<VigilUserManager>() != null);

            var manager = new VigilUserManager(new VigilUserStore(context.Get<IdentityVigilContext>()));

            return manager;
        }

        public override Task<IdentityResult> CreateAsync(VigilUser user)
        {
            Contract.Ensures(Contract.Result<Task<IdentityResult>>() != null);

            Contract.Assume(user != null);
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            var task = base.CreateAsync(user);
            Contract.Assume(task != null);
            return task;
        }
        public override Task<IdentityResult> CreateAsync(VigilUser user, string password)
        {
            Contract.Ensures(Contract.Result<Task<IdentityResult>>() != null);

            Contract.Assume(user != null);
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            var task = base.CreateAsync(user, password);
            Contract.Assume(task != null);
            return task;
        }
    }
}
