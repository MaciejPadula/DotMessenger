using DotMessenger.Contract;
using Microsoft.Extensions.Hosting;

namespace DotMessenger.NetCore;

public abstract class MessengerBackgroundWorker<TMessage>(IMessenger messenger) : BackgroundService
    where TMessage : IMessage
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in messenger.MessageStream<TMessage>(stoppingToken))
        {
            await HandleEvent(@event, stoppingToken);
        }
    }

    protected abstract Task HandleEvent(TMessage message, CancellationToken cancellationToken = default);
}
