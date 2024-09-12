namespace DotMessenger;

public interface IMessenger
{
    Task Push<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    Task<TMessage?> Pop<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    IAsyncEnumerable<TMessage> MessageStream<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : IMessage;
}
