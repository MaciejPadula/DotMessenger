using Azure.Storage.Queues;

namespace DotMessenger.AzureStorageQueue;

internal interface IQueueClientFactory
{
    QueueClient GetQueueClient<TMessage>(AzureQueueConfiguration<TMessage> queueConfiguration)
        where TMessage : IMessage;
}

internal class QueueClientFactory : IQueueClientFactory
{
    public QueueClient GetQueueClient<TMessage>(AzureQueueConfiguration<TMessage> queueConfiguration)
        where TMessage : IMessage
    {
        var client = new QueueClient(
            queueConfiguration.ConnectionString,
            queueConfiguration.QueueName);

        if (queueConfiguration.CreateQueueIfNotExists)
        {
            client.CreateIfNotExists();
        }

        return client;
    }
}
