using System;

namespace CatCode.Events
{
    public interface IReadOnlyEventSignal
    {
        event Action Raised;
    }
}