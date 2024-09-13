using Azure.Storage.Queues;
using DotMessenger.Contract;

namespace DotMessenger.AzureStorageQueue.Infrastructure;

internal interface IQueueClientFactory<TMessage> where TMessage : IMessage
{
    QueueClient GetQueueClient();
}
