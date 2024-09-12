namespace DotMessenger.InMemory;

public class InMemoryQueueConfiguration
{
    internal InMemoryQueueConfiguration() { }

    public TimeSpan MessagePoolingDelay { get; set; } = TimeSpan.FromSeconds(1);
}
