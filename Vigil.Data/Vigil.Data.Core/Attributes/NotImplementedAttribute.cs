using System;

namespace Vigil.Data.Core.Attributes
{
    public class NotImplementedAttribute : Attribute
    {
        public NotImplementedAttribute()
        {
            throw new NotImplementedException();
        }
    }
}
