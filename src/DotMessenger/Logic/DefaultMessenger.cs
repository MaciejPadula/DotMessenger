using DotMessenger.Contract;

namespace DotMessenger.Logic;

internal class DefaultMessenger(IEnumerable<IQueueClient> queueClients) : IMessenger
{
    public async Task Push<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        var client = GetClient<TMessage>();
        await client.Push(message, cancellationToken);
    }

    public async Task<TMessage?> Pop<TMessage>(CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var client = GetClient<TMessage>();
        return await client.Pop(cancellationToken);
    }

    public IAsyncEnumerable<TMessage> MessageStream<TMessage>(CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var client = GetClient<TMessage>();
        return client.MessageStream(cancellationToken);
    }

    private IQueueClient<TMessage> GetClient<TMessage>() where TMessage : IMessage
    {
        var client = queueClients.FirstOrDefault(c => c is IQueueClient<TMessage>);

        if (client is IQueueClient<TMessage> typedClient)
        {
            return typedClient;
        }

        throw new InvalidOperationException("Client not found");
    }
}
