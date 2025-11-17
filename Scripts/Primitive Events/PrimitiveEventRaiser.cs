using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    [Serializable]
    public class PrimitiveEventRaiser
    {
        [HideLabel]
        [EnumToggleButtons]
        public PrimitiveType primitiveType;

        // Shared row layout: left (fields), right (button)
        [HorizontalGroup("Row", Width = 0.8f)]
        [VerticalGroup("Row/Left")]
        [HideLabel, ShowIf("@primitiveType == PrimitiveType.Integer")]
        public IntegerEventSO integerEventSO;

        [VerticalGroup("Row/Left")]
        [ShowIf("@primitiveType == PrimitiveType.Integer")]
        public int integerParam;

        [VerticalGroup("Row/Left")]
        [HideLabel, ShowIf("@primitiveType == PrimitiveType.Float")]
        public FloatEventSO floatEventSO;

        [VerticalGroup("Row/Left")]
        [ShowIf("@primitiveType == PrimitiveType.Float")]
        public float floatParam;

        [VerticalGroup("Row/Left")]
        [HideLabel, ShowIf("@primitiveType == PrimitiveType.Bool")]
        public BoolEventSO boolEventSO;

        [VerticalGroup("Row/Left")]
        [ShowIf("@primitiveType == PrimitiveType.Bool")]
        public bool boolParam;

        [VerticalGroup("Row/Left")]
        [HideLabel, ShowIf("@primitiveType == PrimitiveType.String")]
        public StringEventSO stringEventSO;

        [VerticalGroup("Row/Left")]
        [ShowIf("@primitiveType == PrimitiveType.String")]
        public string stringParam;

        [VerticalGroup("Row/Left")]
        [HideLabel, ShowIf("@primitiveType == PrimitiveType.NoParam")]
        public EventSO noParamEventSO;

        [VerticalGroup("Row/Left")]
        [ShowIf("@primitiveType == PrimitiveType.NoParam")]
        [DisplayAsString]
        [HideLabel]
        [ShowInInspector]
        const string NO_PARAM = "No Parameter";

        // ───────────────────────────────────────────────
        // Button on the right side
        [VerticalGroup("Row/Right")]
        [Button(ButtonHeight = 42)]
        public void Raise()
        {
            switch (primitiveType)
            {
                case PrimitiveType.Integer:
                    integerEventSO?.Raise(integerParam);
                    break;
                case PrimitiveType.Float:
                    floatEventSO?.Raise(floatParam);
                    break;
                case PrimitiveType.Bool:
                    boolEventSO?.Raise(boolParam);
                    break;
                case PrimitiveType.String:
                    stringEventSO?.Raise(stringParam);
                    break;
                case PrimitiveType.NoParam:
                    noParamEventSO?.Raise();
                    break;
            }
        }

        public static void RaiseList(List<PrimitiveEventRaiser> raisers)
        {
            foreach (var raiser in raisers)
            {
                raiser.Raise();
            }
        }
    }

    public enum PrimitiveType
    {
        Integer, Float, Bool, String, NoParam
    }
}
