using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;

namespace Vigil.Application
{
    public class ApplicationContext : BaseVigilContext<ApplicationContext>, IVigilContext
    {
        public DbSet<ApplicationSetting> ApplicationSettings { get; protected set; }

        public ApplicationContext() : base(new VigilUser(), DateTime.Now) { }

        public ApplicationContext(VigilUser affectedBy, DateTime now)
            : base(affectedBy, now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(now != default(DateTime));
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
        }
    }
}
