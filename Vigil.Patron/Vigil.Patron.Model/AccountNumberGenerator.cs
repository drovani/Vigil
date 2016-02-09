using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using Vigil.Data.Core;
using Vigil.Data.Core.System;

namespace Vigil.Patrons.Model
{
    [Export("AccountNumberGenerator", typeof(IValueGenerator<>))]
    public sealed class AccountNumberGenerator : IValueGenerator<string>, IDisposable
    {
        private PatronVigilContext Context { get; set; }

        [ImportingConstructor]
        public AccountNumberGenerator(PatronVigilContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);

            Context = context;
        }

        public string GetNextValue(DateTime now)
        {
            using (DbContextTransaction trans =  Context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                ApplicationSetting setting = Context.Set<ApplicationSetting>().SingleOrDefault(appSet => appSet.SettingName == "AccountNumber");
                if (setting == null)
                {
                    setting = new ApplicationSetting()
                    {
                        SettingName = "AccountNumber",
                        SettingValue = "0"
                    };
                }
                int numericValue = int.Parse(setting.SettingValue, CultureInfo.InvariantCulture) + 1;
                while (numericValue.ToString(CultureInfo.InvariantCulture).Contains("666"))
                {
                    numericValue++;
                }
                setting.SettingValue = numericValue.ToString(CultureInfo.InvariantCulture);
                setting.LastUpdated = now;
                Context.SaveChanges();
                trans.Commit();

                return setting.SettingValue;
            }
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Context != null);
        }
    }
}
