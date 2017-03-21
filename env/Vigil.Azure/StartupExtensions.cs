using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Auth;
using Vigil.Azure;
using Vigil.Domain.Messaging;

namespace Vigil.WebApi.Configuration
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddVigilAzureServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddTransient<IEventBus, AzureEventBus>()
                    .AddTransient<ICommandQueue, AzureCommandQueue>()
                    .AddSingleton(srvProvider => new StorageCredentials(
                        configuration["vigil-storage"],
                        configuration["vigil-storage-key1"]
                    ));

            return services;
        }
    }
}
