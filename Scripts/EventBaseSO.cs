using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RAXY.Event
{
    public abstract class EventBaseSO : ScriptableObject
    {
        public event Action Event;

#if UNITY_EDITOR
        [TitleGroup("Listeners")]
        [ShowInInspector, HideReferenceObjectPicker, HideLabel]
        private DelegatesDrawer _delegatesDrawer = new();
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

        public virtual void Raise()
        {
            Event?.Invoke();
        }

        public virtual void ClearAllListeners()
        {
            Event = null;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

#if UNITY_EDITOR
        private void RefreshVisualizer()
        {
            if (_delegatesDrawer == null)
                _delegatesDrawer = new DelegatesDrawer();

            var list = Event?.GetInvocationList() ?? Array.Empty<Delegate>();
            _delegatesDrawer.SetDelegates(list);
        }
#endif
    }

    public abstract class EventBaseSO<T> : ScriptableObject
    {
        public event Action<T> Event;

        [ShowInInspector]
        [ReadOnly]
        [TitleGroup("Status")]
        [HideLabel]
        protected T _currentParam;

#if UNITY_EDITOR
        [TitleGroup("Listeners")]
        [ShowInInspector, HideReferenceObjectPicker, HideLabel]
        private DelegatesDrawer _delegatesDrawer = new();
#endif

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

        public virtual void Raise(T param)
        {
            _currentParam = param;
            Event?.Invoke(_currentParam);
        }

        public virtual void ClearAllListeners()
        {
            Event = null;
#if UNITY_EDITOR
            RefreshVisualizer();
#endif
        }

#if UNITY_EDITOR
        private void RefreshVisualizer()
        {
            if (_delegatesDrawer == null)
                _delegatesDrawer = new DelegatesDrawer();

            var list = Event?.GetInvocationList() ?? Array.Empty<Delegate>();
            _delegatesDrawer.SetDelegates(list);
        }
#endif
    }

#if UNITY_EDITOR
    [Serializable]
    public class DelegatesDrawer
    {
        private Delegate[] _delegates = Array.Empty<Delegate>();

        [ShowInInspector, ReadOnly, LabelText("Listener Count")]
        private int ListenerCount => _delegates?.Length ?? 0;

        [ShowInInspector, ReadOnly, LabelText("Active Listeners")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        private List<string> _listenerDebugList = new();

        public void SetDelegates(Delegate[] delegates)
        {
            _delegates = delegates ?? Array.Empty<Delegate>();
            RefreshList();
        }

        private void RefreshList()
        {
            _listenerDebugList.Clear();

            if (_delegates == null || _delegates.Length == 0)
            {
                _listenerDebugList.Add("<no listeners>");
                return;
            }

            foreach (var del in _delegates)
            {
                if (del == null)
                {
                    _listenerDebugList.Add("<null delegate>");
                    continue;
                }

                string targetName = del.Target != null ? del.Target.ToString() : "<null>";
                string methodName = del.Method != null ? del.Method.Name : "<unknown>";

                // Detect compiler-generated lambda names
                if (methodName.Contains("b__"))
                {
                    int start = methodName.IndexOf('<') + 1;
                    int end = methodName.IndexOf('>');
                    if (start >= 0 && end > start)
                        methodName = $"<lambda from {methodName.Substring(start, end - start)}>";
                    else
                        methodName = "<lambda>";
                }

                _listenerDebugList.Add($"{targetName} → {methodName}");
            }
        }
    }
#endif
}
