namespace Vigil.Domain.Messaging
{
    public interface ICommandRepository
    {
        void Save<TCommand>(TCommand command) where TCommand : Command;
    }
}
