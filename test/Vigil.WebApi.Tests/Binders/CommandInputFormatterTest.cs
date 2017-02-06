using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Moq;
using Newtonsoft.Json;
using System.Buffers;
using System.IO;
using System.Text;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Xunit;

namespace Vigil.WebApi.Binders
{
    public class CommandInputFormatterTest
    {
        private static readonly ObjectPoolProvider _objectPoolProvider = new DefaultObjectPoolProvider();
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();
        private readonly CommandInputFormatter _formatter;

        public CommandInputFormatterTest()
        {
            var loggerMock = NullLogger.Instance;
            _formatter = new CommandInputFormatter(loggerMock, _serializerSettings, ArrayPool<char>.Shared, _objectPoolProvider);
        }

        [Theory]
        [InlineData("application/json", true)]
        [InlineData("application/*", false)]
        [InlineData("*/*", false)]
        [InlineData("text/json", true)]
        [InlineData("text/*", false)]
        [InlineData("text/xml", false)]
        [InlineData("application/xml", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("invalid", false)]
        public void CanRead_ReturnsTrueForAnySupportedContentType(string requestContentType, bool expectedCanRead)
        {
            // Arrange
            var loggerMock = NullLogger.Instance;

            var contentBytes = Encoding.UTF8.GetBytes("content");

            var httpContext = GetHttpContext(contentBytes, contentType: requestContentType);
            var provider = new EmptyModelMetadataProvider();
            var formatterContext = new InputFormatterContext(
                httpContext,
                modelName: "Irrelevant",
                modelState: new ModelStateDictionary(),
                metadata: provider.GetMetadataForType(typeof(Command)),
                readerFactory: new TestHttpRequestStreamReaderFactory().CreateReader);

            // Act
            var result = _formatter.CanRead(formatterContext);

            // Assert
            Assert.Equal(expectedCanRead, result);
        }

        [Theory]
        [InlineData("application/json")]
        [InlineData("text/json")]
        public void CanRead_WithModeTypeOfString_ReturnsFalse(string requestContentType)
        {
            // Arrange
            var loggerMock = NullLogger.Instance;

            var contentBytes = Encoding.UTF8.GetBytes("content");

            var httpContext = GetHttpContext(contentBytes, contentType: requestContentType);
            var provider = new EmptyModelMetadataProvider();
            var formatterContext = new InputFormatterContext(
                httpContext,
                modelName: "Irrelevant",
                modelState: new ModelStateDictionary(),
                metadata: provider.GetMetadataForType(typeof(string)),
                readerFactory: new TestHttpRequestStreamReaderFactory().CreateReader);

            // Act
            var result = _formatter.CanRead(formatterContext);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanRead_WithModeTypeOfCommand_ReturnsTrue()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                "Irrelevant",
                new ModelStateDictionary(),
                new EmptyModelMetadataProvider().GetMetadataForType(typeof(Command)),
                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 1;
            context.HttpContext.Request.ContentType = "application/json";

            bool result = _formatter.CanRead(context);

            Assert.True(result);
        }

        [Fact]
        public void CanRead_WithModeTypeAssignableFromCommand_ReturnsTrue()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                "Irrelevant",
                new ModelStateDictionary(),
                new EmptyModelMetadataProvider().GetMetadataForType(typeof(CreatePatron)),
                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 1;
            context.HttpContext.Request.ContentType = "application/json";

            bool result = _formatter.CanRead(context);

            Assert.True(result);
        }

        [Fact]
        public void CanRead_WithModeTypeOfCommand_AndContentTypeApplicationJson_ReturnsTrue()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                                "Irrelevant",
                                new ModelStateDictionary(),
                                new EmptyModelMetadataProvider().GetMetadataForType(typeof(Command)),
                                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 1;
            context.HttpContext.Request.ContentType = "application/json";

            bool result = _formatter.CanRead(context);

            Assert.True(result);
        }

        [Fact]
        public void CanRead_WithModeTypeOfCommand_AndContentTypeTextJson_ReturnsTrue()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                                "Irrelevant",
                                new ModelStateDictionary(),
                                new EmptyModelMetadataProvider().GetMetadataForType(typeof(Command)),
                                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 1;
            context.HttpContext.Request.ContentType = "text/json";

            bool result = _formatter.CanRead(context);

            Assert.True(result);
        }

        [Fact]
        public void CanRead_WithModeTypeOfCommand_AndContentTypeNull_ReturnsFalse()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                                "Irrelevant",
                                new ModelStateDictionary(),
                                new EmptyModelMetadataProvider().GetMetadataForType(typeof(Command)),
                                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 1;
            context.HttpContext.Request.ContentType = null;

            bool result = _formatter.CanRead(context);

            Assert.False(result);
        }

        private static HttpContext GetHttpContext(byte[] contentBytes, string contentType = "application/json")
        {
            var request = new Mock<HttpRequest>();
            var headers = new Mock<IHeaderDictionary>();
            request.SetupGet(r => r.Headers).Returns(headers.Object);
            request.SetupGet(f => f.Body).Returns(new MemoryStream(contentBytes));
            request.SetupGet(f => f.ContentType).Returns(contentType);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            return httpContext.Object;
        }
    }
}
