using System;
using Ardalis.GuardClauses;
using Vigil.Framework;

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
            Guard.Against.NonUtcDateTimeKind(generatedOnUtc, nameof(generatedOnUtc));

            GeneratedBy = generatedBy;
            GeneratedOn = generatedOnUtc;
        }
    }
}
