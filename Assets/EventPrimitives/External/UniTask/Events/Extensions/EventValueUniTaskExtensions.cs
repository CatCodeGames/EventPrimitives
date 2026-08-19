#if EVENTS_UNITASK_SUPPORT

using CatCode.EventPrimitives.Promises;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace CatCode.EventPrimitives
{
    public static class EventValueUniTaskExtensions
    {
        public static UniTask WaitAsync<T>(this IReadOnlyEventValue<T> eventValue, ICondition<T> condition, bool checkInitialState = true, CancellationToken cancellationToken = default)
        {
            var taskSource = EventValuePromise<T>.Create(eventValue, condition, checkInitialState, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }

        public static async UniTask WaitAsync<T>(this IReadOnlyEventValue<T> eventValue, Func<T, bool> predicate, bool checkInitialState = true, CancellationToken cancellationToken = default)
        {
            using var handle = FuncConditionPool<T>.Get(predicate, out var condition);
            var taskSource = EventValuePromise<T>.Create(eventValue, condition, checkInitialState, cancellationToken, out var token);
            await new UniTask(taskSource, token);
        }

        public static UniTask WaitAsync<T>(this IReadOnlyEventValue<T> eventValue, CancellationToken cancellationToken)
        {
            var condition = TrueCondition<T>.Default;
            var taskSource = EventValuePromise<T>.Create(eventValue, condition, checkInitialState: false, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}

#endif