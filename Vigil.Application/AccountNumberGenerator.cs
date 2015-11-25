using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using Vigil.Data.Core.System;

namespace Vigil.Application
{
    [Export("AccountNumberGenerator", typeof(IValueGenerator<>))]
    public class AccountNumberGenerator : IValueGenerator<string>, IDisposable
    {
        protected IApplicationContext applicationContext { get; set; }

        [ImportingConstructor]
        public AccountNumberGenerator([Import(typeof(IApplicationContext))] IApplicationContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);

            applicationContext = context;
        }

        public string GetNextValue(DateTime now)
        {
            using (DbContextTransaction trans = applicationContext.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                ApplicationSetting setting = applicationContext.ApplicationSettings.SingleOrDefault(appSet => appSet.SettingName == "AccountNumber");
                if (setting == null)
                {
                    setting = new ApplicationSetting()
                    {
                        SettingName = "AccountNumber",
                        SettingValue = "0"
                    };
                }
                int numericValue = Int32.Parse(setting.SettingValue) + 1;
                while (numericValue.ToString().Contains("666"))
                {
                    numericValue++;
                }
                setting.SettingValue = numericValue.ToString();
                setting.LastUpdated = now;
                applicationContext.SaveChanges();
                trans.Commit();

                return setting.SettingValue;
            }
        }

        public void Dispose()
        {
            if (applicationContext != null)
            {
                applicationContext.Dispose();
            }
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(applicationContext != null);
        }
    }
}
