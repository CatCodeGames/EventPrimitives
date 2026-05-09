using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Pool;

namespace CatCode.Events.Promises
{
    public sealed class EventSignalPromise : IUniTaskSource
    {
        private readonly static ObjectPool<EventSignalPromise> s_pool = new(() => new());

        static EventSignalPromise()
        {
            TaskPool.RegisterSizeGetter(typeof(EventSignalPromise), () => s_pool.CountAll);
        }

        private readonly Action _handler;
        private readonly Action _cancellationAction;

        private IReadOnlyEventSignal _eventSignal;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private UniTaskCompletionSourceCore<AsyncUnit> _core;

        public short Version
            => _core.Version;

        public EventSignalPromise()
        {
            _handler = EventHandler;
            _cancellationAction = OnCancel;
        }

        private void Init(IReadOnlyEventSignal eventSignal, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _eventSignal = eventSignal;

            _eventSignal.Raised += _handler;

            _core.Reset();
            if (_cancellationToken.CanBeCanceled)            
                _cancellationTokenRegistration = _cancellationToken.RegisterWithoutCaptureExecutionContext(_cancellationAction);            
        }

        public static IUniTaskSource Create(IReadOnlyEventSignal eventSignal, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
                return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            
            var promise = s_pool.Get();
            promise.Init(eventSignal, cancellationToken);

            TaskTracker.TrackActiveTask(promise, 3);

            token = promise.Version;
            return promise;
        }

        private void EventHandler()
        {
            _eventSignal.Raised -= _handler;
            _cancellationTokenRegistration.Dispose();
            _core.TrySetResult(AsyncUnit.Default);
        }

        private void OnCancel()
        {
            _eventSignal.Raised -= _handler;
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

            _eventSignal = null;
            _cancellationToken = default;

            s_pool.Release(this);
        }
    }
}