using DotMessenger.AzureStorageQueue;
using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotMessenger.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorageQueue<TMessage>(
        this IServiceCollection services,
        Action<QueueConfiguration<TMessage>> configuration)
            where TMessage : IMessage
    {
        var config = new QueueConfiguration<TMessage>();
        configuration(config);

        services.AddSingleton(_ => config);
        services.TryAddTransient<IQueueClientFactory, QueueClientFactory>();
        services.AddTransient<IQueueClient, AzureStorageQueueClient<TMessage>>();
        return services;
    }
}
