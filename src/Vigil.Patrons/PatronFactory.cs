using System.Diagnostics.Contracts;
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

        public IKeyIdentity CreatePatron(CreatePatronCommand command)
        {
            Contract.Requires(command != null);
            Contract.Ensures(Contract.Result<IKeyIdentity>() != null);

            var key = KeyIdentity.NewIdentity();

            _queue.QueueCommand(command, key);

            return key;
        }
    }
}
