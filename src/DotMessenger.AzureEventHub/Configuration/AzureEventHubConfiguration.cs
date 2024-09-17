using DotMessenger.AzureCore;
using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Configuration;

public class AzureEventHubConfiguration<TMessage> where TMessage : IMessage
{
    internal AzureEventHubConfiguration() { }

    public AzureConnectionType AzureConnectionType { get; set; } = AzureConnectionType.Unknown;
    public string FullyQualifiedNamespace { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;

    internal string ClientName => typeof(TMessage).Name;
}
