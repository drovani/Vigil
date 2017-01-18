using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Vigil.Domain.Messaging;
using System.Reflection;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using System.IO;
using System.Text;

namespace Vigil.WebApi.Binders
{
    public class CommandModelBinder : IModelBinder
    {
        private Func<string, DateTime, Command> _commandModelCreator;
        private readonly IList<IInputFormatter> _formatters;
        private readonly Func<Stream, Encoding, TextReader> _readerFactory;

        public CommandModelBinder(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            if (formatters == null)
            {
                throw new ArgumentNullException(nameof(formatters));
            }

            if (readerFactory == null)
            {
                throw new ArgumentNullException(nameof(readerFactory));
            }

            _formatters = formatters;
            _readerFactory = readerFactory.CreateReader;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            if (!typeof(Command).IsAssignableFrom(bindingContext.ModelType))
            {
                throw new InvalidOperationException("Vigil.WebApi.Binders.CommandModelBinder can only bind object inherited from Vigil.Domain.Messaging.Command");
            }

            if (_commandModelCreator == null)
            {
                var generatedBy = Expression.Parameter(typeof(string), nameof(Command.GeneratedBy));
                var generatedOn = Expression.Parameter(typeof(DateTime), nameof(Command.GeneratedOn));
                var constructor = bindingContext.ModelType.GetConstructor(new Type[] { typeof(string), typeof(DateTime) });
                var newExpr = Expression.New(constructor, generatedBy, generatedOn);

                _commandModelCreator = Expression.Lambda<Func<string, DateTime, Command>>(newExpr, generatedBy, generatedOn)
                                                 .Compile();
            }

            string modelBindingKey;
            if (bindingContext.IsTopLevelObject)
            {
                modelBindingKey = bindingContext.BinderModelName ?? string.Empty;
            }
            else
            {
                modelBindingKey = bindingContext.ModelName;
            }
            var httpContext = bindingContext.HttpContext;
            var formatterContext = new InputFormatterContext(httpContext, modelBindingKey, bindingContext.ModelState, bindingContext.ModelMetadata, _readerFactory);
            var formatter = (IInputFormatter)null;
            foreach (var _formatter in _formatters)
            {
                if (_formatter.CanRead(formatterContext))
                {
                    formatter = _formatter;
                    break;
                }
            }
            if (formatter == null)
            {
                string message = string.Empty; // Resources.FormatUnsupportedContentType(httpContext.Request.ContentType);

                var exception = new UnsupportedContentTypeException(message);
                bindingContext.ModelState.AddModelError(modelBindingKey, exception, bindingContext.ModelMetadata);
                return;
            }

            try
            {
                var previousCount = bindingContext.ModelState.ErrorCount;
                var result = await formatter.ReadAsync(formatterContext);
                var model = result.Model;

                if (result.HasError)
                {
                    return;
                }

                bindingContext.Result = ModelBindingResult.Success(model);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(modelBindingKey, ex, bindingContext.ModelMetadata);
            }


            //var result = _commandModelCreator(bindingContext?.HttpContext?.User?.Identity?.Name ?? "Test User", DateTime.Now.ToUniversalTime());
            //bindingContext.Result = ModelBindingResult.Success(result);

        }
    }
}
