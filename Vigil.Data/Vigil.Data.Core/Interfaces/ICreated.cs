using System;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public interface ICreated
    {
        VigilUser CreatedBy { get; }
        DateTime CreatedOn { get; }
    }
}
