using DotMessenger;
using Microsoft.Extensions.Hosting;

namespace ExampleConsoleInMemory;

internal class Messaging : IHostedService
{
    private readonly IMessenger _messenger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public Messaging(
        IMessenger messenger,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _messenger = messenger;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            await _messenger.Push(new Message { Content = "Hello, World!" }, cancellationToken);
            await _messenger.Push(new AzMessage { Body = "Hello, World!" }, cancellationToken);
            await Task.Delay(1000, cancellationToken);
            await _messenger.Push(new Message { Content = "Goodbye, World!" }, cancellationToken);
            await _messenger.Push(new AzMessage { Body = "Goodbye, World!" }, cancellationToken);
            await Task.Delay(5000, cancellationToken);

            _hostApplicationLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
