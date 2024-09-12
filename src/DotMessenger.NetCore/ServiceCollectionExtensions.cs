using Microsoft.Extensions.DependencyInjection;

namespace DotMessenger.NetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessengerHostedService<TMessage>(
        this IServiceCollection services,
        Func<TMessage, CancellationToken, Task> action) where TMessage : IMessage
    {
        services.AddHostedService(sp =>
            new MessengerFunctionBackgroundWorker<TMessage>(
                sp.GetRequiredService<IMessenger>(),
                action));

        return services;
    }
}
