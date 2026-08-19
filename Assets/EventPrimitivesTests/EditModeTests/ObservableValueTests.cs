using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CatCode.EventPrimitives.Tests
{
    public sealed class ObservableValueTests
    {
        private sealed class TestSubscriber
        {
            private bool _notified;
            private ObservableSubscription _subscription;

            public TestSubscriber(ObservableSource<int> source)
                => Subscribe(source);

            public void Subscribe(ObservableSource<int> source)
                => _subscription = source.Subscribe(_ => _notified = true);

            public void Unsubscribe()
                => _subscription.Dispose();

            public void AssertNotified(bool expected)
            {
                Assert.AreEqual(expected, _notified);
                _notified = false;
            }
        }

        [Test]
        public void SubscribeUnsubscribeMultiple()
        {
            var source = ObservableSource<int>.CreateDefault();
            var subscribers = Enumerable
                .Range(0, 100)
                .Select(_ => new TestSubscriber(source))
                .ToList();

            for (int iteration = 0; iteration < 10; iteration++)
            {
                for (int i = subscribers.Count - 1; i > 0; i--)
                {
                    int j = UnityEngine.Random.Range(0, i + 1);
                    (subscribers[i], subscribers[j]) = (subscribers[j], subscribers[i]);
                }

                int removeCount = subscribers.Count / 2;

                for (int i = 0; i < removeCount; i++)
                    subscribers[i].Unsubscribe();

                source.Publish(iteration);

                for (int i = 0; i < removeCount; i++)
                    subscribers[i].AssertNotified(false);

                for (int i = removeCount; i < subscribers.Count; i++)
                    subscribers[i].AssertNotified(true);

                for (int i = 0; i < removeCount; i++)
                    subscribers[i].Subscribe(source);
            }

            subscribers.ForEach(x => x.Unsubscribe());
        }

        [Test]
        public void SubscribeUnsubscribe()
        {
            var source = ObservableValue<int>.CreateDefault(0, NotificationMode.OnChange);

            int calls = 0;
            int received = 0;

            var subscription = source.Subscribe(value =>
            {
                calls++;
                received = value;
            });

            source.Value = 123;

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            subscription.Dispose();

            source.Value = 456;

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            subscription = source.Subscribe(value =>
            {
                calls++;
                received = value;
            });

            source.Value = 789;

            Assert.AreEqual(2, calls);
            Assert.AreEqual(789, received);

            subscription.Dispose();
        }

        [Test]
        public void NotificationModeOnChange()
        {
            var source = ObservableValue<int>.CreateDefault(0, NotificationMode.OnChange);

            int calls = 0;
            int received = 0;

            var subscription = source.Subscribe(value =>
            {
                calls++;
                received = value;
            });

            source.Value = 123;

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            source.Value = 123;

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            source.Value = 789;

            Assert.AreEqual(2, calls);
            Assert.AreEqual(789, received);

            subscription.Dispose();
        }

        [Test]
        public void NotificationModeAlways()
        {
            var source = ObservableValue<int>.CreateDefault(0, NotificationMode.Always);

            int calls = 0;
            int received = 0;

            var subscription = source.Subscribe(value =>
            {
                calls++;
                received = value;
            });

            source.Value = 123;

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            source.Value = 123;

            Assert.AreEqual(2, calls);
            Assert.AreEqual(123, received);

            source.Value = 789;

            Assert.AreEqual(3, calls);
            Assert.AreEqual(789, received);

            subscription.Dispose();
        }

        [Test]
        public async Task WaitAsync()
        {
            var source = ObservableValue<int>.CreateDefault(0, NotificationMode.OnChange);

            var waitTask = WaitTask();
            var checkTask = CheckTask();

            await UniTask.WhenAll(waitTask, checkTask)
                .Timeout(TimeSpan.FromSeconds(1));

            async UniTask WaitTask()
            {
                await UniTask.Delay(100);
                source.Value = 123;
                await UniTask.Delay(100);
                source.Value = 456;
                await UniTask.Delay(100);
            }

            async UniTask CheckTask()
            {
                await source.WaitAsync(v => v == 0, true);
                Assert.AreEqual(0, source.Value);
                await source.WaitAsync(v => v == 456, true);
                Assert.AreEqual(456, source.Value);
            }
        }

        [Test]
        public async Task WaitAsync_Cancelled()
        {
            var source = ObservableValue<int>.CreateDefault(0, NotificationMode.OnChange);
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            try
            {
                await source.WaitAsync(v => v == 123, false, cts.Token);
                Assert.Fail("WaitAsync should have been cancelled.");
            }
            catch (OperationCanceledException)
            {
                // Expected.
            }
        }
    }
}