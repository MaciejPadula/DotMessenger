using Azure.Messaging.EventHubs.Consumer;
using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Infrastructure;

internal static class EventHubConsumerClientExtensions
{
    public static async Task LoadOffsets<TMessage>(this EventHubConsumerClient client, CancellationToken cancellationToken) where TMessage : IMessage
    {
        var partitionIds = await client.GetPartitionIdsAsync(cancellationToken);

        foreach (var partitionId in partitionIds)
        {
            var partition = await client.GetPartitionPropertiesAsync(partitionId, cancellationToken);
            OffsetRepository<TMessage>.Offsets[partition.Id] = partition.LastEnqueuedOffset;
        }
    }
}
