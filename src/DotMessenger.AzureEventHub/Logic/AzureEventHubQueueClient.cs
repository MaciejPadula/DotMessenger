using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using DotMessenger.AzureEventHub.Configuration;
using DotMessenger.AzureEventHub.Extensions;
using DotMessenger.AzureEventHub.Infrastructure;
using DotMessenger.Contract;
using DotMessenger.Logic;
using Microsoft.Extensions.Azure;
using System.Runtime.CompilerServices;

namespace DotMessenger.AzureEventHub.Logic;

internal class AzureEventHubQueueClient<TMessage>(
    IAzureClientFactory<EventHubConsumerClient> consumerFactory,
    IAzureClientFactory<EventHubProducerClient> producerFactory,
    IEventDataHelper eventDataHelper,
    IOffsetRepository<TMessage> offsetRepository,
    AzureEventHubConfiguration<TMessage> configuration) : IQueueClient<TMessage> where TMessage : IMessage
{
    public async IAsyncEnumerable<TMessage> MessageStream([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = consumerFactory.CreateClient(configuration.ClientName);
        await offsetRepository.LoadOffsets(client, cancellationToken);

        var partitionIds = await client.GetPartitionIdsAsync(cancellationToken);

        var partitionReaders = partitionIds
            .Select(partitionId => client
                .ReadEventsFromPartitionAsync(
                    partitionId,
                    offsetRepository.GetPartitionPosition(partitionId),
                    cancellationToken))
            .Select(x => x.GetAsyncEnumerator())
            .ToList()
            .Merge(cancellationToken);

        await foreach (var partitionEvent in partitionReaders)
        {
            var deserializedEvent = eventDataHelper.ReadEventData<TMessage>(partitionEvent.Data);
            if (deserializedEvent is not null)
            {
                yield return deserializedEvent;
            }
        }
    }

    public Task<TMessage?> Peek(CancellationToken cancellationToken) => throw new NotImplementedException();

    public async Task<TMessage?> Pop(CancellationToken cancellationToken)
    {
        await foreach (var item in MessageStream(cancellationToken))
        {
            return item;
        };

        return default;
    }

    public async Task Push(TMessage message, CancellationToken cancellationToken)
    {
        await PushMultiple([message], cancellationToken);
    }

    private async Task PushMultiple(IEnumerable<TMessage> messages, CancellationToken cancellationToken)
    {
        var producer = producerFactory.CreateClient(configuration.ClientName);

        using var eventBatch = await producer.CreateBatchAsync(cancellationToken);

        foreach (var message in messages)
        {
            eventBatch.TryAdd(eventDataHelper.CreateEventData(message));
        }

        await producer.SendAsync(eventBatch, cancellationToken);
    }
}
