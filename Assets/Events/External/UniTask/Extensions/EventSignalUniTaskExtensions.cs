#if EVENTS_UNITASK_SUPPORT

using CatCode.Events.Promises;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CatCode.Events
{
    public static class EventSignalUniTaskExtensions
    {
        public static UniTask WaitAsync(this IReadOnlyEventSignal eventSignal, CancellationToken cancellationToken)
        {
            var taskSource = EventSignalPromise.Create(eventSignal, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}

#endif