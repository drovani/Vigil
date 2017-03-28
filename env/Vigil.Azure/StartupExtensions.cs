using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
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
                    .AddTransient(srvProvider =>
                        {
                            var storageCredentials = new StorageCredentials(
                                configuration["vigil-storage"],
                                configuration["vigil-storage-key1"]
                            );
                            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
                            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                            var commandQueue = queueClient.GetQueueReference(configuration["vigil-storage-queue"]);
                            return commandQueue;
                        });
                    
            return services;
        }
    }
}
