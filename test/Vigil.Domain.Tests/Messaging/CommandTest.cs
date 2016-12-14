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
        [Fact]
        public void Constructor_Throw_ArgumentNullException_When_GeneratedBy_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>("generatedBy", () => new TestCommand(string.Empty, DateTime.MinValue));
        }
        [Fact]
        public void Constructor_Throw_ArgumentOutOfRangeException_When_GeneratedOn_Is_DefaultDateTime()
        {
            Assert.Throws<ArgumentException>("generatedOn", () => new TestCommand("Create User", default(DateTime)));
        }
    }
}
