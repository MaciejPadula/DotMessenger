using DotMessenger;

namespace ExampleConsoleInMemory;

internal class Message : IMessage
{
    public string Content { get; set; } = string.Empty;
}
