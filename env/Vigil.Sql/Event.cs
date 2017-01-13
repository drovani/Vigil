using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vigil.Sql
{
    public class Event
    {
        public Guid Id { get; set; }
        public Guid SourceId { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string SerializedEvent { get; set; }

        public DateTime? HandledOn { get; set; }
    }
}
