using DotMessenger;
using DotMessenger.InMemory;
using DotMessenger.NetCore;
using ExampleConsoleInMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(s => s
        .AddMessenger()
        .AddInMemoryQueue<Message>()
        .AddAzureStorageQueue<AzMessage>(opt =>
        {
            opt.ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            opt.QueueName = "some-queue";
            opt.CreateQueueIfNotExists = true;
        })
        .AddMessengerBackgroundWorker<Message>((message, _) =>
        {
            Console.WriteLine($"Message from memory {message?.Content}");
            return Task.CompletedTask;
        })
        .AddMessengerBackgroundWorker<AzMessage>((message, _) =>
        {
            Console.WriteLine($"Message from azure: {message?.Body}");
            return Task.CompletedTask;
        })
        .AddHostedService<Messaging>());

var host = hostBuilder.Build();
await host.RunAsync();