using System;
using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core.Attributes
{
    public class DateTimeKindAttributeTests
    {
        [Fact]
        public void Constructor_Sets_Property()
        {
            DateTimeKindAttribute kind = new DateTimeKindAttribute(DateTimeKind.Utc);

            Assert.Equal(DateTimeKind.Utc, kind.Kind);
        }
    }
}
