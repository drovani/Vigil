using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Vigil.Data.Core.System
{
    public class ChangeLog : IdentityCreatedBase, ICreated, IEntityId
    {
        [Required]
        public Guid EntityId { get; protected set; }
        [Required]
        public string ModelName { get; protected set; }
        [Required]
        public string PropertyName { get; protected set; }
        public string OldValue { get; protected set; }
        public string NewValue { get; protected set; }

        protected ChangeLog() : base() { }

        protected ChangeLog(string createdBy, DateTime createdOn, string modelName, string propertyName)
            : base(createdBy, createdOn)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(createdBy));
            Contract.Requires<ArgumentException>(createdOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(modelName));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(propertyName));

            ModelName = modelName.Trim();
            PropertyName = propertyName.Trim();
        }

        /// <summary>Creates a new ChangeLog object.
        /// </summary>
        /// <typeparam name="TSource">The Type of the object of which the logged field is a member.</typeparam>
        /// <typeparam name="TProperty">The property whose changing value is being logged.</typeparam>
        /// <param name="changedBy">The user that submitted the change to log.</param>
        /// <param name="changedOn">The date and time of the change to log.</param>
        /// <param name="identifier">The primary identifier of the object being logged.</param>
        /// <param name="property">An expression resolving to a member whose changing value is being logged.</param>
        /// <param name="oldValue">The old value of the changing member.</param>
        /// <param name="newValue">The new value of the changing member.</param>
        /// <returns>A new ChangeLog instance.</returns>
        public static ChangeLog CreateLog<TSource, TProperty>(string changedBy, DateTime changedOn, Guid identifier, Expression<Func<TSource, TProperty>> property, TProperty oldValue, TProperty newValue)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(changedBy));
            Contract.Requires<ArgumentException>(changedOn != default(DateTime));
            Contract.Requires<ArgumentException>(identifier != Guid.Empty);
            Contract.Requires<ArgumentNullException>(property != null);
            Contract.Ensures(Contract.Result<ChangeLog>() != null);

            MemberExpression memExpr = property.Body as MemberExpression;
            if (memExpr == null || typeof(TSource).GetProperty(memExpr.Member.Name) == null)
            {
                throw new ArgumentException("Expression must be a MemberExpression with a property from TSource.");
            }

            return new ChangeLog(changedBy, changedOn, typeof(TSource).Name, memExpr.Member.Name)
            {
                EntityId = identifier,
                OldValue = oldValue == null ? null : oldValue.ToString(),
                NewValue = newValue == null ? null : newValue.ToString()
            };
        }
        /// <summary>Creates a new ChangeLog object.
        /// </summary>
        /// <typeparam name="TSource">The Type of the object which inherits from <see cref="Vigil.Data.Core.KeyIdentity"/> of which the logged field is a member.</typeparam>
        /// <typeparam name="TProperty">The property whose changing value is being logged.</typeparam>
        /// <param name="changedBy">The user that submitted the change to log.</param>
        /// <param name="changedOn">The date and time of the change to log.</param>
        /// <param name="source">The object whose field is being logged.</param>
        /// <param name="property">An expression resolving to a member whose changing value is being logged.</param>
        /// <param name="oldValue">The old value of the changing member.</param>
        /// <param name="newValue">The new value of the changing member.</param>
        /// <returns>A new ChangeLog instance.</returns>
        public static ChangeLog CreateLog<TSource, TProperty>(string changedBy, DateTime changedOn, TSource source, Expression<Func<TSource, TProperty>> property, TProperty oldValue, TProperty newValue) where TSource : KeyIdentity
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(changedBy));
            Contract.Requires<ArgumentException>(changedOn != default(DateTime));
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentOutOfRangeException>(!source.Id.Equals(Guid.Empty));
            Contract.Requires<ArgumentNullException>(property != null);
            Contract.Ensures(Contract.Result<ChangeLog>() != null);

            return CreateLog(changedBy, changedOn, source.Id, property, oldValue, newValue);
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(ModelName));
            Contract.Invariant(!string.IsNullOrWhiteSpace(PropertyName));
        }

    }
}
