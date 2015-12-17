using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Application
{
    [Export(typeof(IApplicationContext))]
    public class ApplicationContext : BaseVigilContext<ApplicationContext>, IApplicationContext
    {
        public IDbSet<ApplicationSetting> ApplicationSettings { get; protected set; }

        [ImportingConstructor]
        public ApplicationContext(string affectedBy, DateTime now)
            : base(affectedBy, now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(affectedBy.Trim() != string.Empty);
            Contract.Requires<ArgumentOutOfRangeException>(now != default(DateTime));
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Database.BeginTransaction(isolationLevel);
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Database != null);
        }
    }
}
