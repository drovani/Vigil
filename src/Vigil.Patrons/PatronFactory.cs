using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using Vigil.Domain;
using Vigil.MessageQueue;
using Vigil.MessageQueue.Commands;

namespace Vigil.Patrons
{
    public class PatronFactory
    {
        protected readonly ICommandQueue _queue;

        public PatronFactory(ICommandQueue queue)
        {
            _queue = queue;
        }

        public FactoryResult CreatePatron(CreatePatronCommand command)
        {
            Contract.Requires(command != null);
            Contract.Ensures(Contract.Result<IKeyIdentity>() != null);

            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            if (validationResults.Any())
            {
                return new FactoryResult(validationResults);
            }
            else
            {
                var key = KeyIdentity.NewIdentity();
                _queue.QueueCommand(command, key);
                return new FactoryResult(key);
            }
        }
    }
}
