using DotMessenger.Contract;

namespace ExampleConsoleInMemory;

internal record EventHubMessage(string SomeBody) : IMessage;
