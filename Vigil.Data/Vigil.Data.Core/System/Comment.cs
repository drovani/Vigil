using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Vigil.Data.Core.System
{
    public class Comment : IdentityDeletedBase
    {
        /// <summary>The object to which this comment is attached.
        /// </summary>
        public Guid EntityId { get; protected set; }
        [Required]
        public string CommentText { get; protected set; }

        protected Comment() : base() { }

        protected Comment(string createdBy, DateTime createdOn, Guid entityId, string commentText) :base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentOutOfRangeException>(createdOn != default(DateTime));
        }

        public static Comment Create(string createdBy, DateTime createdOn, Guid entityId, string commentText)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentOutOfRangeException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentException>(entityId != Guid.Empty);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(commentText));
            Contract.Ensures(Contract.Result<Comment>() != null);

            return new Comment(createdBy, createdOn, entityId, commentText);
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(EntityId != Guid.Empty);
            Contract.Invariant(CommentText != null);
        }
    }
}
