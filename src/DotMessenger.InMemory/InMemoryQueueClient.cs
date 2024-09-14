using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using DotMessenger.Contract;
using DotMessenger.Logic;

namespace DotMessenger.InMemory;

internal class InMemoryQueueClient<TMessage>(InMemoryQueueConfiguration<TMessage> queueConfiguration) : IQueueClient<TMessage>
    where TMessage : IMessage
{
    private readonly TimeSpan StreamTimeout = queueConfiguration.MessagePoolingDelay;
    private readonly ConcurrentQueue<TMessage> _queue = [];

    public Task<TMessage?> Peek(CancellationToken cancellationToken)
    {
        _queue.TryPeek(out var message);
        return Task.FromResult(message);
    }

    public Task<TMessage?> Pop(CancellationToken cancellationToken)
    {
        _queue.TryDequeue(out var result);
        return Task.FromResult(result);
    }

    public Task Push(TMessage message, CancellationToken cancellationToken)
    {
        _queue.Enqueue(message);
        return Task.CompletedTask;
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
                await Task.Delay(StreamTimeout, cancellationToken);
            }
        }
    }
}
