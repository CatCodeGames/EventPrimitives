#if EVENTS_UNITASK_SUPPORT

using CatCode.Events.Promises;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CatCode.Events
{
    public static class EventSourceUniTaskExtensions
    {
        public static UniTask WaitAsync(this IReadOnlyEventSource eventSource, CancellationToken cancellationToken)
        {
            var taskSource = EventSourcePromise.Create(eventSource, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}

#endif