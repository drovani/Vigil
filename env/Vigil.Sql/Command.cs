using System;

namespace Vigil.Sql
{
    internal class Command
    {
        public Guid Id { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string SerializedCommand { get; set; }

        public DateTime? HandledOn { get; set; }
    }
}