using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Vigil.WebApi.Binders
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddCommandFormatter(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            ServiceDescriptor descriptor = ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, VigilMvcOptionsSetup>();
            builder.Services.TryAddEnumerable(descriptor);
            return builder;
        }
    }
}
