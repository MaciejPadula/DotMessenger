using Azure.Identity;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using DotMessenger.AzureEventHub.Configuration;
using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Infrastructure;
internal class CredentialsAzureEventHubClientFactory<TMessage>(AzureEventHubConfiguration<TMessage> configuration) : IAzureEventHubClientFactory<TMessage> where TMessage : IMessage
{
    public EventHubConsumerClient CreateConsumerClient() =>
        new(configuration.ConsumerGroup, configuration.FullyQualifiedNamespace, configuration.EventHubName, new DefaultAzureCredential());

    public EventHubProducerClient CreateProducerClient() =>
        new(configuration.FullyQualifiedNamespace, configuration.EventHubName, new DefaultAzureCredential());
}
