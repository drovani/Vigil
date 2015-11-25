using System;

namespace Vigil
{
    public interface IValueGenerator<TValue>
    {
        TValue GetNextValue(DateTime now);
    }
}
