using Azure.Storage.Queues;
using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.Contract;

namespace DotMessenger.AzureStorageQueue.Infrastructure;

internal interface IQueueClientProvider<TMessage> where TMessage : IMessage
{
    Task<QueueClient> GetQueueClient();
}

internal class QueueClientProvider<TMessage>(
    IQueueClientFactory<TMessage> queueClientFactory,
    AzureQueueConfiguration<TMessage> queueConfiguration) : IQueueClientProvider<TMessage> where TMessage : IMessage
{
    public async Task<QueueClient> GetQueueClient()
    {
        var client = queueClientFactory.GetQueueClient();

        if (queueConfiguration.CreateQueueIfNotExists)
        {
            await client.CreateIfNotExistsAsync();
        }

        return client;
    }
}
