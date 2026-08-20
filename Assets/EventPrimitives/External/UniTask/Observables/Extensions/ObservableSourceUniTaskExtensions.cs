#if EVENTS_UNITASK_SUPPORT

using CatCode.EventPrimitives.Promises;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace CatCode.EventPrimitives
{
    public static class ObservableSourceUniTaskExtensions
    {
        public static UniTask WaitAsync(this IReadonlyObservableSource source, CancellationToken cancellationToken = default)
        {
            var taskSource = ObservableSourcePromise.Create(source, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }

        public static UniTask WaitAsync<T>(this IReadonlyObservableSource<T> source, ICondition<T> predicate, CancellationToken cancellationToken = default)
        {
            var taskSource = ObservableSourcePromise<T>.Create(source, predicate, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }

        public static async UniTask WaitAsync<T>(this IReadonlyObservableSource<T> value, Func<T, bool> predicate, CancellationToken cancellationToken = default)
        {
            using var handle = FuncConditionPool<T>.Get(predicate, out var condition);
            var taskSource = ObservableSourcePromise<T>.Create(value, condition, cancellationToken, out var token);
            await new UniTask(taskSource, token);
        }
    }
}
#endif