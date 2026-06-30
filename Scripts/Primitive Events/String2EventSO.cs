using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    [CreateAssetMenu(menuName = "RAXY/Event System/Primitive/String2")]
    public class String2EventSO : PrimitiveEventBaseSO<String2>
    {
    }

    [Serializable]
    public struct String2
    {
        [HorizontalGroup]
        public string x;

        [HorizontalGroup]
        public string y;
    }
}
