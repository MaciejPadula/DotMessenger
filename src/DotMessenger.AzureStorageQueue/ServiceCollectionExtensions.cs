using Azure.Identity;
using DotMessenger.AzureCore;
using DotMessenger.AzureStorageQueue.Configuration;
using DotMessenger.AzureStorageQueue.Infrastructure;
using DotMessenger.AzureStorageQueue.Logic;
using DotMessenger.Contract;
using DotMessenger.Logic;
using Microsoft.Extensions.Azure;
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

        services.AddAzureClients(clientBuilder =>
        {
            if (config.AzureConnectionType == AzureConnectionType.ConnectionString)
            {
                clientBuilder
                    .AddQueueServiceClient(config.ConnectionString)
                    .WithName(config.ClientName);
            }
            else if (config.AzureConnectionType == AzureConnectionType.Identity)
            {
                clientBuilder
                    .AddQueueServiceClient(
                        new Uri(config.FullQualifiedQueueWithStorageAccountUrl ?? $"https://{config.StorageAccountName}.queue.core.windows.net/{config.QueueName}"))
                    .WithName(config.ClientName);

                clientBuilder.UseCredential(new DefaultAzureCredential());
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(config.AzureConnectionType));
            }
        });

        services.AddTransient<IQueueClientProvider<TMessage>, QueueClientProvider<TMessage>>();
        services.AddTransient<IQueueClient, AzureStorageQueueClient<TMessage>>();
        return services;
    }
}
