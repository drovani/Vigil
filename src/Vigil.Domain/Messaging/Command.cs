using System;

namespace Vigil.Domain.Messaging
{
    public abstract class Command : KeyIdentity, ICommand
    {
        public string GeneratedBy { get; protected set; }
        public DateTime GeneratedOn { get; protected set; }

        protected Command(string generatedBy, DateTime generatedOnUtc)
        {
            if (string.IsNullOrEmpty(generatedBy)) throw new ArgumentNullException(nameof(generatedBy));
            if (generatedOnUtc == default(DateTime)) throw new ArgumentException($"{nameof(generatedOnUtc)} requires a non-default value.", nameof(generatedOnUtc));
            if (generatedOnUtc.Kind != DateTimeKind.Utc) throw new ArgumentException($"{nameof(generatedOnUtc)} must be DateTimeKind.UTC.", nameof(generatedOnUtc));

            GeneratedBy = generatedBy;
            GeneratedOn = generatedOnUtc;
        }
    }
}
