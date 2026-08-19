namespace CatCode.EventPrimitives
{
    public interface ICondition<T>
    {
        bool Check(T value);
    }
}