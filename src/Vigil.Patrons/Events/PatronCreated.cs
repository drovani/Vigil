using System;

namespace Vigil.Patrons.Events
{
    public class PatronCreated : PatronEvent
    {
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string PatronType { get; set; }

        public PatronCreated(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) {
            Version = 0;
        }
    }
}
