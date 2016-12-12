namespace Vigil.Domain.Messaging
{
    public interface ICommandQueue
    {
        void Publish<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
