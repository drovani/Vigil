using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.System
{
    public class ApplicationSetting : KeyIdentity
    {
        [Required]
        public string SettingName { get; set; }
        [Required]
        public string SettingValue { get; set; }
        public DateTime LastUpdated { get; set; }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(SettingName != null);
            Contract.Invariant(LastUpdated != DateTime.MinValue);
        }
    }
}
