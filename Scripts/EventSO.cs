using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    [CreateAssetMenu(menuName = "RAXY/Event System/Event")]
    public class EventSO : EventBaseSO
    {
        public override bool HasParameter => false;
        public override Type ParameterType => null;

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
            //Debug.Log("Subscribe -> " + GetInstanceID());
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

        public void Raise()
        {
            //Debug.Log("Raise -> " + GetInstanceID());
            Event?.Invoke();
        }

        public override void Raise(object parameter = null)
        {
            Raise();
        }

        public override void ClearAllListeners()
        {
            Event = null;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }
    }

    public class EventSO<T> : EventBaseSO
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

        public override bool HasParameter => true;
        public override Type ParameterType => typeof(T);

        public virtual void ResetParam()
        {
            _currentParam = default;
        }

        public virtual void Subscribe(Action<T> action)
        {
            Event += action;
            //Debug.Log("Subscribe -> " + GetInstanceID());
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

        public virtual void Unsubscribe(Action<T> action)
        {
            Event -= action;
            //Debug.Log("Unsubscribe -> " + GetInstanceID());
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

        public virtual void Raise(T param)
        {
            _currentParam = param;
            //Debug.Log("Raise -> " + GetInstanceID());
            Event?.Invoke(_currentParam);
        }

        public override void Raise(object parameter = null)
        {
            Raise((T)parameter);
        }

        public override void ClearAllListeners()
        {
            Event = null;
            //Debug.Log("Clear -> " + GetInstanceID());
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }
    }
}
