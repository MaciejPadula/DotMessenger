using DotMessenger.AzureEventHub.Configuration;
using DotMessenger.AzureEventHub.Infrastructure;
using DotMessenger.AzureEventHub.Logic;
using DotMessenger.Contract;
using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotMessenger.AzureEventHub;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventHubQueue<TMessage>(
        this IServiceCollection services,
        Action<AzureEventHubConfiguration<TMessage>>? configFactory = null) where TMessage : IMessage
    {
        var config = new AzureEventHubConfiguration<TMessage>();
        configFactory?.Invoke(config);

        services.TryAddTransient<IEventHubDataSerializer, JsonEventHubDataSerializer>();
        services.TryAddTransient<IEventDataHelper, EventDataHelper>();

        services.AddSingleton(config);
        services.AddClientFactory(config);
        services.AddSingleton<IOffsetRepository<TMessage>, OffsetRepository<TMessage>>();
        services.AddTransient<IQueueClient, AzureEventHubQueueClient<TMessage>>();
        return services;
    }

    private static IServiceCollection AddClientFactory<TMessage>(
        this IServiceCollection services,
        AzureEventHubConfiguration<TMessage> config) where TMessage : IMessage => config.EventHubConnectionType switch
        {
            EventHubConnectionType.ConnectionString => services.AddTransient<IAzureEventHubClientFactory<TMessage>, ConnectionStringAzureEventHubClientFactory<TMessage>>(),
            EventHubConnectionType.Identity => services.AddTransient<IAzureEventHubClientFactory<TMessage>, CredentialsAzureEventHubClientFactory<TMessage>>(),
            _ => throw new ArgumentOutOfRangeException(nameof(config.EventHubConnectionType))
        };
}
