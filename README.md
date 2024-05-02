# DotMessenger
.NET package that provides simple extension methods for configuring asynchronous message processing

# Configuration
To add new message handling you have to create message object. It have to implement IMessage interface:

```C#
using DotMessenger;

namespace ExampleConsoleInMemory;

internal class Message : IMessage
{
    public string Content { get; set; } = string.Empty;
}
```
## Registration
To configure messenger simply call AddMessenger method on DI container with or without options:

```C#
var serviceCollection = new ServiceCollection();
serviceCollection.AddMessenger(opt =>
{
    opt.MessagePoolingDelay = TimeSpan.FromSeconds(1)
});
```

## Using IMessenger
After that you will be able to use IMessenger interface to push and receive messages from queue system:
```C#
// Pushes message to queue
await _messenger.Push(new Message { Content = "Hello, World!" });

// Starts while loop that will execute provided function when new message is received
await _messenger.ReceiveMessages<Message>(
    async (receivedMessage, cancellationToken) =>
        Console.WriteLine($"Message from memory {receivedMessage?.Content}"));
```
## Queue Types
To configure queue for specific message you have to install package with desired implementation like InMemory or AzureStorageQueue. Then you can configure it via DI container:
```C#
serviceCollection.AddInMemoryQueue<Message>();

serviceCollection.AddAzureStorageQueue<AzMessage>(opt =>
{
    opt.ConnectionString = "...";
    opt.QueueName = "...";
    opt.CreateQueueIfNotExists = true;
});
```

## Preconfigured Hosted Service
When working with Hosting Environment like ASP.NET Core or Worker Service you can add predefined hosted service that will execute some function when new message is received from the queue:
```C#
serviceCollection.AddMessengerHostedService<Message>((message, _) =>
{
    Console.WriteLine($"Message from memory {message?.Content}");
    return Task.CompletedTask;
});
```
