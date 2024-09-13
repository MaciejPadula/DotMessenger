using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using DotMessenger.AzureEventHub.Configuration;
using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Infrastructure;

internal class ConnectionStringAzureEventHubClientFactory<TMessage>(AzureEventHubConfiguration<TMessage> configuration) : IAzureEventHubClientFactory<TMessage> where TMessage : IMessage
{
    public EventHubConsumerClient CreateConsumerClient() =>
        new(configuration.ConsumerGroup, configuration.ConnectionString, configuration.EventHubName);

    public EventHubProducerClient CreateProducerClient() =>
        new(configuration.ConnectionString, configuration.EventHubName);
}
