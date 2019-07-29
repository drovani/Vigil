using Ardalis.GuardClauses;
using System;
using Xunit;

namespace Vigil.Framework.Tests
{
    public class GuardClauseExtensionsTest
    {
        [Fact]
        public void GuardAgainstNonUtcDateTimeKind_DoesNothing_GivenDateTimeKindUtc()
        {
            Guard.Against.NonUtcDateTimeKind(DateTime.UtcNow, "utcnow");
        }

        [Fact]
        public void GuardAgainstNonUtcDateTimeKind_Throws_Given_NonUtc()
        {
            Assert.Throws<ArgumentException>("unspecified", () => Guard.Against.NonUtcDateTimeKind(new DateTime(1981, 08, 25, 20, 42,36, DateTimeKind.Unspecified), "unspecified"));
            Assert.Throws<ArgumentException>("local", () => Guard.Against.NonUtcDateTimeKind(new DateTime(1981, 08, 25, 20, 42, 36, DateTimeKind.Local), "local"));
        }
    }
}
