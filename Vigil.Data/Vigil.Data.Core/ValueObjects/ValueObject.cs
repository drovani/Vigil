using System;
using System.Collections.Generic;
using System.Reflection;

namespace Vigil.Data.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Cribbed from http://grabbagoft.blogspot.com/2007/06/generic-value-object-equality.html</remarks>
    /// <typeparam name="TValueObject"></typeparam>
    public abstract class ValueObject<TValueObject> : IEquatable<TValueObject>
        where TValueObject : ValueObject<TValueObject>
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            TValueObject other = obj as TValueObject;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            IEnumerable<FieldInfo> fields = GetFields();

            int startValue = 17;
            int multiplier = 59;

            int hashCode = startValue;

            foreach (FieldInfo field in fields)
            {
                object fieldvalue = field.GetValue(this);
                if (fieldvalue != null)
                {
                    hashCode = hashCode * multiplier + fieldvalue.GetHashCode();
                }
            }

            return hashCode;
        }

        public virtual bool Equals(TValueObject other)
        {
            if (other == null)
            {
                return false;
            }

            Type type = GetType();
            Type otherType = other.GetType();

            if (type != otherType)
            {
                return false;
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                object value1 = field.GetValue(other);
                object value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                    {
                        return false;
                    }
                }
                else if (!value1.Equals(value2))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator ==(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        {
            return !left.Equals(right);
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            Type type = GetType();
            List<FieldInfo> fields = new List<FieldInfo>();
            while (type != typeof(object))
            {
                fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                type = type.BaseType;
            }
            return fields;
        }
    }
}
