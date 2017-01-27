using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Vigil.Domain.Messaging;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Vigil.WebApi.Binders
{
    public class CommandInputFormatter : IInputFormatter
    {
        private Func<string, DateTime, Command> _commandModelCreator;

        public bool CanRead(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }else if (context.HttpContext.Request.ContentLength == 0)
            {
                return false;
            }
            else if (!typeof(Command).IsAssignableFrom(context.ModelType))
            {
                return false;
            }
            else
            {
                var contentType = context.HttpContext.Request.ContentType;
                var canRead = contentType == null || contentType == "application/json" || contentType == "text/json";

                return canRead;
            }
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (_commandModelCreator == null)
            {
                var generatedBy = Expression.Parameter(typeof(string), nameof(Command.GeneratedBy));
                var generatedOn = Expression.Parameter(typeof(DateTime), nameof(Command.GeneratedOn));
                var constructor = context.ModelType.GetConstructor(new Type[] { typeof(string), typeof(DateTime) });
                var newExpr = Expression.New(constructor, generatedBy, generatedOn);

                _commandModelCreator = Expression.Lambda<Func<string, DateTime, Command>>(newExpr, generatedBy, generatedOn)
                                                 .Compile();
            }

            var serializer = new JsonSerializer();
            object model = _commandModelCreator(context?.HttpContext?.User?.Identity?.Name ?? "Anonymous User", DateTime.UtcNow);

            using (var sr = new StreamReader(context.HttpContext.Request.Body))
            using (var jsonTextRead = new JsonTextReader(sr))
            {
                serializer.Populate(jsonTextRead, model);
            }

            return InputFormatterResult.SuccessAsync(model);
        }
    }
}
