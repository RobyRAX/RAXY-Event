using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    [CreateAssetMenu(menuName = "RAXY/Event System/Event")]
    public class EventSO : EventBaseSO
    {
        public event Action Event;

#if UNITY_EDITOR
        protected override Delegate[] GetDelegates()
        {
            return Event?.GetInvocationList() ?? Array.Empty<Delegate>();
        }
#endif

        public virtual void Subscribe(Action action)
        {
            Event += action;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

        public virtual void Unsubscribe(Action action)
        {
            Event -= action;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

        public override void Raise()
        {
            Event?.Invoke();
        }

        public override void ClearAllListeners()
        {
            Event = null;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }
    }

    public abstract class EventSO<T> : EventBaseSO
    {
        public event Action<T> Event;

#if UNITY_EDITOR
        protected override Delegate[] GetDelegates()
        {
            return Event?.GetInvocationList() ?? Array.Empty<Delegate>();
        }
#endif

        [ShowInInspector]
        [ReadOnly]
        [TitleGroup("Status")]
        [HideLabel]
        protected T _currentParam;

        public virtual void ResetParam()
        {
            _currentParam = default;
        }

        public virtual void Subscribe(Action<T> action)
        {
            Event += action;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

        public virtual void Unsubscribe(Action<T> action)
        {
            Event -= action;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

        /// <summary>
        /// Will raise the event with 'default' argument
        /// </summary>
        public override void Raise()
        {
            _currentParam = default;
            Event?.Invoke(_currentParam);
        }

        public virtual void Raise(T param)
        {
            _currentParam = param;
            Event?.Invoke(_currentParam);
        }

        public override void ClearAllListeners()
        {
            Event = null;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }
    }
}
