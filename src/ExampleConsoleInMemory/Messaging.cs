using DotMessenger;
using Microsoft.Extensions.Hosting;

namespace ExampleConsoleInMemory;

internal class Messaging(IMessenger messenger, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messenger.Push(new Message("Hello, World!"), stoppingToken);
        await messenger.Push(new AzMessage("Hello, World!"), stoppingToken);
        await messenger.Push(new EventHubMessage("Hello World!"), stoppingToken);
        await Task.Delay(5000, stoppingToken);

        await messenger.Push(new Message("Goodbye, World!"), stoppingToken);
        await messenger.Push(new AzMessage("Goodbye, World!"), stoppingToken);
        await messenger.Push(new EventHubMessage("Goodbye, World!"), stoppingToken);
        await Task.Delay(5000, stoppingToken);

        hostApplicationLifetime.StopApplication();
    }
}
