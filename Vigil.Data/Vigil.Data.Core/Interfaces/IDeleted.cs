using System;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public interface IDeleted : ICreated
    {
        VigilUser DeletedBy { get; }
        DateTime? DeletedOn { get; }
    }
}
