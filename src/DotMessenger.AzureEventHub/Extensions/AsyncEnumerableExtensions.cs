using System.Runtime.CompilerServices;

namespace DotMessenger.AzureEventHub.Extensions;

internal static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<TResult> Merge<TResult>(
        this List<IAsyncEnumerator<TResult>> enumerators,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<(Task<bool> task, IAsyncEnumerator<TResult> enumerator)> enumeratorsInProgress = [];

        foreach (IAsyncEnumerator<TResult> enumerator in enumerators)
        {
            enumeratorsInProgress.Add((enumerator.MoveNextAsync().AsTask(), enumerator));
        }

        while (enumeratorsInProgress.Count != 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.WhenAny(enumeratorsInProgress.Select(item => item.task));

            var length = enumeratorsInProgress.Count - 1;
            for (int i = length; i >= 0; i--)
            {
                // Check for additional TaskStatus as needed
                if (enumeratorsInProgress[i].task.Status == TaskStatus.RanToCompletion)
                {
                    var enumeratorWithCompletedTask = enumeratorsInProgress[i];
                    enumeratorsInProgress.Remove(enumeratorWithCompletedTask);
                    if (enumeratorWithCompletedTask.task.Result)
                    {
                        yield return enumeratorWithCompletedTask.enumerator.Current;
                        var enumeratorInProgress = (enumeratorWithCompletedTask.enumerator.MoveNextAsync().AsTask(), enumeratorWithCompletedTask.enumerator);
                        enumeratorsInProgress.Insert(enumeratorsInProgress.Count, enumeratorInProgress);
                    }

                }
            }
        }
    }
}
