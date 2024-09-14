using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Configuration;

public class AzureEventHubConfiguration<TMessage> where TMessage : IMessage
{
    internal AzureEventHubConfiguration() { }

    public EventHubConnectionType EventHubConnectionType { get; set; } = EventHubConnectionType.Unknown;
    public string FullyQualifiedNamespace { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
    public HashSet<string> PartitionIdsToConnect { get; set; } = [];
    public TimeSpan MessagePoolingDelay { get; set; } = TimeSpan.FromSeconds(1);
}
