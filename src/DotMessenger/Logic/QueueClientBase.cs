namespace DotMessenger.Logic;

internal abstract class QueueClientBase<TMessage> : IQueueClient<TMessage>
    where TMessage : IMessage
{
    public bool CanHandleMessage(IMessage message) =>
        message is TMessage;

    public abstract void Dispose();

    public abstract Task<TMessage?> Peek(CancellationToken cancellationToken);

    public abstract Task<TMessage?> Pop(CancellationToken cancellationToken);

    public abstract Task Push(TMessage message, CancellationToken cancellationToken);
}
