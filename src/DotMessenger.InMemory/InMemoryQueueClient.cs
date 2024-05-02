using System.Collections.Concurrent;
using DotMessenger.Logic;

namespace DotMessenger.InMemory;

internal class InMemoryQueueClient<TMessage> : QueueClientBase<TMessage>, IQueueClient<TMessage>
    where TMessage : IMessage
{
    private readonly ConcurrentQueue<TMessage> _queue = [];

    public override Task<TMessage?> Peek(CancellationToken cancellationToken)
    {
        _queue.TryPeek(out var message);
        return Task.FromResult(message);
    }

    public override Task<TMessage?> Pop(CancellationToken cancellationToken)
    {
        _queue.TryDequeue(out var result);
        return Task.FromResult(result);
    }

    public override Task Push(TMessage message, CancellationToken cancellationToken)
    {
        _queue.Enqueue(message);
        return Task.CompletedTask;
    }

    public override void Dispose() { }
}
