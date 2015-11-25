using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Identity;

namespace Vigil.Data.Core.System
{
    public class Comment : KeyIdentity, ICreated, IModified, IDeleted
    {
        public Guid EntityId { get; protected set; }
        public string CommentText { get; protected set; }

        public static Comment Create(Guid entityId, string commentText, VigilUser createdBy, DateTime createdOn)
        {
            Contract.Requires<ArgumentException>(entityId != Guid.Empty);
            Contract.Requires<ArgumentNullException>(commentText != null);
            Contract.Requires<ArgumentException>(commentText.Trim() != String.Empty);
            Contract.Requires<ArgumentNullException>(createdBy != null);
            Contract.Requires<ArgumentOutOfRangeException>(createdOn != default(DateTime));

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
        public VigilUser CreatedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedOn { get; protected set; }
        public VigilUser ModifiedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? ModifiedOn { get; protected set; }
        public VigilUser DeletedBy { get; protected set; }
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DeletedOn { get; protected set; }

        public bool MarkModified(VigilUser modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = modifiedOn;
            return true;
        }

        public bool MarkDeleted(VigilUser deletedBy, DateTime deletedOn)
        {
            if (DeletedBy == null && DeletedOn == null)
            {
                DeletedBy = deletedBy;
                DeletedOn = deletedOn;
                return true;
            }
            return false;
        }
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
