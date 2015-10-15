using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vigil.Data.Core.ValueObjects
{
    [ComplexType]
    public class DateAccuracy : ValueObject<DateAccuracy>
    {
        /// <summary>Accuracy of the Date, with the first character representing Year, the second representing Month, and the third representing Day
        /// <remarks>'A' means Accurate, 'E' means Estimate, 'U' means Unknown
        /// </remarks>
        /// </summary>
        [DefaultValue("AAA")]
        [Column(TypeName = "char")]
        [StringLength(3)]
        public string Accuracy { get; protected set; }

        /// <summary>Accuracy of the Year component.
        /// <remarks>'A' means Accurate, 'E' means Estimate, 'U' means Unknown
        /// </remarks>
        /// </summary>
        [DefaultValue('A')]
        public char YearAccuracy { get { return Accuracy[0]; } }
        /// <summary>Accuracy of the Month component.
        /// <remarks>'A' means Accurate, 'E' means Estimate, 'U' means Unknown
        /// </remarks>
        /// </summary>
        [DefaultValue('A')]
        public char MonthAccuracy { get { return Accuracy[1]; } }
        /// <summary>Accuracy of the Day component.
        /// <remarks>'A' means Accurate, 'E' means Estimate, 'U' means Unknown
        /// </remarks>
        /// </summary>
        [DefaultValue('A')]
        public char DayAccuracy { get { return Accuracy[2]; } }

        private DateAccuracy()
            : base()
        {
            Accuracy = "AAA";
        }

        public DateAccuracy(char yearAccuracy, char monthAccuracy, char dayAccuracy)
        {
            Contract.Requires<ArgumentOutOfRangeException>(yearAccuracy == 'A' || yearAccuracy == 'E' || yearAccuracy == 'U');
            Contract.Requires<ArgumentOutOfRangeException>(monthAccuracy == 'A' || monthAccuracy == 'E' || monthAccuracy == 'U');
            Contract.Requires<ArgumentOutOfRangeException>(dayAccuracy == 'A' || dayAccuracy == 'E' || dayAccuracy == 'U');

            string accuracy = String.Concat(yearAccuracy, monthAccuracy, dayAccuracy);
            Contract.Assume(accuracy.Length == 3);
            Accuracy = accuracy;
        }
        public DateAccuracy(DateAccuracy accuracy)
            : this(accuracy.YearAccuracy, accuracy.MonthAccuracy, accuracy.DayAccuracy)
        {
            Contract.Requires<ArgumentNullException>(accuracy != null);
        }

        [ContractInvariantMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Accuracy.Length == 3);
        }
    }
}
