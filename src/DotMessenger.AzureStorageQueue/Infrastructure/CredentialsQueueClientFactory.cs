using Azure.Identity;
using Azure.Storage.Queues;
using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.Contract;

namespace DotMessenger.AzureStorageQueue.Infrastructure;

internal class CredentialsQueueClientFactory<TMessage>(AzureQueueConfiguration<TMessage> queueConfiguration) : IQueueClientFactory<TMessage> where TMessage : IMessage
{
    public QueueClient GetQueueClient() =>
        new(new Uri(GetQueueUri(queueConfiguration)), new DefaultAzureCredential());

    private static string GetQueueUri(AzureQueueConfiguration<TMessage> queueConfiguration) =>
        queueConfiguration.FullQualifiedQueueWithStorageAccountUrl ?? $"https://{queueConfiguration.StorageAccountName}.queue.core.windows.net/{queueConfiguration.QueueName}";
}
