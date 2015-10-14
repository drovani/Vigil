using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core
{
    public abstract class Identity : IEquatable<Identity>
    {
        [Key]
        public Guid Id { get; protected set; }

        protected Identity()
        {
            Id = Guid.NewGuid();
        }

        protected Identity(Guid id)
        {
            this.Id = id;
        }

        /// <summary>Compares two Vigil.Data.Core.Identity classes for equality by Id.
        /// </summary>
        /// <param name="other">The Vigil.Data.Core.Identity class to compare Id values.</param>
        /// <returns>Returns true if the two Id values are equal; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (other.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Identity)other); 
        }

        public bool Equals(Identity other)
        {
            if (other == null)
            {
                return false;
            }
            return Guid.Equals(other.Id, this.Id);
        }

        public static bool operator ==(Identity left, Identity right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Identity left, Identity right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Id != Guid.Empty);
        }
    }
}
