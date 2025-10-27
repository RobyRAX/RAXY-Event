using UnityEngine;

namespace RAXY.Event
{
    public abstract class EventRaiserBase
    {
        public EventBaseSO targetEvent;

        public void Raise()
        {
            targetEvent?.Raise();
        }
    }

    public abstract class EventRaiserBase<T>
    {
        public EventBaseSO<T> targetEvent;
        public T arg;

        public void Raise()
        {
            targetEvent?.Raise(arg);
        }
    }
}
