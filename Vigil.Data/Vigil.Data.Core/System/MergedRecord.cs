using System;

namespace Vigil.Data.Core.System
{
    public class MergedRecord : KeyIdentity
    {
        public Guid RecordId { get; protected set; }
        public Guid OriginalPatronId { get; protected set; }
        public Guid TargetPatronId { get; protected set; }
    }
}
