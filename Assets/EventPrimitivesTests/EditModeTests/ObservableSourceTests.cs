using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatCode.EventPrimitives.Tests
{
    public sealed class ObservableSourceTests
    {
        [Test]
        public void SubscribeUnsubscribe()
        {
            var source = ObservableSource.CreateDefault();

            int calls = 0;

            var subscription = source.Subscribe(() => calls++);

            source.Publish();
            Assert.AreEqual(1, calls);

            subscription.Dispose();

            source.Publish();
            Assert.AreEqual(1, calls);

            subscription = source.Subscribe(() => calls++);

            source.Publish();
            Assert.AreEqual(2, calls);

            subscription.Dispose();
        }

        [Test]
        public async Task WaitAsync()
        {
            var source = ObservableSource.CreateDefault();

            var publishTask = Publish();
            var checkTask = Check();

            await UniTask.WhenAll(publishTask, checkTask)
                .Timeout(TimeSpan.FromSeconds(1))
                .AsTask();

            async UniTask Publish()
            {
                await UniTask.Delay(100);
                source.Publish();
            }

            async UniTask Check()
            {
                await source.WaitAsync();
            }
        }

        [Test]
        public async Task WaitAsync_Cancelled()
        {
            var source = ObservableSource.CreateDefault();
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            try
            {
                await source.WaitAsync(cts.Token);
                Assert.Fail("WaitAsync should have been cancelled.");
            }
            catch (OperationCanceledException)
            {
                // Expected.
            }
        }


        [Test]
        public void Generic_SubscribeUnsubscribe()
        {
            var source = ObservableSource<int>.CreateDefault();

            int calls = 0;
            int received = 0;

            var subscription = source.Subscribe(value =>
            {
                calls++;
                received = value;
            });

            source.Publish(123);

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            subscription.Dispose();

            source.Publish(456);

            Assert.AreEqual(1, calls);
            Assert.AreEqual(123, received);

            subscription = source.Subscribe(value =>
            {
                calls++;
                received = value;
            });

            source.Publish(789);

            Assert.AreEqual(2, calls);
            Assert.AreEqual(789, received);

            subscription.Dispose();
        }

        [Test]
        public async Task Generic_WaitAsync()
        {
            var source = ObservableSource<int>.CreateDefault();

            var publishTask = Publish();
            var checkTask = Check();

            await UniTask.WhenAll(publishTask, checkTask)
                .Timeout(TimeSpan.FromSeconds(1));

            async UniTask Publish()
            {
                await UniTask.Delay(100);
                source.Publish(123);

                await UniTask.Delay(100);
                source.Publish(456);
            }

            async UniTask Check()
            {
                await source.WaitAsync(v => v == 123);
                await source.WaitAsync(v => v == 456);
            }
        }
        [Test]
        public async Task Generic_WaitAsync_Cancelled()
        {
            var source = ObservableSource<int>.CreateDefault();
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            try
            {
                await source.WaitAsync(v => v == 123, cts.Token);
                Assert.Fail("WaitAsync should have been cancelled.");
            }
            catch (OperationCanceledException)
            {
                // Expected.
            }
        }
        [Test]
        public async Task Generic_WaitAsync_CompletesWhenConditionMet()
        {
            var source = ObservableSource<int>.CreateDefault();

            var publishTask = Publish();
            var waitTask = Wait();

            await UniTask.WhenAll(publishTask, waitTask)
                .Timeout(TimeSpan.FromSeconds(1));

            async UniTask Publish()
            {
                await UniTask.Delay(100);
                source.Publish(123);

                await UniTask.Delay(100);
                source.Publish(456);
            }

            async UniTask Wait()
            {
                await source.WaitAsync(value => value == 456);
            }
        }
    }
}