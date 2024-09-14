using DotMessenger.AzureStorageQueue;
using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.AzureStorageQueue.Infrastructure;
using DotMessenger.Contract;
using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace DotMessenger.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorageQueue<TMessage>(
        this IServiceCollection services,
        Action<AzureQueueConfiguration<TMessage>>? configFactory = null) where TMessage : IMessage
    {
        var config = new AzureQueueConfiguration<TMessage>();
        configFactory?.Invoke(config);

        services.AddSingleton(config);
        services.AddClientFactory(config);
        services.AddTransient<IQueueClientProvider<TMessage>, QueueClientProvider<TMessage>>();
        services.AddTransient<IQueueClient, AzureStorageQueueClient<TMessage>>();
        return services;
    }

    private static IServiceCollection AddClientFactory<TMessage>(
        this IServiceCollection services,
        AzureQueueConfiguration<TMessage> config) where TMessage : IMessage => config.AzureQueueConnectionType switch
    {
        AzureQueueConnectionType.ConnectionString => services.AddTransient<IQueueClientFactory<TMessage>, ConnectionStringQueueClientFactory<TMessage>>(),
        AzureQueueConnectionType.Identity => services.AddTransient<IQueueClientFactory<TMessage>, CredentialsQueueClientFactory<TMessage>>(),
        _ => throw new ArgumentOutOfRangeException(nameof(config.AzureQueueConnectionType))
    };
}
