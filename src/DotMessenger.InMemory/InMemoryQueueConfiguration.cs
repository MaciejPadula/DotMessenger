using DotMessenger.Contract;

namespace DotMessenger.InMemory;

public class InMemoryQueueConfiguration<TMessage> where TMessage : IMessage
{
    internal InMemoryQueueConfiguration() { }

    public TimeSpan MessagePoolingDelay { get; set; } = TimeSpan.FromSeconds(1);
}
