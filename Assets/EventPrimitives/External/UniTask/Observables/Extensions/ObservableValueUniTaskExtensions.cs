using CatCode.EventPrimitives.Promises;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace CatCode.EventPrimitives
{
    public static class ObservableValueUniTaskExtensions
    {
        public static UniTask WaitAsync<T>(this IReadOnlyObservableValue<T> value, ICondition<T> condition, bool checkInitialState = true, CancellationToken cancellationToken = default)
        {
            var taskSource = ObservableValuePromise<T>.Create(value, condition, checkInitialState, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }

        public static async UniTask WaitAsync<T>(this IReadOnlyObservableValue<T> value, Func<T, bool> predicate, bool checkInitialState = true, CancellationToken cancellationToken = default)
        {
            using var handle = FuncConditionPool<T>.Get(predicate, out var condition);
            var taskSource = ObservableValuePromise<T>.Create(value, condition, checkInitialState, cancellationToken, out var token);
            await new UniTask(taskSource, token);
        }

        public static UniTask WaitAsync<T>(this IReadOnlyObservableValue<T> value, CancellationToken cancellationToken)
        {
            var condition = TrueCondition<T>.Default;
            var taskSource = ObservableValuePromise<T>.Create(value, condition, checkInitialState: false, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}