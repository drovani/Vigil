using System;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Domain
{
    public abstract class KeyIdentity : IEquatable<KeyIdentity>, IKeyIdentity
    {
        [Key]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        protected KeyIdentity() { }

        /// <summary>Compares two Vigil.Data.Core.Identity classes for equality by Id.
        /// </summary>
        /// <param name="obj">The Vigil.Data.Core.Identity class to compare Id values.</param>
        /// <returns>Returns true if the two Id values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((KeyIdentity)obj);
        }

        public bool Equals(KeyIdentity other) => other == null ? false : Equals(other.Id, Id);

        public static bool operator ==(KeyIdentity left, KeyIdentity right) => Equals(left, right);
        public static bool operator !=(KeyIdentity left, KeyIdentity right) => !Equals(left, right);

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id.ToString();
    }
}
