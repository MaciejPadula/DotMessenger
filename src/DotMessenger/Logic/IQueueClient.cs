namespace DotMessenger.Logic;

internal interface IQueueClient
{
    bool CanHandleMessage(IMessage message);
}

internal interface IQueueClient<T> : IDisposable, IQueueClient
    where T : IMessage
{
    Task<T?> Peek(CancellationToken cancellationToken);
    Task<T?> Pop(CancellationToken cancellationToken);
    Task Push(T message, CancellationToken cancellationToken);
}
