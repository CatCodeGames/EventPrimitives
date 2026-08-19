#if EVENTS_UNITASK_SUPPORT

using CatCode.EventPrimitives.Promises;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace CatCode.EventPrimitives
{
    public static class EventSourceUniTaskExtensions
    {
        public static UniTask WaitAsync(this IReadOnlyEventSource source, CancellationToken cancellationToken = default)
        {
            var taskSource = EventSourcePromise.Create(source, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }

        public static UniTask WaitAsync<T>(this IReadOnlyEventSource<T> source, ICondition<T> condition, CancellationToken cancellationToken = default)
        {
            var taskSource = EventSourcePromise<T>.Create(source, condition, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }

        public static async UniTask WaitAsync<T>(this IReadOnlyEventSource<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default)
        {
            using var handle = FuncConditionPool<T>.Get(predicate, out var condition);
            var taskSource = EventSourcePromise<T>.Create(source, condition, cancellationToken, out var token);
            await new UniTask(taskSource, token);
        }

        public static UniTask WaitAsync<T>(this IReadOnlyEventSource<T> source, CancellationToken cancellationToken = default)
        {
            var condition = TrueCondition<T>.Default;
            var taskSource = EventSourcePromise<T>.Create(source, condition, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}

#endif