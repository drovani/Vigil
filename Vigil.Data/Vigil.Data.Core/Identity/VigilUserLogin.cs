using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vigil.Data.Core.Identity
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
    public class VigilUserLogin : IdentityUserLogin<Guid>
    {
    }
}
