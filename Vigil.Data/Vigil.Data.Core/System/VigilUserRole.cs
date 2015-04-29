using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vigil.Data.Core.System
{
    public class VigilUserRole : IdentityUserRole<Guid>
    {
    }
}
