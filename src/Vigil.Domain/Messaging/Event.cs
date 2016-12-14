using System;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Domain.Messaging
{
    public abstract class Event : KeyIdentity, IEvent
    {
        /// <summary>The user that caused this event to occur.
        /// </summary>
        public string GeneratedBy { get; protected set; }
        public DateTime GeneratedOn { get; protected set; }
        /// <summary>The unique identifier of the command or event that caused this event to occur.
        /// </summary>
        public Guid SourceId { get; protected set; }

        protected Event(string generatedBy, DateTime generatedOn, Guid sourceId)
        {
            if (string.IsNullOrEmpty(generatedBy)) throw new ArgumentNullException(nameof(generatedBy));
            if (generatedOn == default(DateTime)) throw new ArgumentException($"{nameof(generatedOn)} requires a non-default value.", nameof(generatedOn));
            if (Guid.Empty.Equals(sourceId)) throw new ArgumentException($"{nameof(sourceId)} requires a non-default value.", nameof(sourceId));

            GeneratedBy = generatedBy;
            GeneratedOn = generatedOn;
            SourceId = sourceId;
        }
    }
}
