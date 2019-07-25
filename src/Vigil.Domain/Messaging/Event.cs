using System;
using Ardalis.GuardClauses;

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

        protected Event(string generatedBy, DateTime generatedOnUtc, Guid sourceId)
        {
            Guard.Against.NullOrEmpty(generatedBy, nameof(generatedBy));
            Guard.Against.Default(generatedOnUtc, nameof(generatedOnUtc));
            if (generatedOnUtc.Kind != DateTimeKind.Utc) throw new ArgumentException($"{nameof(generatedOnUtc)} must be DateTimeKind.UTC.", nameof(generatedOnUtc));
            Guard.Against.Default(sourceId, nameof(sourceId));

            GeneratedBy = generatedBy;
            GeneratedOn = generatedOnUtc;
            SourceId = sourceId;
        }
    }
}
