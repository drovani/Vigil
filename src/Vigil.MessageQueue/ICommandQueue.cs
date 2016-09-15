using Vigil.Domain;

namespace Vigil.MessageQueue
{
    public interface ICommandQueue
    {
        void QueueCommand(ICommand command, IKeyIdentity key);
    }
}
