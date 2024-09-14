using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.AzureStorageQueue.Infrastructure;
using DotMessenger.Contract;
using DotMessenger.Logic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DotMessenger.AzureStorageQueue;

internal class AzureStorageQueueClient<TMessage>(
    IQueueClientProvider<TMessage> queueClientFactory,
    AzureQueueConfiguration<TMessage> configuration) : IQueueClient<TMessage>
    where TMessage : IMessage
{
    public async Task<TMessage?> Peek(CancellationToken cancellationToken)
    {
        var client = await queueClientFactory.GetQueueClient();
        var message = await client.ReceiveMessageAsync(
            cancellationToken: cancellationToken);

        if (message.Value?.MessageText is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TMessage>(message.Value.MessageText);
    }

    public async Task<TMessage?> Pop(CancellationToken cancellationToken)
    {
        var client = await queueClientFactory.GetQueueClient();
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

    public async Task Push(TMessage message, CancellationToken cancellationToken)
    {
        var client = await queueClientFactory.GetQueueClient();
        await client.SendMessageAsync(
            JsonSerializer.Serialize(message),
            cancellationToken);
    }

    public async IAsyncEnumerable<TMessage> MessageStream([EnumeratorCancellation] CancellationToken cancellationToken)
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
}
