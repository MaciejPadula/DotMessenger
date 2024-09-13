using DotMessenger.Contract;
using System.Collections.Concurrent;

namespace DotMessenger.AzureEventHub.Infrastructure;

internal class OffsetRepository<TMessage> where TMessage : IMessage
{
    public static readonly ConcurrentDictionary<string, long> Offsets = [];
}
