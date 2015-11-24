using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Vigil.Data.Core.System;

namespace Vigil.Application
{
    [Export("AccountNumberGenerator", typeof(IValueGenerator<>))]
    public class AccountNumberGenerator : IValueGenerator<string>
    {
        public string GetNextValue()
        {
            using (ApplicationContext context = new ApplicationContext())
            using (DbContextTransaction trans = context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                ApplicationSetting setting = context.ApplicationSettings.SingleOrDefault(appSet => appSet.SettingName == "AccountNumber");
                if (setting == null)
                {
                    setting = new ApplicationSetting()
                    {
                        SettingName = "AccountNumber",
                        SettingValue = "0"
                    };
                }
                setting.SettingValue = (Int32.Parse(setting.SettingValue) + 1).ToString();
                setting.LastUpdated = DateTime.Now;
                context.SaveChanges();
                trans.Commit();

                return setting.SettingValue;
            }
        }
    }
}
