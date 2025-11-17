using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RAXY.Event
{
    public class EventSystemCleaner : MonoBehaviour
    {
#if UNITY_EDITOR
        [TitleGroup("All Event SO")]
        [ShowInInspector, ReadOnly]
        private List<EventBaseSO> _allEventSOs = new();

        [HorizontalGroup("All Event SO/Op")]
        [Button("Find All")]
        private void FindAllEventSO()
        {
            _allEventSOs.Clear();

            string[] guids = AssetDatabase.FindAssets("t:EventBaseSO");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EventBaseSO so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) as EventBaseSO;
                if (so != null)
                    _allEventSOs.Add(so);
            }

            Debug.Log($"[EventSystemCleaner] Found {_allEventSOs.Count} EventSO assets.");
        }

        [HorizontalGroup("All Event SO/Op")]
        [Button("Clean Up")]
        private void CleanUpAllEventSO()
        {
            FindAllEventSO();

            foreach (var eventSO in _allEventSOs)
            {
                eventSO.ClearAllListeners();
            }

            Debug.Log($"[EventSystemCleaner] Cleared listeners from {_allEventSOs.Count} EventSO assets.");
        }

        void OnDestroy()
        {
            CleanUpAllEventSO();
        }
#endif
    }
}
