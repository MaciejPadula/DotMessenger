namespace DotMessenger.AzureStorageQueue;

public class AzureQueueConfiguration<TMessage>
    where TMessage : IMessage
{
    internal AzureQueueConfiguration() { }

    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public bool CreateQueueIfNotExists { get; set; } = false;
    public TimeSpan MessagePoolingDelay { get; set; } = TimeSpan.FromSeconds(1);
}
