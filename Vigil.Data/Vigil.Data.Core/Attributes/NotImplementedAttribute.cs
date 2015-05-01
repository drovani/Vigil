using System;

namespace Vigil.Data.Core.Attributes
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
