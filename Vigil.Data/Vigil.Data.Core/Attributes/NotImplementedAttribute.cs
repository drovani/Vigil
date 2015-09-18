using System;

namespace Vigil.Data.Core
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class NotImplementedAttribute : Attribute
    {
        public NotImplementedAttribute()
        {
            throw new NotImplementedException();
        }
    }
}
