using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.Event
{
    /// <summary>
    /// Unity [SerializeReference] cannot persist System.String or value types.
    /// This box wraps them so EventSoRaiser parameters survive domain reload / reopening Unity.
    /// </summary>
    [Serializable]
    public class EventParamBox<T>
    {
        [HideLabel]
        public T value;

        public EventParamBox() { }

        public EventParamBox(T value)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class EventSoRaiser : ISerializationCallbackReceiver
    {
        bool IsWithParam => eventSO is not null and not EventSO;

        [HorizontalGroup]
        [OnValueChanged("RefreshParameter")]
        public EventBaseSO eventSO;

        [SerializeReference]
        [HideReferenceObjectPicker]
        [ShowIf("IsWithParam")]
        [LabelText("@ParameterTypeName")]
        public object parameter;

        string ParameterTypeName
        {
            get
            {
                Type type = eventSO != null && eventSO.HasParameter
                    ? eventSO.ParameterType
                    : GetBoxedValueType(parameter);

                return type?.GetCSharpName() ?? "None";
            }
        }

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
            Type storageType = GetStorageType(paramType);

            // Sudah sesuai tipenya, tidak perlu dibuat ulang.
            if (parameter != null && parameter.GetType() == storageType)
                return;

            object previousValue = Unwrap(parameter);
            parameter = CreateStorage(paramType, previousValue);
        }

        //[Button]
        public void Raise()
        {
            eventSO?.Raise(Unwrap(parameter));
        }

        public void OnBeforeSerialize()
        {
            EnsureParameterStorage();
        }

        public void OnAfterDeserialize()
        {
            EnsureParameterStorage();
        }

        void EnsureParameterStorage()
        {
            if (eventSO == null || !eventSO.HasParameter)
                return;

            Type paramType = eventSO.ParameterType;
            if (paramType == null)
                return;

            Type storageType = GetStorageType(paramType);
            if (parameter != null && parameter.GetType() == storageType)
                return;

            // Migrate raw string / boxed value types (or null) into a serializable box.
            if (RequiresBox(paramType))
            {
                parameter = CreateStorage(paramType, Unwrap(parameter));
                return;
            }

            // Reference types that somehow became null stay null until Refresh.
        }

        static bool RequiresBox(Type paramType)
        {
            return paramType == typeof(string) || paramType.IsValueType;
        }

        static Type GetStorageType(Type paramType)
        {
            return RequiresBox(paramType)
                ? typeof(EventParamBox<>).MakeGenericType(paramType)
                : paramType;
        }

        static object CreateStorage(Type paramType, object previousValue)
        {
            if (RequiresBox(paramType))
            {
                Type boxType = typeof(EventParamBox<>).MakeGenericType(paramType);
                object box = Activator.CreateInstance(boxType);
                FieldInfo valueField = boxType.GetField(nameof(EventParamBox<object>.value));

                if (previousValue != null && paramType.IsInstanceOfType(previousValue))
                    valueField.SetValue(box, previousValue);
                else if (paramType == typeof(string))
                    valueField.SetValue(box, string.Empty);
                else if (paramType.IsValueType)
                    valueField.SetValue(box, Activator.CreateInstance(paramType));

                return box;
            }

            if (previousValue != null && paramType.IsInstanceOfType(previousValue))
                return previousValue;

            try
            {
                return Activator.CreateInstance(paramType);
            }
            catch
            {
                // Abstract class / no parameterless ctor.
                return null;
            }
        }

        static object Unwrap(object stored)
        {
            if (stored == null)
                return null;

            Type type = stored.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventParamBox<>))
                return type.GetField(nameof(EventParamBox<object>.value)).GetValue(stored);

            return stored;
        }

        static Type GetBoxedValueType(object stored)
        {
            if (stored == null)
                return null;

            Type type = stored.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventParamBox<>))
                return type.GetGenericArguments()[0];

            return type;
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
