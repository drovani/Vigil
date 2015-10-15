using Vigil.Data.Core.ValueObjects;
using Xunit;

namespace Vigil.Testing.Data.Core.ValueObjects
{
    public class DateAccuracyTests
    {
        [Fact]
        public void Constructor_Sets_Properties()
        {
            DateAccuracy dateAccuracy = new DateAccuracy('U', 'A', 'E');

            Assert.Equal("UAE", dateAccuracy.Accuracy);
            Assert.Equal('U', dateAccuracy.YearAccuracy);
            Assert.Equal('A', dateAccuracy.MonthAccuracy);
            Assert.Equal('E', dateAccuracy.DayAccuracy);
        }

        [Fact]
        public void CopyConstructor_Sets_Properties()
        {
            DateAccuracy originalAccuracy = new DateAccuracy('U', 'A', 'E');
            DateAccuracy dateAccuracy = new DateAccuracy(originalAccuracy);

            Assert.Equal("UAE", dateAccuracy.Accuracy);
            Assert.Equal('U', dateAccuracy.YearAccuracy);
            Assert.Equal('A', dateAccuracy.MonthAccuracy);
            Assert.Equal('E', dateAccuracy.DayAccuracy);
        }
    }
}
