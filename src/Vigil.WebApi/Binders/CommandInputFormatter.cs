using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vigil.Domain.Messaging;

namespace Vigil.WebApi.Binders
{
    /// <summary>
    /// A <see cref="JsonInputFormatter"/> for <see cref="Command"/> parameters.
    /// </summary>
    public class CommandInputFormatter : JsonInputFormatter
    {
        private static readonly Dictionary<Type, Func<string, DateTime, Command>> _commandModelCreators = new Dictionary<Type, Func<string, DateTime, Command>>();
        private readonly IArrayPool<char> _charPool;
        private readonly ILogger _logger;
        private readonly ObjectPoolProvider _objectPoolProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonInputFormatter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="serializerSettings">
        /// The <see cref="JsonSerializerSettings"/>. Should be either the application-wide settings
        /// (<see cref="MvcJsonOptions.SerializerSettings"/>) or an instance
        /// <see cref="JsonSerializerSettingsProvider.CreateSerializerSettings"/> initially returned.
        /// </param>
        /// <param name="charPool">The <see cref="ArrayPool{Char}"/>.</param>
        /// <param name="objectPoolProvider">The <see cref="ObjectPoolProvider"/>.</param>
        public CommandInputFormatter(ILogger logger
            , JsonSerializerSettings serializerSettings
            , ArrayPool<char> charPool
            , ObjectPoolProvider objectPoolProvider)
            : base(logger, serializerSettings, charPool, objectPoolProvider)
        {
            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }
            _charPool = new JsonArrayPool<char>(charPool);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _objectPoolProvider = objectPoolProvider ?? throw new ArgumentNullException(nameof(objectPoolProvider));
        }

        /// <summary>
        /// Determines whether this <see cref="InputFormatter"/> can deserialize an object of the given
        /// <paramref name="type"/>.
        /// </summary>
        /// <remarks>The given <paramref name="type"/> must be assignable from <see cref="Command"/>.</remarks>
        /// <param name="type">The <see cref="Type"/> of object that will be read.</param>
        /// <returns><c>true</c> if the <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
        protected override bool CanReadType(Type type)
        {
            return base.CanReadType(type) && typeof(Command).IsAssignableFrom(type);
        }

        /// <summary>Reads an object from the request body.
        /// </summary>
        /// <remarks>Most of the code is the same between this and how <see cref="JsonInputFormatter"/> implements it.
        /// However, this uses a very specific constructor, and then utilizes the <see cref="JsonSerializer.Populate(JsonReader, object)"/>
        /// instead of <see cref="JsonSerializer.Deserialize(JsonReader, Type)"/>. If there is anything wrong with this method, first
        /// check to see if a fix was made in <see cref="JsonInputFormatter.ReadRequestBodyAsync(InputFormatterContext, Encoding)"/>.</remarks>
        /// <param name="context">The <see cref="InputFormatterContext"/>.</param>
        /// <returns>A <see cref="Task"/> that on completion deserializes the request body.</returns>
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }


            using (var streamReader = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    jsonReader.ArrayPool = _charPool;
                    jsonReader.CloseInput = false;

                    var successful = true;
                    EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> errorHandler = (sender, eventArgs) =>
                    {
                        successful = false;

                        var exception = eventArgs.ErrorContext.Error;

                        // Handle path combinations such as "" + "Property", "Parent" + "Property", or "Parent" + "[12]".
                        var key = eventArgs.ErrorContext.Path;
                        if (!string.IsNullOrEmpty(context.ModelName))
                        {
                            if (string.IsNullOrEmpty(eventArgs.ErrorContext.Path))
                            {
                                key = context.ModelName;
                            }
                            else if (eventArgs.ErrorContext.Path[0] == '[')
                            {
                                key = context.ModelName + eventArgs.ErrorContext.Path;
                            }
                            else
                            {
                                key = context.ModelName + "." + eventArgs.ErrorContext.Path;
                            }
                        }

                        var metadata = GetPathMetadata(context.Metadata, eventArgs.ErrorContext.Path);
                        context.ModelState.TryAddModelError(key, eventArgs.ErrorContext.Error, metadata);

                        _logger.LogDebug(1, eventArgs.ErrorContext.Error, "Command Input Formatter threw an exception");

                        // Error must always be marked as handled
                        // Failure to do so can cause the exception to be rethrown at every recursive level and
                        // overflow the stack for x64 CLR processes
                        eventArgs.ErrorContext.Handled = true;
                    };

                    var type = context.ModelType;
                    var jsonSerializer = CreateJsonSerializer();
                    jsonSerializer.Error += errorHandler;
                    object model = GetModel(context);
                    try
                    {
                        jsonSerializer.Populate(jsonReader, model);
                    }
                    finally
                    {
                        // Clean up the error handler since CreateJsonSerializer() pools instances.
                        jsonSerializer.Error -= errorHandler;
                        ReleaseJsonSerializer(jsonSerializer);
                    }

                    if (successful)
                    {
                        return InputFormatterResult.SuccessAsync(model);
                    }

                    return InputFormatterResult.FailureAsync();
                }
            }
        }

        protected virtual object GetModel(InputFormatterContext context)
        {
            if (!_commandModelCreators.ContainsKey(context.ModelType))
            {
                var generatedBy = Expression.Parameter(typeof(string), nameof(Command.GeneratedBy));
                var generatedOn = Expression.Parameter(typeof(DateTime), nameof(Command.GeneratedOn));
                var constructor = context.ModelType.GetConstructor(new Type[] { typeof(string), typeof(DateTime) });
                var newExpr = Expression.New(constructor, generatedBy, generatedOn);
                var lambda = Expression.Lambda<Func<string, DateTime, Command>>(newExpr, generatedBy, generatedOn);

                _commandModelCreators.Add(context.ModelType, lambda.Compile());
            }
            // @TODO Find a better way to get the current user's name
            return _commandModelCreators[context.ModelType](context?.HttpContext?.User?.Identity?.Name ?? "Anonymous User", DateTime.UtcNow);
        }

        private ModelMetadata GetPathMetadata(ModelMetadata metadata, string path)
        {
            var index = 0;
            while (index >= 0 && index < path.Length)
            {
                if (path[index] == '[')
                {
                    // At start of "[0]".
                    if (metadata.ElementMetadata == null)
                    {
                        // Odd case but don't throw just because ErrorContext had an odd-looking path.
                        break;
                    }

                    metadata = metadata.ElementMetadata;
                    index = path.IndexOf(']', index);
                }
                else if (path[index] == '.' || path[index] == ']')
                {
                    // Skip '.' in "prefix.property" or "[0].property" or ']' in "[0]".
                    index++;
                }
                else
                {
                    // At start of "property", "property." or "property[0]".
                    var endIndex = path.IndexOfAny(new[] { '.', '[' }, index);
                    if (endIndex == -1)
                    {
                        endIndex = path.Length;
                    }

                    var propertyName = path.Substring(index, endIndex - index);
                    if (metadata.Properties[propertyName] == null)
                    {
                        // Odd case but don't throw just because ErrorContext had an odd-looking path.
                        break;
                    }

                    metadata = metadata.Properties[propertyName];
                    index = endIndex;
                }
            }

            return metadata;
        }

    }
}
