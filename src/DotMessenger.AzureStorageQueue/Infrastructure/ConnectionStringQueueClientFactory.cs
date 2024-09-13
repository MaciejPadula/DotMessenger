using Azure.Storage.Queues;
using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.Contract;

namespace DotMessenger.AzureStorageQueue.Infrastructure;

internal class ConnectionStringQueueClientFactory<TMessage>(AzureQueueConfiguration<TMessage> queueConfiguration) : IQueueClientFactory<TMessage> where TMessage : IMessage
{
    public QueueClient GetQueueClient() =>
        new(queueConfiguration.ConnectionString, queueConfiguration.QueueName);
}
