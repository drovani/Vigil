using System;
using Ardalis.GuardClauses;

namespace Vigil.Domain.Messaging
{
    public abstract class Command : KeyIdentity, ICommand
    {
        public string GeneratedBy { get; protected set; }
        public DateTime GeneratedOn { get; protected set; }

        protected Command(string generatedBy, DateTime generatedOnUtc)
        {
            Guard.Against.NullOrEmpty(generatedBy, nameof(generatedBy));
            Guard.Against.Default(generatedOnUtc, nameof(generatedOnUtc));
            if (generatedOnUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(generatedOnUtc)} must be DateTimeKind.UTC.", nameof(generatedOnUtc));
            }

            GeneratedBy = generatedBy;
            GeneratedOn = generatedOnUtc;
        }
    }
}
