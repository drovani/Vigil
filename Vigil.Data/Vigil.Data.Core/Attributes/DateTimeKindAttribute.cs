using System;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [ContractVerification(false)]
    public sealed class DateTimeKindAttribute : Attribute
    {
        private readonly DateTimeKind _kind;
        public DateTimeKindAttribute(DateTimeKind kind)
        {
            this._kind = kind;
        }
        public DateTimeKind Kind { get { return _kind; } }
    }
}
