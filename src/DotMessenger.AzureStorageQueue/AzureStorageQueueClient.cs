using System.Runtime.CompilerServices;
using System.Text.Json;
using DotMessenger.Logic;

namespace DotMessenger.AzureStorageQueue;

internal class AzureStorageQueueClient<TMessage>(
    IQueueClientFactory queueClientFactory,
    AzureQueueConfiguration<TMessage> configuration) : QueueClientBase<TMessage>, IQueueClient<TMessage>
    where TMessage : IMessage
{
    public async override Task<TMessage?> Peek(CancellationToken cancellationToken)
    {
        var client = queueClientFactory.GetQueueClient(configuration);
        var message = await client.ReceiveMessageAsync(
            cancellationToken: cancellationToken);

        if (message.Value?.MessageText is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TMessage>(message.Value.MessageText);
    }

    public override async Task<TMessage?> Pop(CancellationToken cancellationToken)
    {
        var client = queueClientFactory.GetQueueClient(configuration);
        var message = await client.ReceiveMessageAsync(
            cancellationToken: cancellationToken);

        if (message.Value?.MessageText is null)
        {
            return default;
        }

        var value = JsonSerializer.Deserialize<TMessage>(message.Value.MessageText);

        await client.DeleteMessageAsync(
            message.Value.MessageId,
            message.Value.PopReceipt,
            cancellationToken);

        return value;
    }

    public override async Task Push(TMessage message, CancellationToken cancellationToken)
    {
        var client = queueClientFactory.GetQueueClient(configuration);
        await client.SendMessageAsync(
            JsonSerializer.Serialize(message),
            cancellationToken);
    }

    public override async IAsyncEnumerable<TMessage> MessageStream([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await Pop(cancellationToken);
            if (result is not null)
            {
                yield return result;
            }
            else
            {
                await Task.Delay(configuration.MessagePoolingDelay, cancellationToken);
            }
        }

    }

    public override void Dispose() { }
}
