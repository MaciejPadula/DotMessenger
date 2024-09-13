using DotMessenger.Contract;
using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace DotMessenger.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryQueue<TMessage>(this IServiceCollection services, Action<InMemoryQueueConfiguration<TMessage>>? configFactory = null)
        where TMessage : IMessage
    {
        var config = new InMemoryQueueConfiguration<TMessage>();
        configFactory?.Invoke(config);

        services.AddSingleton(config);
        services.AddSingleton<IQueueClient, InMemoryQueueClient<TMessage>>();
        return services;
    }
}
