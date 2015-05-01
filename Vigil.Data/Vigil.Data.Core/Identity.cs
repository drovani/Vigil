using System;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Data.Core
{
    public abstract class Identity : IEquatable<Identity>
    {
        [Key]
        public virtual Guid Id { get; protected set; }

        public bool Equals(Identity other)
        {
            return other != null && Guid.Equals(this.Id, other.Id);
        }
    }
}
