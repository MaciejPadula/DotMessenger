using Azure.Messaging.EventHubs;
using DotMessenger.AzureEventHub.Infrastructure;
using DotMessenger.Contract;

namespace DotMessenger.AzureEventHub.Logic;

internal interface IEventDataHelper
{
    EventData CreateEventData<TMessage>(TMessage message) where TMessage : IMessage;
    TMessage? ReadEventData<TMessage>(EventData eventData) where TMessage : IMessage;
}

internal class EventDataHelper(IEventHubDataSerializer eventHubDataSerializer) : IEventDataHelper
{
    public EventData CreateEventData<TMessage>(TMessage message) where TMessage : IMessage
    {
        return new EventData(
            new BinaryData(
                eventHubDataSerializer.Serialize(message)));
    }

    public TMessage? ReadEventData<TMessage>(EventData eventData) where TMessage : IMessage
    {
        return eventHubDataSerializer.Deserialize<TMessage>(eventData.EventBody.ToArray());
    }
}
