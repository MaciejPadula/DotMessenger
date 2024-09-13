namespace DotMessenger.NetCore;

internal class MessengerFunctionBackgroundWorker<TMessage>(
    IMessenger messenger,
    Func<TMessage, CancellationToken, Task> handler) : MessengerBackgroundWorker<TMessage>(messenger) where TMessage : IMessage
{
    protected override async Task HandleEvent(TMessage message, CancellationToken cancellationToken = default)
    {
        await handler(message, cancellationToken);
    }
}
