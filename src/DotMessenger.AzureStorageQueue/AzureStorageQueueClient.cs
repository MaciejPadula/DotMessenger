using System.Text.Json;
using DotMessenger.Logic;

namespace DotMessenger.AzureStorageQueue;

internal class AzureStorageQueueClient<TMessage> : QueueClientBase<TMessage>, IQueueClient<TMessage>
    where TMessage : IMessage
{
    private readonly IQueueClientFactory _queueClientFactory;
    private readonly QueueConfiguration<TMessage> _configuration;

    public AzureStorageQueueClient(
        IQueueClientFactory queueClientFactory,
        QueueConfiguration<TMessage> configuration)
    {
        _queueClientFactory = queueClientFactory;
        _configuration = configuration;
    }

    public async override Task<TMessage?> Peek(CancellationToken cancellationToken)
    {
        var client = _queueClientFactory.GetQueueClient(_configuration);
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
        var client = _queueClientFactory.GetQueueClient(_configuration);
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
        var client = _queueClientFactory.GetQueueClient(_configuration);
        await client.SendMessageAsync(
            JsonSerializer.Serialize(message),
            cancellationToken);
    }
    public override void Dispose() { }
}
