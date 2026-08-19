namespace CatCode.Events
{
    public sealed class TrueCondition<T> : ICondition<T>
    {
        private static TrueCondition<T> _default = new TrueCondition<T>();
        public static TrueCondition<T> Default => _default;
        public bool Check(T value) => true;
    }
}