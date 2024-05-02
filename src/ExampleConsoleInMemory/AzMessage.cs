using DotMessenger;

namespace ExampleConsoleInMemory;

internal class AzMessage : IMessage
{
    public string Body { get; set; } = string.Empty;
}
