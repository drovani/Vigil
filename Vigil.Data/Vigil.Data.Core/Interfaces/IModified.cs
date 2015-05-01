using System;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public interface IModified : ICreated
    {
        VigilUser ModifiedBy { get; }
        DateTime? ModifiedOn { get; }
    }
}
