namespace Vigil
{
    public interface Handles<T> where T : IDomainEvent
    {
        void Hande(T args);
    }
}
