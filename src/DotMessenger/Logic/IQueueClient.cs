using DotMessenger.Contract;

namespace DotMessenger.Logic;

internal interface IQueueClient
{
    bool CanHandleMessage(IMessage message);
}

internal interface IQueueClient<TMessage> : IQueueClient
    where TMessage : IMessage
{
    Task<TMessage?> Peek(CancellationToken cancellationToken = default);
    Task<TMessage?> Pop(CancellationToken cancellationToken = default);
    Task Push(TMessage message, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TMessage> MessageStream(CancellationToken cancellationToken = default);
}
