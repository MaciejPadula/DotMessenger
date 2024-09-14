using Azure.Storage.Queues;
using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.Contract;
using Microsoft.Extensions.Azure;

namespace DotMessenger.AzureStorageQueue.Infrastructure;

internal interface IQueueClientProvider<TMessage> where TMessage : IMessage
{
    Task<QueueClient> GetQueueClient();
}

internal class QueueClientProvider<TMessage>(
    IAzureClientFactory<QueueServiceClient> queueClientFactory,
    AzureQueueConfiguration<TMessage> queueConfiguration) : IQueueClientProvider<TMessage> where TMessage : IMessage
{
    public async Task<QueueClient> GetQueueClient()
    {
        var serviceClient = queueClientFactory.CreateClient(queueConfiguration.ClientName);
        var client = serviceClient.GetQueueClient(queueConfiguration.QueueName);

        if (queueConfiguration.CreateQueueIfNotExists)
        {
            await client.CreateIfNotExistsAsync();
        }

        return client;
    }
}
