using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.System
{
    public class ApplicationSetting : KeyIdentity
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public DateTime LastUpdated { get; set; }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(SettingName != null);
        }
    }
}
