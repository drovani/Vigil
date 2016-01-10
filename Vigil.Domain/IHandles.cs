namespace Vigil
{
    public interface IHandles<T> where T : IDomainEvent
    {
        void Hande(T args);
    }
}
