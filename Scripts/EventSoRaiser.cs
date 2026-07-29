using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    [Serializable]
    public class EventSoRaiser
    {
        bool IsNoParam => eventSO is EventSO;
        bool IsWithParam => eventSO is not null and not EventSO;

        [HorizontalGroup]
        [OnValueChanged("RefreshParameter")]
        public EventBaseSO eventSO;

        [SerializeReference]
        [HideReferenceObjectPicker]
        [ShowIf("IsWithParam")]
        [LabelText("@ParameterTypeName")]
        public object parameter;

        string ParameterTypeName => parameter?.GetType().GetCSharpName() ?? "None";

        [HorizontalGroup(0.15f)]
        [ShowIf("IsWithParam")]
        [Button("Refresh")]
        void RefreshParameter()
        {
            if (eventSO is null)
                return;

            if (!eventSO.HasParameter)
            {
                parameter = null;
                return;
            }

            Type paramType = eventSO.ParameterType;

            // Sudah sesuai tipenya, tidak perlu dibuat ulang.
            if (parameter != null && parameter.GetType() == paramType)
                return;

            // string tidak punya default ctor
            if (paramType == typeof(string))
            {
                parameter = string.Empty;
            }
            // Value type (int, float, bool, Vector3, dll)
            else if (paramType.IsValueType)
            {
                parameter = Activator.CreateInstance(paramType);
            }
            // Reference type
            else
            {
                try
                {
                    parameter = Activator.CreateInstance(paramType);
                }
                catch
                {
                    // Misalnya abstract class atau tidak punya ctor kosong.
                    parameter = null;
                }
            }
        }

        [Button]
        public void Raise()
        {
            eventSO?.Raise(parameter);
        }
    }

    public static class TypeExtensions
    {
        public static string GetCSharpName(this Type type)
        {
            if (type == typeof(bool)) return "bool";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(char)) return "char";
            if (type == typeof(short)) return "short";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(string)) return "string";
            if (type == typeof(object)) return "object";

            return type.Name;
        }
    }
}
