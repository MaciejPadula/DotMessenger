using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Infrastructure;

internal interface IAzureEventHubClientFactory<TMessage> where TMessage : IMessage
{
    EventHubProducerClient CreateProducerClient();
    EventHubConsumerClient CreateConsumerClient();
}
