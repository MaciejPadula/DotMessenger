using DotMessenger.Contract;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace DotMessenger.AzureEventHub.Infrastructure;

internal interface IEventHubDataSerializer
{
    TMessage? Deserialize<TMessage>(byte[] bytes) where TMessage : IMessage;
    byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage;
}

internal class JsonEventHubDataSerializer(ILogger<JsonEventHubDataSerializer> logger) : IEventHubDataSerializer
{
    public TMessage? Deserialize<TMessage>(byte[] bytes) where TMessage : IMessage
    {
        var encodedData = Encoding.UTF8.GetString(bytes);
        try
        {
            return JsonSerializer.Deserialize<TMessage?>(encodedData);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize message: {Message}", encodedData);
            return default;
        }
    }

    public byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage
    {
        var encodedData = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(encodedData);
    }
}
