using System;

namespace CatCode.EventPrimitives
{
    public sealed class FuncCondition<T> : ICondition<T>
    {
        private Func<T, bool> _pred;

        public FuncCondition() { }

        public void Init(Func<T, bool> pred)
            => _pred = pred;

        public bool Check(T value)
            => _pred(value);

        public void Reset()
            => _pred = null;
    }
}