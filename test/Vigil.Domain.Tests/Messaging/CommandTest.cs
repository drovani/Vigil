using System;
using Xunit;

namespace Vigil.Domain.Messaging
{
    public class CommandTest
    {
        private class TestCommand : Command
        {
            public TestCommand(string generatedBy, DateTime generatedOn) : base(generatedBy, generatedOn) { }
        }

        [Fact]
        public void Constructor_Throw_ArgumentNullException_When_GeneratedBy_Is_Null()
        {
            Assert.Throws<ArgumentNullException>("generatedBy", () => new TestCommand(null, DateTime.MinValue));
        }
        [Fact(Skip = "Guard.Against.Default does not return the ParameterName")]
        public void Constructor_Throw_ArgumentNullException_When_GeneratedBy_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>("generatedBy", () => new TestCommand(string.Empty, DateTime.MinValue));
        }
        [Fact(Skip = "Guard.Against.Default does not return the ParameterName")]
        public void Constructor_Throw_ArgumentException_When_GeneratedOn_Is_DefaultDateTime()
        {
            Assert.Throws<ArgumentException>("generatedOnUtc", () => new TestCommand("Create User", default));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_GeneratedOn_Is_KindUnspecified()
        {
            var nonUtcNow = new DateTime(1981, 8, 26, 1, 17, 00, DateTimeKind.Unspecified);

            Assert.Throws<ArgumentException>("generatedOnUtc", () => new TestCommand("Create User", nonUtcNow));
        }
        [Fact]
        public void Constructor_Throw_ArgumentException_When_GeneratedOn_Is_KindLocal()
        {
            var nonUtcNow = new DateTime(1981, 8, 26, 1, 17, 00, DateTimeKind.Local);

            Assert.Throws<ArgumentException>("generatedOnUtc", () => new TestCommand("Create User", nonUtcNow));
        }
    }
}
