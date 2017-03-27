using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Vigil.Domain.Messaging;

namespace Vigil.Azure
{
    public class AzureCommandQueue : ICommandQueue
    {
        private CloudQueue commandQueue;

        public AzureCommandQueue(CloudQueue commandQueue)
        {
            this.commandQueue = commandQueue;
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
