using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    public class EventSoRaiserBehaviour : MonoBehaviour
    {
        [HideLabel]
        [HideReferenceObjectPicker]
        public EventSoRaiser raiser = new();

        [Button]
        public void Raise()
        {
            raiser?.Raise();
        }
    }
}
