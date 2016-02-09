using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patrons.Model
{
    public class PatronVigilContext : BaseVigilContext<PatronVigilContext>, IVigilContext
    {
        public IDbSet<Patron> Patrons { get { return Set<Patron>(); } }
        public IDbSet<PatronType> PatronTypes { get { return Set<PatronType>(); } }

        public PatronVigilContext(string affectedBy, DateTime now)
            : base(affectedBy, now)
        {
            Contract.Requires<ArgumentNullException>(affectedBy != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(affectedBy));
            Contract.Requires<ArgumentException>(now != default(DateTime));
        }

        public TPatron Create<TPatron>(PatronType patronType, string displayName, string accountNumber = null, bool isAnonymous = false) where TPatron : Patron
        {
            Contract.Requires<ArgumentNullException>(patronType != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(displayName));
            Contract.Ensures(Contract.Result<TPatron>() != null);

            return Patron.Create(AffectedBy, Now, patronType, displayName, accountNumber, isAnonymous) as TPatron;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patron>();
            modelBuilder.Entity<PatronType>();

            base.OnModelCreating(modelBuilder);
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
