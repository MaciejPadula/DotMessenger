using DotMessenger.Contract;

namespace ExampleConsoleInMemory;

internal record AzMessage(string Body) : IMessage;
