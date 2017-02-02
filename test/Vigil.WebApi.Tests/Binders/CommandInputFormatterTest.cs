using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System;
using System.IO;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Xunit;

namespace Vigil.WebApi.Binders
{
    public class CommandInputFormatterTest
    {
        private readonly CommandInputFormatter _formatter = new CommandInputFormatter();

        [Fact]
        public void CanRead_WithNullContext_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _formatter.CanRead(null));
        }

        [Fact]
        public void CanRead_WithEmptyRequest_ReturnsFalse()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                "Irrelevant",
                new ModelStateDictionary(),
                new EmptyModelMetadataProvider().GetMetadataForType(typeof(Command)),
                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 0;
            context.HttpContext.Request.ContentType = "application/json";

            bool result = _formatter.CanRead(context);

            Assert.False(result);
        }

        [Fact]
        public void CanRead_WithModeTypeNotOfCommand_ReturnsFalse()
        {
            var context = new InputFormatterContext(new DefaultHttpContext(),
                "Irrelevant",
                new ModelStateDictionary(),
                new EmptyModelMetadataProvider().GetMetadataForType(typeof(object)),
                (stream, encoding) => new StreamReader(stream, encoding));

            context.HttpContext.Request.ContentLength = 1;
            context.HttpContext.Request.ContentType = "application/json";

            bool result = _formatter.CanRead(context);

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
    }
}
