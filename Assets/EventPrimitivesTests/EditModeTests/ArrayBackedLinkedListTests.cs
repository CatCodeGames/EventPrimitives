using CatCode.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace CatCode.EventPrimitives.Tests
{
    public class ArrayBackedLinkedListTests
    {
        [Test]
        public void Add()
        {
            var storage = new ArrayBackedLinkedList<int>();

            var value1 = 123;
            var slot1 = storage.Add(value1);
            Assert.AreEqual(storage.Count, 1);
            Assert.AreEqual(storage.Get(slot1), value1);
            Assert.IsTrue(storage.IsValid(slot1));

            var value2 = 345;
            var slot2 = storage.Add(value2);
            Assert.AreEqual(storage.Count, 2);
            Assert.AreEqual(storage.Get(slot2), value2);
            Assert.IsTrue(storage.IsValid(slot1));
            Assert.IsTrue(storage.IsValid(slot2));

            var value3 = 789;
            var slot3 = storage.Add(value3);
            Assert.AreEqual(storage.Count, 3);
            Assert.AreEqual(storage.Get(slot3), value3);
            Assert.IsTrue(storage.IsValid(slot1));
            Assert.IsTrue(storage.IsValid(slot2));
            Assert.IsTrue(storage.IsValid(slot3));
        }

        [Test]
        public void Remove()
        {
            var storage = new ArrayBackedLinkedList<int>();

            var value1 = 123;
            var slot1 = storage.Add(value1);
            var value2 = 345;
            var slot2 = storage.Add(value2);
            var value3 = 789;
            var slot3 = storage.Add(value3);


            storage.Remove(slot2);

            Assert.AreEqual(storage.Count, 2);

            Assert.IsTrue(storage.IsValid(slot1));
            Assert.IsFalse(storage.IsValid(slot2));
            Assert.IsTrue(storage.IsValid(slot3));

            Assert.AreEqual(storage.Get(slot1), value1);
            Assert.AreEqual(storage.Get(slot3), value3);


            storage.Remove(slot1);

            Assert.AreEqual(storage.Count, 1);

            Assert.IsFalse(storage.IsValid(slot1));
            Assert.IsFalse(storage.IsValid(slot2));
            Assert.IsTrue(storage.IsValid(slot3));

            Assert.AreEqual(storage.Get(slot3), value3);


            storage.Remove(slot3);

            Assert.AreEqual(storage.Count, 0);

            Assert.IsFalse(storage.IsValid(slot1));
            Assert.IsFalse(storage.IsValid(slot2));
            Assert.IsFalse(storage.IsValid(slot3));
        }

        [Test]
        public void GetEnumerator()
        {
            var storage = new ArrayBackedLinkedList<int>();

            var values = new[] { 123, 456, 789 };
            var slots = new SlotId[values.Length];

            for (int i = 0; i < values.Length; i++)
                slots[i] = storage.Add(values[i]);

            var index = 0;
            foreach (var value in storage)
                Assert.AreEqual(value, values[index++]);
        }

        [Test]
        public void RemoveDeferred()
        {
            var storage = new ArrayBackedLinkedList<int>();

            var values = new[] { 123, 456, 789 };
            var slots = new SlotId[values.Length];

            for (int i = 0; i < values.Length; i++)
                slots[i] = storage.Add(values[i]);

            var enumerator = storage.GetEnumerator();
            storage.RemoveDeferred(slots[1]);

            Assert.AreEqual(storage.Count, 2);

            Assert.IsTrue(storage.IsValid(slots[0]));
            Assert.IsFalse(storage.IsValid(slots[1]));
            Assert.IsTrue(storage.IsValid(slots[2]));

            Assert.AreEqual(storage.Get(slots[0]), values[0]);
            Assert.AreEqual(storage.Get(slots[2]), values[2]);

            for (int i = 0; i < values.Length; i++)
            {
                enumerator.MoveNext();
                Assert.AreEqual(enumerator.Current, values[i]);
            }

            storage.ApplyRemove();
            enumerator = storage.GetEnumerator();

            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, values[0]);
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, values[2]);
        }

        [Test]
        public void AddRemove()
        {
            var storage = new ArrayBackedLinkedList<int>();

            var values = new[] { 123, 456, 789 };
            var slots = new SlotId[values.Length];

            for (int i = 0; i < values.Length; i++)
                slots[i] = storage.Add(values[i]);

            storage.Remove(slots[1]);
            var newSlot = storage.Add(values[1]);

            Assert.AreEqual(storage.Get(slots[0]), values[0]);
            Assert.AreEqual(storage.Get(newSlot), values[1]);
            Assert.AreEqual(storage.Get(slots[2]), values[2]);
        }

        [Test]
        public void RandomizedAddRemove()
        {
            var iterations = 4;
            var capacity = 20;
            var storage = new ArrayBackedLinkedList<int>();
            var expected = new List<(SlotId Id, int Value)>();

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < capacity; i++)
                {
                    int value = iteration * iterations + i;
                    expected.Add((storage.Add(value), value));
                }

                AssertStorage(storage, expected);

                for (int i = expected.Count - 1; i > 0; i--)
                {
                    int j = UnityEngine.Random.Range(0, i + 1);
                    (expected[i], expected[j]) = (expected[j], expected[i]);
                }

                int removeCount = expected.Count / 2;

                for (int i = 0; i < removeCount; i++)
                {
                    var pair = expected[i];

                    Assert.IsTrue(storage.Remove(pair.Id));
                    Assert.IsFalse(storage.IsValid(pair.Id));
                }

                expected.RemoveRange(0, removeCount);

                AssertStorage(storage, expected);


                void AssertStorage(ArrayBackedLinkedList<int> storage, List<(SlotId Id, int Value)> expected)
                {
                    Assert.AreEqual(expected.Count, storage.Count);
                    for (int i = 0; i < expected.Count; i++)
                        Assert.AreEqual(expected[i].Value, storage.Get(expected[i].Id));
                }
            }
        }
    }
}