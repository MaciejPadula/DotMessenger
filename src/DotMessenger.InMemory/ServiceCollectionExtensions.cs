using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace DotMessenger.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryQueue<TMessage>(this IServiceCollection services, Action<InMemoryQueueConfiguration>? configFactory = null)
        where TMessage : IMessage
    {
        var config = new InMemoryQueueConfiguration();
        configFactory?.Invoke(config);

        services.AddSingleton<IQueueClient>(_ => new InMemoryQueueClient<TMessage>(config));
        return services;
    }
}
