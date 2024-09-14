using Azure.Messaging.EventHubs.Consumer;
using DotMessenger.Contract;
using System.Collections.Concurrent;

namespace DotMessenger.AzureEventHub.Infrastructure;

internal interface IOffsetRepository<TMessage>
{
    Task LoadOffsets(EventHubConsumerClient consumerClient, CancellationToken cancellationToken);
    EventPosition GetPartitionPosition(string partitionId);
}

internal class OffsetRepository<TMessage> : IOffsetRepository<TMessage> where TMessage : IMessage
{
    private readonly ConcurrentDictionary<string, long> _offsets = [];

    public EventPosition GetPartitionPosition(string partitionId)
    {
        var offset = _offsets.TryGetValue(partitionId, out var value) ? value : 0;
        var shouldIncludeLastOffsetedElement = offset != 0;
        return EventPosition.FromOffset(
            offset,
            shouldIncludeLastOffsetedElement);
    }

    public async Task LoadOffsets(EventHubConsumerClient consumerClient, CancellationToken cancellationToken)
    {
        var partitionIds = await consumerClient.GetPartitionIdsAsync(cancellationToken);

        foreach (var partitionId in partitionIds)
        {
            var partition = await consumerClient.GetPartitionPropertiesAsync(partitionId, cancellationToken);
            _offsets[partition.Id] = partition.LastEnqueuedOffset;
        }
    }
}
