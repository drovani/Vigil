using System;

namespace Vigil.Domain.Messaging
{
    public abstract class Command : KeyIdentity, ICommand
    {
        public string GeneratedBy { get; protected set; }
        public DateTime GeneratedOn { get; protected set; }

        protected Command(string generatedBy, DateTime generatedOn)
        {
            if (string.IsNullOrEmpty(generatedBy)) throw new ArgumentNullException(nameof(generatedBy));
            if (generatedOn == default(DateTime)) throw new ArgumentException(nameof(generatedOn));

            GeneratedBy = generatedBy;
            GeneratedOn = generatedOn;
        }
    }
}
