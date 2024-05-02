using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace DotMessenger.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryQueue<TMessage>(this IServiceCollection services)
        where TMessage : IMessage
    {
        services.AddSingleton<IQueueClient, InMemoryQueueClient<TMessage>>();
        return services;
    }
}
