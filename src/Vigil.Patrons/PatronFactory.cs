using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            List<ValidationResult> validationResults = ValidateCommand(command);

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

        public FactoryResult UpdatePatron(UpdatePatronCommand command)
        {
            List<ValidationResult> validationResults = ValidateCommand(command);

            if (validationResults.Any())
            {
                return new FactoryResult(validationResults);
            }
            else
            {
                _queue.QueueCommand(command, command.TargetPatron);
                return new FactoryResult(command.TargetPatron);
            }
        }

        protected List<ValidationResult> ValidateCommand(ICommand command)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true);

            return validationResults;
        }
    }
}
