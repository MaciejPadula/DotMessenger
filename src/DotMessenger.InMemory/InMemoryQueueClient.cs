using System.Collections.Concurrent;
using DotMessenger.Logic;

namespace DotMessenger.InMemory;

internal class InMemoryQueueClient<TMessage>(InMemoryQueueConfiguration queueConfiguration) : QueueClientBase<TMessage>, IQueueClient<TMessage>
    where TMessage : IMessage
{
    private readonly TimeSpan StreamTimeout = queueConfiguration.MessagePoolingDelay;
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

    public override async IAsyncEnumerable<TMessage> MessageStream(CancellationToken cancellationToken)
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
                await Task.Delay(StreamTimeout, cancellationToken);
            }
        }
    }

    public override void Dispose() { }
}
