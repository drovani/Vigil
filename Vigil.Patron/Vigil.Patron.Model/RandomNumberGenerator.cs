using System;

namespace Vigil.Patrons.Model
{
    public class RandomNumberGenerator : IValueGenerator<string>
    {
        public string GetNextValue(DateTime now)
        {
            var r = new Random();
            return r.Next(1, 10000000).ToString();
        }
    }
}
