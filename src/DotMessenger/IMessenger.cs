namespace DotMessenger;

public interface IMessenger
{
    Task Push<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage;
    Task ReceiveMessages<TMessage>(Func<TMessage?, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where TMessage : IMessage;
}
