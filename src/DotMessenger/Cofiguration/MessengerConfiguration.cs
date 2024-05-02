namespace DotMessenger.Cofiguration;

public class MessengerConfiguration
{
    internal MessengerConfiguration() { }

    public TimeSpan MessagePoolingDelay { get; set; } = TimeSpan.FromSeconds(1);
}
