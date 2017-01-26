using System;

namespace Vigil.Patrons.Events
{
    public class PatronHeaderChanged : PatronEvent
    {
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
        public string PatronType { get; set; }

        public PatronHeaderChanged(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }
    }
}
