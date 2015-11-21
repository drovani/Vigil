namespace Vigil
{
    public interface IValueGenerator<TValue>
    {
        TValue GetNextValue();
    }
}
