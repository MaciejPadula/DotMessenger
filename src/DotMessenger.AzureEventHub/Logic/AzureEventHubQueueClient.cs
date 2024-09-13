using Azure.Messaging.EventHubs.Consumer;
using DotMessenger.AzureEventHub.Configuration;
using DotMessenger.AzureEventHub.Infrastructure;
using DotMessenger.Contract;
using DotMessenger.Logic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DotMessenger.AzureEventHub.Logic;

internal class AzureEventHubQueueClient<TMessage>(
    IAzureEventHubClientFactory<TMessage> azureEventHubClientFactory,
    IEventDataHelper eventDataHelper,
    AzureEventHubConfiguration<TMessage> configuration) : QueueClientBase<TMessage>, IQueueClient<TMessage> where TMessage : IMessage
{
    public override async IAsyncEnumerable<TMessage> MessageStream([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var client = azureEventHubClientFactory.CreateConsumerClient();
        await client.LoadOffsets<TMessage>(cancellationToken);

        await foreach (var partitionEvent in client.ReadEventsAsync(cancellationToken))
        {
            if (OffsetRepository<TMessage>.Offsets.TryGetValue(partitionEvent.Partition.PartitionId, out var offset)
                && partitionEvent.Data.Offset <= offset)
            {
                continue;
            }

            var deserializedEvent = eventDataHelper.ReadEventData<TMessage>(partitionEvent.Data);
            if (deserializedEvent is not null)
            {
                yield return deserializedEvent;
            }
            else
            {
                await Task.Delay(configuration.MessagePoolingDelay, cancellationToken);
            }
        }
    }

    public override Task<TMessage?> Peek(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public override async Task<TMessage?> Pop(CancellationToken cancellationToken)
    {
        await foreach (var item in MessageStream(cancellationToken))
        {
            return item;
        };

        return default;
    }

    public override async Task Push(TMessage message, CancellationToken cancellationToken)
    {
        await PushMultiple([message], cancellationToken);
    }

    private async Task PushMultiple(IEnumerable<TMessage> messages, CancellationToken cancellationToken)
    {
        await using var producer = azureEventHubClientFactory.CreateProducerClient();

        using var eventBatch = await producer.CreateBatchAsync(cancellationToken);

        foreach (var message in messages)
        {
            eventBatch.TryAdd(eventDataHelper.CreateEventData(message));
        }

        await producer.SendAsync(eventBatch, cancellationToken);
    }
}
