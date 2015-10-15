using System;
using System.Diagnostics.Contracts;
using System.Linq;

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

        public static void Apply(object entity)
        {
            if (entity == null) return;

            var properties = entity.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?))
                .Where(x => x.CustomAttributes.OfType<DateTimeKindAttribute>().Any());
            foreach (var property in properties)
            {
                var attr = property.CustomAttributes.OfType<DateTimeKindAttribute>().Single();
                var dt = property.PropertyType == typeof(DateTime?)
                    ? (DateTime?)property.GetValue(entity)
                    : (DateTime)property.GetValue(entity);
                if (dt == null) continue;

                property.SetValue(entity, DateTime.SpecifyKind(dt.Value, attr.Kind));
            }
        }
    }
}
