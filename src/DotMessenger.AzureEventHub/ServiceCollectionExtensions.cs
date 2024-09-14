using Azure.Identity;
using DotMessenger.AzureCore;
using DotMessenger.AzureEventHub.Configuration;
using DotMessenger.AzureEventHub.Infrastructure;
using DotMessenger.AzureEventHub.Logic;
using DotMessenger.Contract;
using DotMessenger.Logic;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotMessenger.AzureEventHub;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureEventHubQueue<TMessage>(
        this IServiceCollection services,
        Action<AzureEventHubConfiguration<TMessage>>? configFactory = null) where TMessage : IMessage
    {
        var config = new AzureEventHubConfiguration<TMessage>();
        configFactory?.Invoke(config);

        services.TryAddTransient<IEventHubDataSerializer, JsonEventHubDataSerializer>();
        services.TryAddTransient<IEventDataHelper, EventDataHelper>();

        services.AddSingleton(config);

        services.AddAzureClients(clientBuilder =>
        {
            if (config.AzureConnectionType == AzureConnectionType.ConnectionString)
            {
                clientBuilder
                    .AddEventHubConsumerClient(config.ConsumerGroup, config.ConnectionString, config.EventHubName)
                    .WithName(config.ClientName);

                clientBuilder
                    .AddEventHubProducerClient(config.ConnectionString, config.EventHubName)
                    .WithName(config.ClientName);
            }
            else if (config.AzureConnectionType == AzureConnectionType.Identity)
            {
                clientBuilder
                    .AddEventHubConsumerClientWithNamespace(config.ConsumerGroup, config.FullyQualifiedNamespace, config.EventHubName)
                    .WithName(config.ClientName);

                clientBuilder
                   .AddEventHubProducerClientWithNamespace(config.FullyQualifiedNamespace, config.EventHubName)
                   .WithName(config.ClientName);

                clientBuilder.UseCredential(new DefaultAzureCredential());
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(config.AzureConnectionType));
            }
        });

        services.AddSingleton<IOffsetRepository<TMessage>, OffsetRepository<TMessage>>();
        services.AddTransient<IQueueClient, AzureEventHubQueueClient<TMessage>>();
        return services;
    }
}
