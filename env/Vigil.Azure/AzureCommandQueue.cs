using System;
using Vigil.Domain.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vigil.Azure
{
    public class AzureCommandQueue : ICommandQueue
    {
        private CloudQueue commandQueue;

        public AzureCommandQueue(StorageCredentials storageCredentials)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            commandQueue = queueClient.GetQueueReference("commandqueue");
        }

        public void Publish<TCommand>(TCommand command) where TCommand : ICommand
        {
            PublishAsync(command).Wait();
        }

        public async Task PublishAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            await commandQueue.CreateIfNotExistsAsync();

            var newCmd = new Command(command)
            {
                DispatchedOn = DateTime.UtcNow
            };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(newCmd));
            await commandQueue.AddMessageAsync(message);
        }
    }
}
