namespace DotMessenger.AzureStorageQueue;

public class QueueConfiguration<TMessage>
    where TMessage : IMessage
{
    internal QueueConfiguration() { }

    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public bool CreateQueueIfNotExists { get; set; } = false;
}
