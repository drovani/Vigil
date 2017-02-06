using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Buffers;

namespace Vigil.WebApi.Binders
{
    public class VigilMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ArrayPool<char> _charPool;
        private readonly ObjectPoolProvider _objectPoolProvider;

        public VigilMvcOptionsSetup(
            ILoggerFactory loggerFactory,
            IOptions<MvcJsonOptions> jsonOptions,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider)
        {
            if (jsonOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonOptions));
            }

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _jsonSerializerSettings = jsonOptions.Value.SerializerSettings;
            _charPool = charPool ?? throw new ArgumentNullException(nameof(charPool));
            _objectPoolProvider = objectPoolProvider ?? throw new ArgumentNullException(nameof(objectPoolProvider));
        }

        public void Configure(MvcOptions options)
        {
            options.InputFormatters.Insert(0, new CommandInputFormatter(
                _loggerFactory.CreateLogger<CommandInputFormatter>(),
                _jsonSerializerSettings,
                _charPool,
                _objectPoolProvider
            ));
        }
    }
}
