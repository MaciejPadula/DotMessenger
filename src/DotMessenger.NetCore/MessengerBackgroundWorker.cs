using Microsoft.Extensions.Hosting;

namespace DotMessenger.NetCore;

public class MessengerBackgroundWorker<TMessage> : IHostedService
    where TMessage : IMessage
{
    private readonly IMessenger _messenger;
    private readonly Func<TMessage?, CancellationToken, Task> _action;

    public MessengerBackgroundWorker(
        IMessenger messenger,
        Func<TMessage?, CancellationToken, Task> action)
    {
        _messenger = messenger;
        _action = action;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messenger.ReceiveMessages(_action, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
