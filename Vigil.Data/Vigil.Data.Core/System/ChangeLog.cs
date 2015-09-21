using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Vigil.Data.Core.System
{
    public class ChangeLog : Identity, ICreated
    {
        [Required]
        public VigilUser CreatedBy { get; protected set; }
        public DateTime CreatedOn { get; protected set; }

        [Required]
        public Guid SourceId { get; protected set; }
        [Required]
        public string ModelName { get; protected set; }
        [Required]
        public string PropertyName { get; protected set; }
        public string OldValue { get; protected set; }
        public string NewValue { get; protected set; }

        private ChangeLog() : base() { }

        /// <summary>Creates a new ChangeLog object.
        /// </summary>
        /// <typeparam name="TSource">The Type of the object of which the logged field is a member.</typeparam>
        /// <typeparam name="TProperty">The property whose changing value is being logged.</typeparam>
        /// <param name="identifier">The primary identifier of the object being logged.</param>
        /// <param name="property">An expression resolving to a member whose changing value is being logged.</param>
        /// <param name="oldValue">The old value of the changing member.</param>
        /// <param name="newValue">The new value of the changing member.</param>
        /// <returns>A new ChangeLog instance.</returns>
        public static ChangeLog CreateLog<TSource, TProperty>(Guid identifier, Expression<Func<TSource, TProperty>> property, TProperty oldValue, TProperty newValue)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!identifier.Equals(Guid.Empty));
            Contract.Requires<ArgumentNullException>(property != null);
            Contract.Ensures(Contract.Result<ChangeLog>() != null);

            MemberExpression memExpr = property.Body as MemberExpression;
            if (memExpr == null || typeof(TSource).GetProperty(memExpr.Member.Name) == null)
            {
                throw new ArgumentException("Expression must be a MemberExpression with a property from TSource.");
            }

            return new ChangeLog()
            {
                SourceId = identifier,
                ModelName = typeof(TSource).Name,
                PropertyName = memExpr.Member.Name,
                OldValue = oldValue == null ? null : oldValue.ToString(),
                NewValue = newValue == null ? null : newValue.ToString()
            };
        }
        /// <summary>Creates a new ChangeLog object.
        /// </summary>
        /// <typeparam name="TSource">The Type of the object which inherits from <see cref="Vigil.Data.Core.Identity"/> of which the logged field is a member.</typeparam>
        /// <typeparam name="TProperty">The property whose changing value is being logged.</typeparam>
        /// <param name="source">The object whose field is being logged.</param>
        /// <param name="property">An expression resolving to a member whose changing value is being logged.</param>
        /// <param name="oldValue">The old value of the changing member.</param>
        /// <param name="newValue">The new value of the changing member.</param>
        /// <returns>A new ChangeLog instance.</returns>
        public static ChangeLog CreateLog<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> property, TProperty oldValue, TProperty newValue) where TSource : Identity
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(property != null);
            Contract.Ensures(Contract.Result<ChangeLog>() != null);

            return CreateLog<TSource, TProperty>(source.Id, property, oldValue, newValue);
        }
    }
}
