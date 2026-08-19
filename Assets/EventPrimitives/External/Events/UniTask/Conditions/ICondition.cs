namespace CatCode.Events
{
    public interface ICondition<T>
    {
        bool Check(T value);
    }
}