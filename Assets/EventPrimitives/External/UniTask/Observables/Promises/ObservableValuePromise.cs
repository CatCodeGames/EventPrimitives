#if EVENTS_UNITASK_SUPPORT

using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Pool;

namespace CatCode.EventPrimitives.Promises
{
    public sealed class ObservableValuePromise<T> : IUniTaskSource
    {
        private readonly static ObjectPool<ObservableValuePromise<T>> s_pool = new(() => new());

        static ObservableValuePromise()
        {
            TaskPool.RegisterSizeGetter(typeof(ObservableValuePromise<T>), () => s_pool.CountAll);
        }

        private readonly Action<T> _handler;
        private readonly Action _cancellationAction;

        private IReadOnlyObservableValue<T> _value;
        private ObservableSubscription _subscription;
        private ICondition<T> _predicate;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private UniTaskCompletionSourceCore<AsyncUnit> _core;

        public short Version
            => _core.Version;

        public ObservableValuePromise()
        {
            _handler = EventHandler;
            _cancellationAction = OnCancel;
        }

        private void Init(IReadOnlyObservableValue<T> value, ICondition<T> predicate, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _value = value;
            _predicate = predicate;

            _subscription = _value.Subscribe(_handler);

            _core.Reset();
            if (_cancellationToken.CanBeCanceled)
                _cancellationTokenRegistration = _cancellationToken.RegisterWithoutCaptureExecutionContext(_cancellationAction);
        }

        public static IUniTaskSource Create(IReadOnlyObservableValue<T> value, ICondition<T> predicate, bool checkInitialState, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
                return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);

            if (checkInitialState && predicate.Check(value.Value))
                return AutoResetUniTaskCompletionSource.CreateCompleted(out token);

            var promise = s_pool.Get();
            promise.Init(value, predicate, cancellationToken);

            TaskTracker.TrackActiveTask(promise, 3);

            token = promise.Version;
            return promise;
        }

        private void EventHandler(T value)
        {
            if (!_predicate.Check(value))
                return;

            _subscription.Unsubscribe();
            _cancellationTokenRegistration.Dispose();
            _core.TrySetResult(AsyncUnit.Default);
        }

        private void OnCancel()
        {
            _subscription.Unsubscribe();
            _cancellationTokenRegistration.Dispose();

            _core.TrySetCanceled(_cancellationToken);
        }

        public void GetResult(short token)
        {
            try
            {
                _core.GetResult(token);
            }
            finally
            {
                Release();
            }
        }

        public UniTaskStatus GetStatus(short token)
            => _core.GetStatus(token);

        public UniTaskStatus UnsafeGetStatus()
            => _core.UnsafeGetStatus();

        public void OnCompleted(Action<object> continuation, object state, short token)
            => _core.OnCompleted(continuation, state, token);

        private void Release()
        {
            TaskTracker.RemoveTracking(this);

            _value = null;
            _predicate = null;
            _cancellationToken = default;

            s_pool.Release(this);
        }
    }
}
#endif