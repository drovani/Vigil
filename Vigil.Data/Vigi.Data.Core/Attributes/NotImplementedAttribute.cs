using System;

namespace Vigi.Data.Core.Attributes
{
    public class NotImplementedAttribute:Attribute
    {
        public NotImplementedAttribute()
        {
            throw new NotImplementedException();
        }
    }
}
