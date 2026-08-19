using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CatCode.EventPrimitives.Tests
{
    public sealed class EventTests
    {
        [Test]
        public void EventSource_SubscribeUnsubscribe()
        {
            var value = false;
            var eventSource = new EventSource();

            Assert.IsFalse(value);

            eventSource.Raised += OnRaised;
            eventSource.Raise();

            Assert.IsTrue(value);

            eventSource.Raised -= OnRaised;
            value = false;
            eventSource.Raise();

            Assert.IsFalse(value);

            void OnRaised()
            {
                value = true;
            }
        }

        [Test]
        public void EventSource_Generic_SubscribeUnsubscribe()
        {
            var currentValue = 0;
            var defaultValue = 0;
            var targetValue = 100;
            var eventSource = new EventSource<int>();

            Assert.AreEqual(currentValue, defaultValue);

            eventSource.Raised += OnRaised;
            eventSource.Raise(targetValue);

            Assert.AreEqual(currentValue, targetValue);

            eventSource.Raised -= OnRaised;
            currentValue = defaultValue;
            eventSource.Raise(targetValue);

            Assert.AreEqual(currentValue,defaultValue);

            void OnRaised(int value)
            {
                currentValue = value;
            }
        }


        [Test]
        public async Task EventSource_WaitAsync()
        {
            var source = new EventSource();

            var publishTask = Publish();
            var checkTask = Check();

            await UniTask.WhenAll(publishTask, checkTask)
                .Timeout(TimeSpan.FromSeconds(1))
                .AsTask();

            async UniTask Publish()
            {
                await UniTask.Delay(100);
                source.Raise();
            }

            async UniTask Check()
            {
                await source.WaitAsync();
            }
        }

        [Test]
        public async Task EventSource_Generic_WaitAsync()
        {
            var source = new EventSource<int>();

            var publishTask = Publish();
            var checkTask = Check();

            await UniTask.WhenAll(publishTask, checkTask)
                .Timeout(TimeSpan.FromSeconds(1));

            async UniTask Publish()
            {
                await UniTask.Delay(100);
                source.Raise(123);

                await UniTask.Delay(100);
                source.Raise(456);
            }

            async UniTask Check()
            {
                await source.WaitAsync(v => v == 123);
                await source.WaitAsync(v => v == 456);
            }
        }
    }
}