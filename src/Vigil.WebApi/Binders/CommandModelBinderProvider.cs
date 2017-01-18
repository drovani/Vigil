using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;
using System.Reflection;
using Vigil.Domain.Messaging;

namespace Vigil.WebApi.Binders
{
    public class CommandModelBinderProvider : IModelBinderProvider
    {
        private readonly IList<IInputFormatter> _formatters;
        private readonly IHttpRequestStreamReaderFactory _readerFactory;
        private readonly BodyModelBinderProvider _defaultProvider;

        public CommandModelBinderProvider(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            _formatters = formatters;
            _readerFactory = readerFactory;
            _defaultProvider = new BodyModelBinderProvider(formatters, readerFactory);
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (typeof(Command).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new CommandModelBinder(_formatters, _readerFactory);
            }
            else
            {
                return _defaultProvider.GetBinder(context);
            }
        }
    }
}
