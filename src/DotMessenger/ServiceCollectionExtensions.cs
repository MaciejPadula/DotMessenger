using DotMessenger.Cofiguration;
using DotMessenger.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace DotMessenger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessenger(
        this IServiceCollection services,
        Action<MessengerConfiguration>? configure = null)
    {
        var config = new MessengerConfiguration();
        configure?.Invoke(config);

        services.AddSingleton(_ => config);
        services.AddTransient<IMessenger, DefaultMessenger>();
        return services;
    }
}
