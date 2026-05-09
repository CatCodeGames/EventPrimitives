using System;
using UnityEngine.Pool;

namespace CatCode.Events
{
    public sealed class FuncConditionPool<T>
    {
        private readonly static ObjectPool<FuncCondition<T>> s_pool;

        static FuncConditionPool()
        {
            s_pool = new ObjectPool<FuncCondition<T>>(
                createFunc: () => new(),
                actionOnRelease: (instance) => instance.Reset(),
                collectionCheck: false);
        }

        public static FuncCondition<T> Get(Func<T, bool> predicate)
        {
            var condition = s_pool.Get();
            condition.Init(predicate);
            return condition;
        }

        public static void Release(FuncCondition<T> condition)
            => s_pool.Release(condition);

        public static PooledObject<FuncCondition<T>> Get(Func<T, bool> predicate, out FuncCondition<T> condition)
        {
            var handle = s_pool.Get(out condition);
            condition.Init(predicate);
            return handle;
        }
    }
}