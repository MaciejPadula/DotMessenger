using DotMessenger;
using DotMessenger.AzureCore;
using DotMessenger.AzureEventHub;
using DotMessenger.InMemory;
using DotMessenger.NetCore;
using ExampleConsoleInMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json");
        config.AddEnvironmentVariables();
    })
    .ConfigureServices(s => s
        .AddMessenger()
        .AddInMemoryQueue<Message>(opt =>
        {
            opt.MessagePoolingDelay = TimeSpan.FromSeconds(1);
        })
        .AddAzureStorageQueue<AzMessage>(opt =>
        {
            opt.AzureConnectionType = AzureConnectionType.ConnectionString;
            opt.ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            opt.QueueName = "some-queue";
            opt.CreateQueueIfNotExists = true;
            opt.MessagePoolingDelay = TimeSpan.FromSeconds(1);
        })
        .AddAzureEventHubQueue<EventHubMessage>(opt =>
        {
            opt.AzureConnectionType = AzureConnectionType.ConnectionString;
            opt.ConnectionString = "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
            opt.EventHubName = "eh1";
            opt.ConsumerGroup = "cg1";
        })
        .AddMessengerHostedService<Message>((message, _) =>
        {
            Console.WriteLine($"Message from memory {message.Content}");
            return Task.CompletedTask;
        })
        .AddMessengerHostedService<AzMessage>((message, _) =>
        {
            Console.WriteLine($"Message from azure storage queue: {message.Body}");
            return Task.CompletedTask;
        })
        .AddMessengerHostedService<EventHubMessage>((message, _) =>
        {
            Console.WriteLine($"Message from azure eventhub: {message.SomeBody}");
            return Task.CompletedTask;
        })
        .AddHostedService<Messaging>());

var host = hostBuilder.Build();
await host.RunAsync();