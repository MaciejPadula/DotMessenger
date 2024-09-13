using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DotMessenger.AzureEventHub.Tests;

internal static class LoggerTestExtensions
{
    public static void LogError<T, TException>(this ILogger<T> logger) where T : class where TException : Exception
    {
        logger.Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<TException>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}