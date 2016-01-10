using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using Vigil.Data.Core.System;

namespace Vigil.Application
{
    [Export("AccountNumberGenerator", typeof(IValueGenerator<>))]
    public sealed class AccountNumberGenerator : IValueGenerator<string>, IDisposable
    {
        private IApplicationContext ApplicationContext { get; set; }

        [ImportingConstructor]
        public AccountNumberGenerator([Import(typeof(IApplicationContext))] IApplicationContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);

            ApplicationContext = context;
        }

        public string GetNextValue(DateTime now)
        {
            using (DbContextTransaction trans = ApplicationContext.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                ApplicationSetting setting = ApplicationContext.ApplicationSettings.SingleOrDefault(appSet => appSet.SettingName == "AccountNumber");
                if (setting == null)
                {
                    setting = new ApplicationSetting()
                    {
                        SettingName = "AccountNumber",
                        SettingValue = "0"
                    };
                }
                int numericValue = Int32.Parse(setting.SettingValue, CultureInfo.InvariantCulture) + 1;
                while (numericValue.ToString(CultureInfo.InvariantCulture).Contains("666"))
                {
                    numericValue++;
                }
                setting.SettingValue = numericValue.ToString(CultureInfo.InvariantCulture);
                setting.LastUpdated = now;
                ApplicationContext.SaveChanges();
                trans.Commit();

                return setting.SettingValue;
            }
        }

        public void Dispose()
        {
            if (ApplicationContext != null)
            {
                ApplicationContext.Dispose();
            }
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(ApplicationContext != null);
        }
    }
}
