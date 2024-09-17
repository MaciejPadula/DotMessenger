using DotMessenger.AzureCore;
using DotMessenger.Contract;

namespace DotMessenger.AzureStorageQueue.Configuration;

public class AzureQueueConfiguration<TMessage>
    where TMessage : IMessage
{
    internal AzureQueueConfiguration() { }

    public AzureConnectionType AzureConnectionType { get; set; } = AzureConnectionType.Unknown;
    public string StorageAccountName { get; set; } = string.Empty;
    public string? FullQualifiedQueueWithStorageAccountUrl { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public bool CreateQueueIfNotExists { get; set; } = false;
    public TimeSpan MessagePoolingDelay { get; set; } = TimeSpan.FromSeconds(1);

    internal string ClientName => typeof(TMessage).Name;
}
