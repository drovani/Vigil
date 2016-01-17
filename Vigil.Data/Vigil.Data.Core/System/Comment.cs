using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;

namespace Vigil.Data.Core.System
{
    public class Comment : KeyIdentity, ICreated, IModified, IDeleted, IEntityId
    {
        /// <summary>The object to which this comment is attached.
        /// </summary>
        public Guid EntityId { get; protected set; }
        [Required]
        public string CommentText { get; protected set; }

        protected Comment() { }

        protected Comment(Guid id) : base(id) { }

        public static Comment Create(Guid entityId, string commentText, string createdBy, DateTime createdOn)
        {
            Contract.Requires<ArgumentException>(entityId != Guid.Empty);
            Contract.Requires<ArgumentNullException>(commentText != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(commentText));
            Contract.Requires<ArgumentNullException>(createdBy != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentOutOfRangeException>(createdOn != default(DateTime));
            Contract.Ensures(Contract.Result<Comment>() != null);

            return new Comment
            {
                EntityId = entityId,
                CommentText = commentText.Trim(),
                CreatedBy = createdBy,
                CreatedOn = createdOn.ToUniversalTime(),
                Id = Guid.NewGuid()
            };
        }

        #region ICreated, IModified, IDeleted Implementation
        [Required]
        public string CreatedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedOn { get; protected set; }
        public string ModifiedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? ModifiedOn { get; protected set; }
        public string DeletedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DeletedOn { get; protected set; }
        #endregion

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(EntityId != Guid.Empty);
        }
    }
}
