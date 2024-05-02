using DotMessenger.Cofiguration;

namespace DotMessenger.Logic;

internal class DefaultMessenger : IMessenger
{
    private readonly IEnumerable<IQueueClient> _queueClients;
    private readonly MessengerConfiguration _config;

    public DefaultMessenger(
        IEnumerable<IQueueClient> queueClients,
        MessengerConfiguration config)
    {
        _queueClients = queueClients;
        _config = config;
    }

    public async Task ReceiveMessages<TMessage>(
        Func<TMessage, CancellationToken, Task> action,
        CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        using var client = GetClient<TMessage>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await client.Pop(cancellationToken);
            if (message is not null)
            {
                await action(message, cancellationToken);
            }
            await Task.Delay(_config.MessagePoolingDelay, cancellationToken);
        }
    }

    public async Task Push<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        using var client = GetClient<TMessage>();
        await client.Push(message, cancellationToken);
    }

    private IQueueClient<TMessage> GetClient<TMessage>() where TMessage : IMessage
    {
        var client = _queueClients.FirstOrDefault(c => c is IQueueClient<TMessage>);

        if (client is IQueueClient<TMessage> typedClient)
        {
            return typedClient;
        }

        throw new InvalidOperationException("Client not found");
    }
}
