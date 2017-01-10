using System;
using Xunit;

namespace Vigil.Domain.Messaging
{
    public class EventTest
    {
        private class TestEvent : Event
        {
            public TestEvent(string generatedBy, DateTime generatedOn, Guid sourceId) : base(generatedBy, generatedOn, sourceId) { }
        }

        [Fact]
        public void Constructor_Throw_ArgumentNullException_When_GeneratedBy_Is_Null()
        {
            Assert.Throws<ArgumentNullException>("generatedBy", () => new TestEvent(null, default(DateTime), Guid.Empty));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_GeneratedBy_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>("generatedBy", () => new TestEvent(string.Empty, default(DateTime), Guid.Empty));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_GeneratedOn_Is_DefaultDateTime()
        {
            Assert.Throws<ArgumentException>("generatedOnUtc", () => new TestEvent("Create User", default(DateTime), Guid.Empty));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_GeneratedOn_Is_KindUnspecified()
        {
            var nonUtcNow = new DateTime(1981, 8, 26, 1, 17, 00, DateTimeKind.Unspecified);

            Assert.Throws<ArgumentException>("generatedOnUtc", () => new TestEvent("Create User", nonUtcNow, Guid.Empty));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_GeneratedOn_Is_KindLocal()
        {
            var nonUtcNow = new DateTime(1981, 8, 26, 1, 17, 00, DateTimeKind.Local);

            Assert.Throws<ArgumentException>("generatedOnUtc", () => new TestEvent("Create User", nonUtcNow, Guid.Empty));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_SourceId_Is_DefaultGuid()
        {
            Assert.Throws<ArgumentException>("sourceId", () => new TestEvent("Create User", TestHelper.Now, Guid.Empty));
        }
    }
}
