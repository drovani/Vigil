using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Vigil.Domain.Messaging;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Vigil.WebApi.Binders
{
    public class CommandInputFormatter : IInputFormatter
    {
        private static readonly Dictionary<Type, Func<string, DateTime, Command>> _commandModelCreators = new Dictionary<Type, Func<string, DateTime, Command>>();
        private static readonly string[] _allowedContentTypes = new string[] { "application/json", "text/json" };

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
                return _allowedContentTypes.Contains(context.HttpContext.Request.ContentType);
            }
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!_commandModelCreators.ContainsKey(context.ModelType))
            {
                var generatedBy = Expression.Parameter(typeof(string), nameof(Command.GeneratedBy));
                var generatedOn = Expression.Parameter(typeof(DateTime), nameof(Command.GeneratedOn));
                var constructor = context.ModelType.GetConstructor(new Type[] { typeof(string), typeof(DateTime) });
                var newExpr = Expression.New(constructor, generatedBy, generatedOn);
                var lambda = Expression.Lambda<Func<string, DateTime, Command>>(newExpr, generatedBy, generatedOn);

                _commandModelCreators.Add(context.ModelType, lambda.Compile());
            }

            var serializer = new JsonSerializer();
            object model = _commandModelCreators[context.ModelType](context?.HttpContext?.User?.Identity?.Name ?? "Anonymous User", DateTime.UtcNow);

            using (var sr = new StreamReader(context.HttpContext.Request.Body))
            using (var jsonTextRead = new JsonTextReader(sr))
            {
                serializer.Populate(jsonTextRead, model);
            }

            return InputFormatterResult.SuccessAsync(model);
        }
    }
}
