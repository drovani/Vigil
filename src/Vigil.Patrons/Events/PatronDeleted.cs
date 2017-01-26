using System;

namespace Vigil.Patrons.Events
{
    public class PatronDeleted : PatronEvent
    {
        public PatronDeleted(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }
    }
}